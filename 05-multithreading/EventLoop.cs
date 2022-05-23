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
            InvokeLoadingArticles().Wait();
        }
        catch (Exception ex)
        {
            Logger.WriteLine("Caught iteration exception with message \"" + ex.Message + "\"");
        }
    }

    private static async Task InvokeLoadingArticles()
    {
        _stats.Clear();
        Logger.WriteLine("Iteration started");
        
        var processedArticlesNames = ReadProcessedArticleNames();
        var rssFeedLinks = ReadRssFeedLinks();
        Logger.WriteLine("Load rss links from file");
        
        foreach (var url in rssFeedLinks)
        {
            try
            {
                var feed = await Rss.Loader.LoadFeed(url);
                Logger.WriteLine($"Feed {url} loaded");

                var feedArticles = Rss.Processor.GetArticles(feed);
                var unprocessedArticles = FilterUnprocessedArticles(feedArticles, processedArticlesNames);
                _stats.OldArticles += feedArticles.Count - unprocessedArticles.Count;

                //int articlesAffectedInThisIter = 0;
                foreach (var articleToLoad in unprocessedArticles)
                {
                    try
                    {
                        //if (++articlesAffectedInThisIter == ArticlePerIterationLimit) break;
                        var article = await Article.Load(articleToLoad.name, articleToLoad.link);
                        Logger.WriteLine("Article \"" + article.Name + "\" loaded");
                        await WriteArticleToFile(article);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(
                            $"Article {articleToLoad.name} loading finished with exception : {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(
                    $"Feed {url} loading finished with exception : {ex.Message}");

            }
            
            _stats.FeedsProcessed++;
            Logger.WriteLine($"Feed {url} processed");
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

    private static async Task WriteArticleToFile(Article article)
    {
        await article.WriteToFile(); 
        await using var writer = new StreamWriter(ProcessedArticlesListFilePath, append: true);
        {
            await writer.WriteLineAsync(article.Name);
            Logger.WriteLine("Article \"" + article.Name + "\" uploaded to file");
            _stats.NewArticles++;
        }
    }
}