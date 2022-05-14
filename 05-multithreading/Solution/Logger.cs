namespace Multithreading.Solution;

internal static class Logger
{
    internal static void LogFeedDownloaded(string feedName)
    {
        Console.WriteLine($"RSS feed \"{feedName}\" downloaded");
    }

    internal static void LogArticlesToProcess(string feedName, int newArticlesCount)
    {
        Console.WriteLine(newArticlesCount == 0
            ? $"All articles in feed \"{feedName}\" have already been downloaded"
            : $"Downloading {newArticlesCount} articles from feed \"{feedName}\"");
    }

    internal static void LogArticleDownloaded(string title)
    {
        Console.WriteLine($"Article \'{title}\' downloaded");
    }

    internal static void LogArticleSaved(string title)
    {
        Console.WriteLine($"Article \'{title}\' saved");
    }

    internal static void LogArticleSavedToProcessed(string title)
    {
        Console.WriteLine($"Article \'{title}\' registered as processed");
    }
}