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

    internal void SaveToProcessed(IEnumerable<ArticleInfo> infos)
    {
        lock (_processedFilepath)
        {
            File.AppendAllText(_processedFilepath, string.Join(Environment.NewLine, infos.Select(info => info.Link)));
        }

        foreach (var info in infos)
        {
            Logger.LogArticleSavedToProcessed(info.Title);
        }
    }

    internal async Task SaveArticle(ArticleInfo info, string contents)
    {
        await File.WriteAllTextAsync(Path.Combine(_saveDirPath, $"{info.Title}.html"), contents);
        Logger.LogArticleSaved(info.Title);
    }
}