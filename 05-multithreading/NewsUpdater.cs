using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;

public class AtomicState
{
    public int Downloaded;
    public int Old;
    public int Error;
}

static class NewsUpdater
{
    static Logger logger = new Logger(LoggerFlag.All);
    const string rss_list = "./data/rss_list.txt";
    const string news_dir = "./data/news";
    const int delay = 100000;

    static bool IsOld(string news_link) => ProcessedMarker.GetProcessed().Contains(news_link);
    static async Task DownloadNews(Uri link, AtomicState state, string host_name)
    {
        if (IsOld(link.ToString()))
        {
            Interlocked.Increment(ref state.Old);
            logger.Log($"{link.ToString()} уже скачан, проигнорирован", LoggerFlag.Articles);
            return;
        }

        try
        {
            using (WebClient client = new WebClient())
            {
                logger.Log($"Скачиваем {link.ToString()}", LoggerFlag.Articles);
                await client.DownloadFileTaskAsync(link, $"{news_dir}/{host_name}/" +
                    $"{link.ToString().Replace('/', '_').Replace(':', '_') + ".html"}");
            }
        }
        catch (Exception e)
        {
            logger.Log($"Ошибка при скачивании {link.ToString()}", LoggerFlag.Errors | LoggerFlag.Articles);
            Interlocked.Increment(ref state.Error);
            return;
        }

        Interlocked.Increment(ref state.Downloaded);
        link.MarkProcessed();
    }

    static async Task HandleFeed(string rss, AtomicState state)
    {
        SyndicationFeed feed;
        try
        {
            using XmlReader reader = XmlReader.Create(rss);
            feed = await Task.Run(() => SyndicationFeed.Load(reader));
            reader.Close();
        }
        catch (Exception e)
        {
            logger.Log($"Не удалось скачать ленту с {rss}", LoggerFlag.Errors | LoggerFlag.Feeds);
            return;
        }
        logger.Log($"Загружена лента {rss}", LoggerFlag.Feeds);

        var feed_uri = new Uri(rss);
        Directory.CreateDirectory($"{news_dir}/{feed_uri.Host}");

        var update_state = new AtomicState();
        try
        {
            await Task.WhenAll(feed.Items.Select(link => link.Links.First().Uri)
                .Select(link => DownloadNews(link, update_state, feed_uri.Host)));
        }
        catch (Exception e)
        {
            logger.Log($"Ошибка подключения с {rss}", LoggerFlag.Errors | LoggerFlag.Feeds);
            return;
        }

        logger.Log($"Результат работы для {rss}\n" + 
            $"[скачаны/проигнорированы/ошибка] {update_state.Downloaded}/{update_state.Old}/{update_state.Error}\n", LoggerFlag.Feeds);

        Interlocked.Add(ref state.Downloaded, update_state.Downloaded);
        Interlocked.Add(ref state.Old, update_state.Old);
        Interlocked.Add(ref state.Error, update_state.Error);
    }

    public static async Task UpdateLoop(LoggerFlag f)
    {
        while (true)
        {
            logger = new Logger(f);
            var state = new AtomicState();
            await Task.WhenAll(File.ReadLines(rss_list).Select(rss => HandleFeed(rss, state)));

            logger.Log($"Лент обработано - {File.ReadLines(rss_list).Count()}", LoggerFlag.Loop);
            logger.Log($"Новости [скачаны/проигнорированы/ошибка] {state.Downloaded}/{state.Old}/{state.Error}", LoggerFlag.Loop);
            logger.Log($"======================================================\n", LoggerFlag.Loop);
            Thread.Sleep(delay);
        }
    }
}