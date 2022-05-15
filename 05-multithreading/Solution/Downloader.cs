namespace Multithreading.Solution;

internal class Downloader
{
    private readonly HttpClient _client = new();

    internal Downloader()
    {
        _client.DefaultRequestHeaders.Add(
            "cookie", /*add your cookie from browser here, because otherwise 403 is returned*/"");
    }

    internal async Task<string> DownloadRss(RssInfo info)
    {
        var xml = await _client.GetStringAsync(info.Link);
        Logger.LogFeedDownloaded(info.GetFeedName());
        return xml;
    }

    internal async Task<string> DownloadArticle(ArticleInfo articleInfo)
    {
        var html = await _client.GetStringAsync(articleInfo.Link);
        Logger.LogArticleDownloaded(articleInfo.Title);
        return html;
    }
}