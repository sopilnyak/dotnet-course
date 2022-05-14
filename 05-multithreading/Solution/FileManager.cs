namespace Multithreading.Solution;

internal class FileManager
{
    private readonly string _rssFilepath;
    private readonly string _processedFilepath;
    private readonly string _saveDirPath;

    internal FileManager(string rssFilepath, string processedFilepath, string saveDirPath)
    {
        File.Create(processedFilepath).Dispose();
        Directory.CreateDirectory(saveDirPath);

        _rssFilepath = rssFilepath;
        _processedFilepath = processedFilepath;
        _saveDirPath = saveDirPath;
    }

    internal IEnumerable<RssInfo> ReadRssInfo() =>
        File.ReadAllLines(_rssFilepath).Select(link => new RssInfo {Link = link});

    internal HashSet<string> ReadProcessedArticlesLinks() => File.ReadAllLines(_processedFilepath).ToHashSet();

    internal void SaveToProcessed(ArticleInfo info)
    {
        lock (_processedFilepath)
        {
            File.AppendAllText(_processedFilepath, info.Link + Environment.NewLine);
        }

        Logger.LogArticleSavedToProcessed(info.Title);
    }

    internal Task SaveArticle(ArticleInfo info, string contents)
    {
        return File.WriteAllTextAsync(Path.Combine(_saveDirPath, $"{info.Title}.html"), contents)
            .ContinueWith(_ => Logger.LogArticleSaved(info.Title));
    }
}