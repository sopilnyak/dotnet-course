namespace Multithreading.Solution;

public class RssReader
{
    private readonly FileManager _fileManager;
    private readonly Downloader _client = new();

    private bool _stopped;

    private HashSet<string> _processedArticlesLinks = null!;

    public RssReader(string rssFilepath, string processedFilepath, string saveDirPath) =>
        _fileManager = new FileManager(rssFilepath, processedFilepath, saveDirPath);
    
    private async Task ProcessArticle(ArticleInfo info)
    {
        var html = await _client.DownloadArticle(info);
        await _fileManager.SaveArticle(info, html);
    }

    private async Task ProcessRss(RssInfo rssInfo)
    {
        var xml = await _client.DownloadRss(rssInfo);
        var newArticlesInfo = RssXmlParser.GetNewArticlesInfo(rssInfo, xml, _processedArticlesLinks);
        await Task.WhenAll(newArticlesInfo.Select(ProcessArticle));
        _fileManager.SaveToProcessed(newArticlesInfo);
    }

    private void Read()
    {
        _processedArticlesLinks = _fileManager.ReadProcessedArticlesLinks();
        Task.WhenAll(_fileManager.ReadRssInfo().Select(ProcessRss)).Wait();
        Console.WriteLine("All articles have been successfully processed and saved");
    }

    public async Task Run(int nMinutes)
    {
        while (true)
        {
            if (_stopped)
            {
                break;
            }

            Read();
            await Task.Delay(TimeSpan.FromMinutes(nMinutes));
        }
    }

    public void Stop()
    {
        _stopped = true;
    }
}