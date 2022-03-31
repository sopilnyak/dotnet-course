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


using System.Net;
using System.Xml;
using System.ServiceModel.Syndication;
using logger;

public class ProcessedArticlesLinks {
    public void Append(string[] batch_link) {
        try {
            rwlock_.AcquireWriterLock(200);
            File.AppendAllLines(location_, batch_link);
           // ram_is_up_to_date_ = false;
        }
        finally {
            rwlock_.ReleaseWriterLock();
        }
    }

    public string[] Read() {
        string[] links = {};
        try {
            rwlock_.AcquireReaderLock(200);
            //if (!ram_is_up_to_date_) {
                links = File.ReadAllLines(location_);
                //ram_is_up_to_date_ = true;
            //}
        }
        finally {
            rwlock_.ReleaseReaderLock();
        }

        return links;
    }

    public bool HasLink(string link) {

        //if (!ram_is_up_to_date_) {
        string[] links = Read();
        //}

        return links.Contains(link);
    }
    
    //private string[] links_;
    //private bool ram_is_up_to_date_ = true;
    private ReaderWriterLock rwlock_ = new();
    private static readonly string location_ = "./links/processed_articles.txt";
}

public struct Info {
    public int new_articles;
    public int old_articles;
    public int failed_downloads;
}

public class NewsDownloader {
    public async Task AsyncDownload(Uri link, string path_to_download, Info info) {
        if (processed_.HasLink(link.ToString())) {
            Interlocked.Increment(ref info.old_articles);
            return;
        }

        try {
            using (var client = new WebClient()) {
                await client.DownloadFileTaskAsync(link, path_to_download);
            }
            Interlocked.Increment(ref info.new_articles);
        }
        catch (Exception e) {
            Console.WriteLine($"While downloading from {link} an exception was triggered: {e.ToString()}!");
        }
        
        processed_.Append(new[]{link.ToString()});
    }

    public async Task PrepareAndDownload(Uri link, Uri rss_uri, Info local_info) {
        var news_itself = $"{link.ToString().Replace('/', '_')}.html";
        var filepath = $"./news/{rss_uri.Host}/{news_itself}";
        await AsyncDownload(link, filepath, local_info);
    }

    private readonly ProcessedArticlesLinks processed_ = new();
}


public class RSSParser {
    public static async Task<SyndicationFeed> GetFeed(string rss_link) {
        var reader =  XmlReader.Create(rss_link);
        var feed = await Task.Run(() => SyndicationFeed.Load(reader));
        reader.Close();
        return feed;
    }
}

public class Program {
    public static async Task ParseRss(string rss_link, ILogger logger, Info info) {
        SyndicationFeed feed = await RSSParser.GetFeed(rss_link);
        
        logger.Log($"Got feed on link {rss_link} successfully.");

        Info local_info = new Info();
        
        var rss_uri = new Uri(rss_link);
        try {
            Directory.CreateDirectory($"./news/{rss_uri.Host}");
        }
        catch (Exception e) {
            logger.Log($"While creating directory {rss_uri.Host} an exception occuried: {e.ToString()}!");
        }
        
        logger.Log($"Directory {rss_uri.Host} created successfully.");

        NewsDownloader downloader = new NewsDownloader();
        try {
            await Task.WhenAll(
                feed.Items
                    .Select(item => item.Links.First().Uri)
                    .Select(link => downloader.PrepareAndDownload(link, rss_uri, local_info)));
        }
        catch (Exception e) {
            logger.Log($"While downloading articles an exception occurred: {e.ToString()}!");
        }

        var overall_news = feed.Items.Count();
        var failed_cnt = overall_news - local_info.new_articles - local_info.old_articles;
        
        logger.Log($"Для новостей из {rss_uri.Host} получено:\n" +
                   $"Новых статей - {local_info.new_articles}\n" +
                   $"Старых статей - {local_info.old_articles}\n" +
                   $"Неуспешных скачиваний - {failed_cnt}");
        
        Interlocked.Add(ref info.new_articles, local_info.new_articles);
        Interlocked.Add(ref info.old_articles, local_info.old_articles);
        Interlocked.Add(ref info.failed_downloads, failed_cnt);
    }
    
    public static async Task ProceedIteration() {
        ILogger logger = new ConsoleLogger();
        Info info = new Info();

        await Task.WhenAll(
            File.ReadLines("./links/rss_list.txt")
                .Select(link => ParseRss(link, logger, info)));
        
        logger.Log($"Обработано {info.new_articles + info.old_articles + info.failed_downloads} статей, из которых:");
        logger.Log($"Получено {info.new_articles} новых статей.");
        logger.Log($"Получено {info.old_articles} старых статей.");
        logger.Log($"Неудачных скачиваний: {info.failed_downloads}");
    }
   
    public static void Main(string[] args) {
        ProceedIteration().Wait();
    }
}
