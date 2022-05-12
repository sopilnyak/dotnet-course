using System;
using TASK3.src.NewsReader;
using System.Threading.Tasks;

namespace TASK3
{
    public class  Program
    {
        private static async Task Main(string[] args)
        {

            // TASK#3
            RssScheduler scheduler = new RssScheduler();
            RssReader rssReader = new RssReader("resources\\rssClient\\rss_list_ru.txt", "resources\\rssClient\\processed_articles.txt", "resources\\rssClient\\htmls\\");
            rssReader.ClearedHtmlContent();
            rssReader.ClearProcessedLinksFile();

            await scheduler.RssSchedulerJob();

            Console.ReadKey();
           
        }


    }
}
