using util;

namespace _05_multithreading;

public static class EventLoop
{
    private static readonly ConsoleLogger Logger = new();
    private static Rss.Stats _stats = new();
    private static Timer _timer = null!;
    private const int PeriodMs = 60000;
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

        var rssFeedLinks = ReadRssFeedLinks();
        var rssFeeds = Rss.Loader.LoadFeeds(rssFeedLinks);
        Logger.WriteLine("Feeds loaded");
        var processedArticlesNames = ReadProcessedArticleNames();
        foreach (var feed in rssFeeds)
        {
            var feedArticles = Rss.Processor.GetArticles(feed);
            var unprocessedArticles = FilterUnprocessedArticles(feedArticles, processedArticlesNames);
            var loadedArticles = LoadArticles(unprocessedArticles);
            WriteArticlesToFile(loadedArticles);

            _stats.FeedsProcessed++;
            _stats.NewArticles += unprocessedArticles.Count;
            _stats.OldArticles += feedArticles.Count - unprocessedArticles.Count;
            Logger.WriteLine("Feed processed");
        }

        _stats.LogStats(Logger);
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
}