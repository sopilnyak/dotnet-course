using util;

namespace _05_multithreading;

public static class EventLoop
{
    private static readonly ConsoleLogger Logger = new();
    private static Rss.Stats _stats = new();
    private static Timer _timer = null!;
    private const int PeriodMs = 60000;
    // private const int ArticlePerIterationLimit = 5;
    private const string RssFeedLinksFilePath = "./rss_list.txt";
    private const string ProcessedArticlesListFilePath = "./processed_articles.txt";

    public static void Start()
    {
        _timer = new Timer(EventHandler, null, 0, PeriodMs);
    }

    public static void Stop()
    {
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
        
        var processedArticlesNames = ReadProcessedArticleNames().ToList();
        var rssFeedLinks = ReadRssFeedLinks();
        Logger.WriteLine("Load rss links from file");

        var feedProcessTasks = new List<Task>();
        foreach (var url in rssFeedLinks)
        {
            feedProcessTasks.Add(ProcessFeed(url, processedArticlesNames));
        }
        Task.WhenAll(feedProcessTasks).Wait();

        _stats.LogStats(Logger);
        Logger.WriteLine("Iteration finished");
    }

    private static async Task ProcessFeed(string url, List<string> processedArticlesNames)
    {
        try
        {
            var feed = await Rss.Loader.LoadFeed(url);
            Logger.WriteLine($"Feed {url} loaded");

            var feedArticles = Rss.Processor.GetArticles(feed);
            var unprocessedArticles = FilterUnprocessedArticles(feedArticles, processedArticlesNames);
            Interlocked.Add(ref _stats.OldArticles, feedArticles.Count - unprocessedArticles.Count);
            
            var articleProcessTasks = new List<Task>();
            foreach (var (name, link) in unprocessedArticles)
            {
                articleProcessTasks.Add(ProcessArticle(name, link));
            }
            Task.WhenAll(articleProcessTasks).Wait();
        }
        catch (Exception ex)
        {
            Logger.WriteLine(
                $"Feed {url} loading finished with exception : {ex.Message}");
        }

        Interlocked.Add(ref _stats.FeedsProcessed, 1);
        Logger.WriteLine($"Feed {url} processed");
    }

    private static async Task ProcessArticle(string name, string link)
    {
        try
        {
            var article = await Article.Load(name, link);
            Logger.WriteLine("Article \"" + article.Name + "\" loaded");
            await WriteArticleToFile(article);
        }
        catch (Exception ex)
        {
            Logger.WriteLine(
                $"Article {name} loading finished with exception : {ex.Message}");
        }
    }

    private static IEnumerable<string> ReadRssFeedLinks()
    {
        return File.ReadAllLines(RssFeedLinksFilePath);
    }

    private static IEnumerable<string> ReadProcessedArticleNames()
    {
        return File.ReadAllLines(ProcessedArticlesListFilePath);
    }

    private static List<(string name, string link)> FilterUnprocessedArticles(List<(string name, string link)> articles,
        List<string> processedArticleNames)
    {
        return articles.Where(article => !processedArticleNames.Contains(article.name)).ToList();
    }

    private static async Task WriteArticleToFile(Article article)
    {
        await article.WriteToFile(); 
        await using var writer = new StreamWriter(ProcessedArticlesListFilePath, append: true);
        {
            await writer.WriteLineAsync(article.Name);
            Logger.WriteLine("Article \"" + article.Name + "\" uploaded to file");
            Interlocked.Add(ref _stats.NewArticles, 1);
        }
    }
}