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

using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Util;

namespace _05_multithreading;

public static class EventLoop
{
    private static readonly Logger Logger = new(LogDst.Console);
    private static Stats _stats = new();
    private static System.Threading.Timer? _timer;
    private const int PeriodMs = 60000;
    private const string RssFeedLinksFilePath = "./rss_list.txt";
    private const string ProcessedArticlesListFilePath = "./processed_articles.txt";

    public static void Start()
    {
        _timer = new System.Threading.Timer(EventHandler, null, 0, PeriodMs);
    }

    public static void Stop()
    {
        if (_timer is null) return;
        _timer.Dispose();
    }

    private static void EventHandler(object? _)
    {
        try
        {
            InvokeLoadingArticles();
        }
        catch (Exception ex)
        {
            Logger.WriteLine("Caught iteration exception with message \"" + ex.Message + "\"");
        }
    }

    private static void InvokeLoadingArticles()
    {
        _stats.Clear();
        Logger.WriteLine("Iteration started");

        var rssFeedLinks = ReadRssFeedLinks();
        var rssFeeds = RssFeedLoader.LoadRssFeeds(rssFeedLinks);
        Logger.WriteLine("Feeds loaded");
        var processedArticlesNames = ReadProcessedArticleNames();
        foreach (var feed in rssFeeds)
        {
            var feedArticles = RssFeedProcessor.GetArticles(feed);
            var unprocessedArticles = FilterUnprocessedArticles(feedArticles, processedArticlesNames);
            var loadedArticles = LoadArticles(unprocessedArticles);
            WriteArticlesToFile(loadedArticles);

            _stats.FeedsProcessed++;
            _stats.NewArticles += unprocessedArticles.Count;
            _stats.OldArticles += feedArticles.Count - unprocessedArticles.Count;
            Logger.WriteLine("Feed processed");
        }
        LogStats();
        Logger.WriteLine("Iteration finished");
    }

    private static List<string> ReadLinesFromFile(string path)
    {
        using var reader = new StreamReader(path);
        List<string> result = new();
        while (reader.ReadLine() is { } line)
        {
            result.Add(line);
        }

        reader.Close();

        return result;
    }

    private static List<string> ReadRssFeedLinks()
    {
        return ReadLinesFromFile(RssFeedLinksFilePath);
    }

    private static List<string> ReadProcessedArticleNames()
    {
        return ReadLinesFromFile(ProcessedArticlesListFilePath);
    }

    private static List<(string name, string link)> FilterUnprocessedArticles(List<(string name, string link)> articles,
        List<string> processedArticleNames)
    {
        return articles.Where(article => !processedArticleNames.Contains(article.name)).ToList();
    }

    private static List<Article> LoadArticles(List<(string name, string link)> articles)
    {
        List<Task<Article>> articlesLoadTasks = new();
        foreach (var article in articles)
        {
            var articleLoadTask = Article.Load(article.name, article.link);
            articlesLoadTasks.Add(articleLoadTask);
            Logger.WriteLine("Article \"" + article.name + "\" loaded");
        }

        Task allLoaded = Task.WhenAll(articlesLoadTasks);
        allLoaded.Wait();

        return articlesLoadTasks.Select(task => task.Result).ToList();
    }

    private static void WriteArticlesToFile(List<Article> articles)
    {
        var writer = new StreamWriter(ProcessedArticlesListFilePath);
        foreach (var article in articles)
        {
            article.WriteToFile().ContinueWith(_ =>
            {
                writer.WriteLine(article.Name);
                Logger.WriteLine("Article \"" + article.Name + "\" written to file");
            });
        }
    }

    private static void LogStats()
    {
        var logMessage = "Iteration stats :" +
                         "\n\tFeeds processed: " + _stats.FeedsProcessed +
                         "\n\t New articles: " + _stats.NewArticles +
                         "\n\t Old articles: " + _stats.OldArticles +
                         "\n";
        Logger.WriteLine(logMessage);
    }

    private struct Stats
    {
        public int FeedsProcessed { get; set; } = 0;
        public int NewArticles { get; set; } = 0;
        public int OldArticles { get; set; } = 0;

        public Stats()
        {
        }

        public void Clear()
        {
            FeedsProcessed = 0;
            NewArticles = 0;
            OldArticles = 0;
        }
    }
}

public class Article
{
    public string Name { get; }
    private readonly string _htmlPage;
    private static readonly HttpClient HttpClient = new();

    public Article(string name, string htmlPage)
    {
        Name = name;
        _htmlPage = htmlPage;
    }

    public async Task WriteToFile()
    {
        var fileName = Regex.Replace("./articles/" + Name + ".txt", @"\s+", "_");
        await using var writer = File.CreateText(fileName);
        await writer.WriteAsync(_htmlPage);
    }

    public static async Task<Article> Load(string name, string url)
    {
        var htmlPageString = await HttpClient.GetStringAsync(url);
        return new Article(name, htmlPageString);
    }
}

public static class RssFeedProcessor
{
    public static List<(string name, string link)> GetArticles(XDocument feed)
    {
        List<(string name, string link)> articles = new();
        foreach (var item in feed.Descendants("item"))
        {
            if (!(item.Elements("title").Any() && item.Elements("link").Any())) continue;

            articles.Add((item.Element("title")!.Value, item.Element("link")!.Value));
        }

        return articles;
    }
}

public static class RssFeedLoader
{
    private static readonly HttpClient HttpClient = new();

    public static List<XDocument> LoadRssFeeds(List<string> links)
    {
        List<Task<XDocument>> loadTasks = new(links.Count);
        loadTasks.AddRange(links.Select(LoadFeed));

        Task allLoaded = Task.WhenAll(loadTasks);
        allLoaded.Wait();

        return loadTasks.Select(task => task.Result).ToList();
    }

    private static async Task<XDocument> LoadFeed(string url)
    {
        var rssFeedString = await HttpClient.GetStringAsync(url);
        return XDocument.Parse(rssFeedString);
    }
}