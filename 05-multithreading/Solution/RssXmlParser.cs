using System.Xml;

namespace Multithreading.Solution;

internal static class RssXmlParser
{
    internal static IEnumerable<ArticleInfo> GetNewArticlesInfo(RssInfo rssInfo, string xmlResponse,
        IReadOnlySet<string> processedLinks)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xmlResponse);

        var info = doc.GetElementsByTagName("item")
            .Cast<XmlElement>()
            .Select(node => new ArticleInfo
            {
                Link = node.GetElementsByTagName("link").Item(0)!.InnerXml,
                Title = node.GetElementsByTagName("title").Item(0)!.InnerXml
                    .Replace(Path.DirectorySeparatorChar, '_')
            })
            .Where(info => info.Link.EndsWith(".html") && !processedLinks.Contains(info.Link));

        Logger.LogArticlesToProcess(rssInfo.GetFeedName(), info.Count());
        return info;
    }
}