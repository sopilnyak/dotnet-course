using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASK3.src.NewsReader
{
    internal class RssNewsJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            //RssReader rssReader = (RssReader)dataMap["RssReader"];
            RssReader rssReader = new RssReader("resources\\rssClient\\rss_list_ru.txt", "resources\\rssClient\\processed_articles.txt", "resources\\rssClient\\htmls\\");
            rssReader.ReadRssFile();
            rssReader.ReadProcessedFile();
            rssReader.ParseRssFile();
            rssReader.ShowStatistics();

            await Console.Out.WriteLineAsync("#################################################################");
        }
    }
}
