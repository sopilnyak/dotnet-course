/*
    1. Подготовка
    Найти десяток новостных порталов с RSS лентой, записать их в файл.

    2. Задание
    Раз в N минут нужно читать файл со списком адресов RSS лент (rss_list.txt), а так же файл со списоком уже обработанных ссылок (processed_articles.txt).
    Каждую RSS-ленту нужно скачать, распарсить, достать из неё список ссылок на новостные статьи.
    По тем ссылкам, которые ещё не было ранее обработаны, нужно скачать html содержимое.
    Содежимое нужно сохранить на диск, а так же записать в файл processed_articles.txt информацию о том что ссылка была обработана.

    3. Логи
    В процессе работы нужно писать логи в консоль: лента обработана, статья скачена и т.д. 
    После каждой итерации нужно выводить статистику: сколько лент обработано, сколько новых новостей, сколько старых.


    10 баллов
    Мягкий дедлайн: 31.03.2022 23:59
    Жесткий дедлайн: 12.05.2022 23:59
*/

using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;


public static class ProcessedArticles
{
    static string processed_articles = "processed_articles.txt";    
    static ReaderWriterLock locker = new ReaderWriterLock();
    public static void MarkProcessed(this Uri link)
    {
        try
        {
            locker.AcquireWriterLock(500);
            System.IO.File.AppendAllLines(processed_articles, new[] { link.ToString() });
        }
        finally
        {
            locker.ReleaseWriterLock();
        }
    }

    public static string[] Get()
    {
        string[] result = new string[] {};

        try
        {
            locker.AcquireWriterLock(500);
            result = System.IO.File.ReadAllLines(processed_articles);
        }
        finally
        {
            locker.ReleaseWriterLock();
        }

        return result;
    }
}


public class AtomicCounter {
    public int Value;
}


static class Program
{
    const string rss_list = "./rss_list.txt";
    static int delay_minutes = 1;


    static bool IsNewsOld(string news_link)
    {
        return ProcessedArticles.Get().Contains(news_link);
    }


    static async Task DownloadNews(string host, Uri news_link, AtomicCounter successful_downloads_count, AtomicCounter old_news_count)
    {
        if (IsNewsOld(news_link.ToString()))
        {
            Interlocked.Increment(ref old_news_count.Value);
            Console.WriteLine($"Article is already downloaded: {news_link.ToString()}");
            return;
        }

        try
        {
            using (WebClient client = new WebClient())
            {
                var news_filename = $"{news_link.ToString().Replace('/', '_')}.html";
                await client.DownloadFileTaskAsync(news_link, $"./news/{host}/{news_filename}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(
               "Exception occured while trying "
                + $"to download from {news_link.ToString()}. "
                + $"Exception: {e.ToString()}"
            );
            return;
        }

        news_link.MarkProcessed();

        Console.WriteLine($"Downloaded article: {news_link.ToString()}");
        Interlocked.Increment(ref successful_downloads_count.Value);
    }


    static async Task<SyndicationFeed> DownloadFeed(string feed_rss_link)
    {
        XmlReader reader = XmlReader.Create(feed_rss_link);
        SyndicationFeed feed = await Task.Run(() => SyndicationFeed.Load(reader));
        reader.Close();
        return feed;
    }


    static async Task UpdateNews(string feed_rss_link, AtomicCounter successful_processed_feeds_count,
                                 AtomicCounter total_news_downloaded, AtomicCounter total_news_old,
                                 AtomicCounter total_news_failed)
    {
        var feed = new SyndicationFeed();
        try
        {
            feed = await DownloadFeed(feed_rss_link);
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Exception occured while trying "
                + $"to process {feed_rss_link}. "
                + $"Exception: {e.ToString()}"
            );
            return;
        }

        Console.WriteLine($"Feed downloaded: {feed_rss_link}");

        var feed_uri = new Uri(feed_rss_link);
        try
        {
            Directory.CreateDirectory($"./news/{feed_uri.Host}");
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Exception occured while trying "
                + $"to make dir for {feed_rss_link}. "
                + $"Exception: {e.ToString()}"
            );
            return;
        }
        Console.WriteLine($"Directory created for {feed_rss_link}");

        var successful_downloads_count = new AtomicCounter();
        var old_news_count = new AtomicCounter();
        try
        {
            await Task.WhenAll(
                    feed.Items
                    .Select(item => item.Links.First().Uri)
                    .Select(news_link => DownloadNews(
                        feed_uri.Host, news_link, successful_downloads_count, old_news_count))
                );
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Exception occured while trying "
                + $"to download news from {feed_rss_link}." 
                + $"Exception: {e.ToString()}"
            );
            return;
        }

        var news_count = feed.Items.Count();
        var news_failed = news_count - successful_downloads_count.Value - old_news_count.Value;
        Console.WriteLine(
            $"Feed processed: {feed_rss_link}. "
            + $"Downloaded new - {successful_downloads_count.Value}, "
            + $"seen old - {old_news_count.Value}, "
            + $"failed to download - {news_failed}."
        );

        Interlocked.Increment(ref successful_processed_feeds_count.Value);

        Interlocked.Add(ref total_news_downloaded.Value, successful_downloads_count.Value);
        Interlocked.Add(ref total_news_old.Value, old_news_count.Value);
        Interlocked.Add(ref total_news_failed.Value, news_failed);
    }


    static async Task KeepNewsUpToDate(object state)
    {
        var cancellation_token = (CancellationToken)state;

        for (var iteration = 0; ; iteration++)
        {
            var successful_processed_feeds_count = new AtomicCounter();
            var total_news_downloaded = new AtomicCounter();
            var total_news_old = new AtomicCounter();
            var total_news_failed = new AtomicCounter();

            await Task.WhenAll(
                File.ReadLines(rss_list)
                .Select(link => UpdateNews(
                    link, successful_processed_feeds_count,
                    total_news_downloaded, total_news_old, total_news_failed
                ))
            );
            Console.WriteLine(
                $"Feeds processed: {File.ReadLines(rss_list).Count()}. "
                + $"{successful_processed_feeds_count.Value} out of them processed successfully"
            );

            Console.WriteLine(
                $"Successfully downloaded news: {total_news_downloaded.Value}\n"
                + $"Old news processed: {total_news_old.Value}\n"
                + $"News failed to download: {total_news_failed.Value}"
            );

            Thread.Sleep(delay_minutes * 60 * 1000);

            if (cancellation_token.IsCancellationRequested)
            {
                break;
            }
        }
    }

    static async Task Run()
    {
        var cancellation_token = new CancellationTokenSource();
        await KeepNewsUpToDate(cancellation_token.Token);

        Console.WriteLine("Введите <Enter> для остановки");
        Console.ReadLine();
        cancellation_token.Cancel();

        Console.WriteLine("Остановлено");
    }

    static void Main(string[] args)
    {
        Run().Wait();
    }
}