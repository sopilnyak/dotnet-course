using System.Xml;

namespace Multithreading.Solution;

public class RssReader
{
    private class ArticleInfo
    {
        public string Link { get; init; } = null!;
        public string Title { get; init; } = null!;
    }

    private readonly string _rssFilepath;
    private readonly string _processedFilepath;
    private readonly string _saveDirPath;
    private readonly HttpClient _client = new();

    private bool _stopped;

    public RssReader(string rssFilepath, string processedFilepath, string saveDirPath)
    {
        File.Create(processedFilepath).Dispose();
        Directory.CreateDirectory(saveDirPath);

        _rssFilepath = rssFilepath;
        _processedFilepath = processedFilepath;
        _saveDirPath = saveDirPath;

        _client.DefaultRequestHeaders.Add(
            "cookie", /*add your cookie from browser here, because otherwise 403 is returned*/"");
    }

    private IEnumerable<string> ReadRssLinks() => File.ReadAllLines(_rssFilepath);

    private IEnumerable<string> ReadProcessedArticlesLinks() => File.ReadAllLines(_processedFilepath);

    private IEnumerable<string> FetchXlmResponses(IEnumerable<string> rssLinks)
    {
        var fetchTasks = rssLinks.Select(link => _client.GetStringAsync(link)
            .ContinueWith(
                task =>
                {
                    Console.WriteLine($"RSS feed {link.Split('/').Last().Split('.').First()} downloaded");
                    return task.Result;
                }));
        Task.WhenAll(fetchTasks).Wait();
        return fetchTasks.Select(task => task.Result);
    }

    private static IEnumerable<ArticleInfo> GetArticlesInfo(IEnumerable<string> xmlResponses)
    {
        var info = new List<ArticleInfo>();
        foreach (var xmlResponse in xmlResponses)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlResponse);

            info.AddRange(doc.GetElementsByTagName("item")
                .Cast<XmlElement>()
                .Select(node => new ArticleInfo
                {
                    Link = node.GetElementsByTagName("link").Item(0)!.InnerXml,
                    Title = node.GetElementsByTagName("title").Item(0)!.InnerXml
                        .Replace(Path.DirectorySeparatorChar, '_')
                })
                .Where(currInfo => currInfo.Link.EndsWith(".html")));
        }

        return info;
    }

    private static IEnumerable<ArticleInfo> FilterOutProcessed(IEnumerable<ArticleInfo> all,
        IEnumerable<string> processedLinks)
    {
        return all.Where(info => processedLinks.All(link => link != info.Link));
    }

    private void SaveArticles(IReadOnlyList<ArticleInfo> articlesInfo)
    {
        Console.WriteLine(articlesInfo.Count == 0
            ? "All articles have already been downloaded"
            : $"Downloading {articlesInfo.Count} articles");
        var saveTasks = articlesInfo
            .Select((info, index) =>
                _client.GetStringAsync(info.Link)
                    .ContinueWith(httpTask =>
                        {
                            var title = articlesInfo[index].Title;
                            Console.WriteLine($"Article \'{title}\' downloaded");
                            File.WriteAllTextAsync(
                                Path.Combine(_saveDirPath, $"{articlesInfo[index].Title}.html"),
                                httpTask.Result
                            ).ContinueWith(
                                writeTask =>
                                {
                                    Console.WriteLine($"Article \'{title}\' saved");
                                    return writeTask;
                                }
                            );
                        }
                    ));
        Task.WhenAll(saveTasks).Wait();
        Console.WriteLine("All articles have been successfully processed and saved");
    }

    private void SaveToProcessed(IEnumerable<ArticleInfo> articlesInfo)
    {
        File.AppendAllText($"{_processedFilepath}", string.Join("\n", articlesInfo.Select(info => info.Link)));
    }

    private void Read()
    {
        var rssLinks = ReadRssLinks();
        var xmlResponses = FetchXlmResponses(rssLinks);
        var allArticlesInfo = GetArticlesInfo(xmlResponses);
        var processedArticlesLinks = ReadProcessedArticlesLinks();
        var newArticlesInfo = FilterOutProcessed(allArticlesInfo, processedArticlesLinks);
        SaveArticles(newArticlesInfo.ToList());
        SaveToProcessed(newArticlesInfo);
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