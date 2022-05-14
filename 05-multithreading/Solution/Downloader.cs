namespace Multithreading.Solution;

internal class Downloader
{
    private readonly HttpClient _client = new();

    internal Downloader()
    {
        _client.DefaultRequestHeaders.Add(
            "cookie", /*add your cookie from browser here, because otherwise 403 is returned*/"");
    }

    internal Task<string> DownloadRss(RssInfo info)
    {
        return _client.GetStringAsync(info.Link)
            .ContinueWith(httpTask =>
                {
                    Logger.LogFeedDownloaded(info.GetFeedName());
                    return httpTask.Result;
                }
            );
    }

    internal Task<string> DownloadArticle(ArticleInfo articleInfo)
    {
        return _client.GetStringAsync(articleInfo.Link)
            .ContinueWith(httpTask =>
                {
                    Logger.LogArticleDownloaded(articleInfo.Title);
                    return httpTask.Result;
                }
            );
    }
}