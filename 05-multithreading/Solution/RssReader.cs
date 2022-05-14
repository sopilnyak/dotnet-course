namespace Multithreading.Solution;

public class RssReader
{
    private readonly FileManager _fileManager;
    private readonly Downloader _client = new();

    private bool _stopped;

    public RssReader(string rssFilepath, string processedFilepath, string saveDirPath) =>
        _fileManager = new FileManager(rssFilepath, processedFilepath, saveDirPath);

    private void Read()
    {
        var processedArticlesLinks = _fileManager.ReadProcessedArticlesLinks();

        Task.WhenAll(_fileManager.ReadRssInfo().Select(rssInfo =>
            _client.DownloadRss(rssInfo)
                .ContinueWith(task =>
                    Task.WhenAll(
                        RssXmlParser.GetNewArticlesInfo(rssInfo, task.Result, processedArticlesLinks)
                            .Select(info =>
                                _client.DownloadArticle(info)
                                    .ContinueWith(httpTask => _fileManager.SaveArticle(info, httpTask.Result))
                                    .ContinueWith(_ => _fileManager.SaveToProcessed(info))))))).Wait();
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