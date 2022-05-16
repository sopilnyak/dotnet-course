using System.Xml.Linq;

namespace _05_multithreading.Rss;

public static class Loader
{
    private static readonly HttpClient HttpClient = new();

    public static List<XDocument> LoadFeeds(List<string> links)
    {
        List<Task<XDocument>> loadTasks = new(links.Count);
        loadTasks.AddRange(links.Select(LoadFeed));

        Task allLoaded = Task.WhenAll(loadTasks);
        allLoaded.Wait();

        return loadTasks.Select(task => task.Result).ToList();
    }

    private static async Task<XDocument> LoadFeed(string url)
    {
        var rssFeedString = await HttpClient.GetStringAsync(url);
        return XDocument.Parse(rssFeedString);
    }
}