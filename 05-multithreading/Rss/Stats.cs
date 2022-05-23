namespace _05_multithreading.Rss;

public struct Stats
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

    public void LogStats(util.ILogger logger)
    {
        var logMessage = "Iteration stats :" +
                         "\n\t Feeds processed: " +FeedsProcessed +
                         "\n\t New articles: " + NewArticles +
                         "\n\t Old articles: " + OldArticles;
        logger.WriteLine(logMessage);
    }
}