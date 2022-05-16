using System.Xml.Linq;

namespace _05_multithreading.Rss;

public static class Processor
{
    public static List<(string name, string link)> GetArticles(XDocument feed)
    {
        List<(string name, string link)> articles = new();
        foreach (var item in feed.Descendants("item"))
        {
            if (!(item.Elements("title").Any() && item.Elements("link").Any())) continue;

            articles.Add((item.Element("title")!.Value, item.Element("link")!.Value));
        }

        return articles;
    }
}