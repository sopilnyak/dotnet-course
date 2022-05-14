namespace Multithreading;

using Solution;

public static class Program
{
    public static async Task Main()
    {
        var reader = new RssReader("../../../Solution/rss_list.txt", "../../../Solution/processed_articles.txt", "../../../Solution/news");
        await reader.Run(1);
    }
}
