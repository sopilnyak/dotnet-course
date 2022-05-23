using System.Xml.Linq;

namespace _05_multithreading.Rss;

public static class Loader
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<XDocument> LoadFeed(string url)
    {
        var rssFeedString = await HttpClient.GetStringAsync(url);
        return XDocument.Parse(rssFeedString);
    }
}