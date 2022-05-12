using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Homework2
{
    public static class Statistics {
        public static int linkCount = 0;
        public static int newArticles = 0;
        public static int oldArticles = 0;
    }
    class FileReader {
         public List<string> ReadUrl(string url) {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            XmlDocument xml = new XmlDocument();
            xml.Load(response.GetResponseStream());
            XmlNodeList xmlList = xml["rss"]["channel"].GetElementsByTagName("item");
            var listOfArticles = new List<string>();
            foreach (XmlNode i in xmlList)
            {
                if (i["link"].InnerText.Length > 0)
                {
                    listOfArticles.Add(i["link"].InnerText);
                }
            }
            return listOfArticles;
        }
    }

    class RSSParser {
        private object _syncObject = new object();
        private object _objStart = new object();
        private object _objEnd = new object();
        public string rssAddresses;
        public string processed_articles;
        public int sleepTime;
        FileReader fileReader;
        public RSSParser(string rssAddresses, string processed_articles, int sleepTime) {
            this.rssAddresses = rssAddresses;
            this.processed_articles = processed_articles;
            this.sleepTime = sleepTime * 60 * 1000; //per minute in seconds
            fileReader = new FileReader();
            while(true) {
                Statistics.linkCount = 0;
                Statistics.newArticles = 0;
                Statistics.oldArticles = 0;
                Parse();
                using(var writer = new StreamWriter("logs.txt", true)) {
                    writer.WriteLine($"Statistics: linkCount  = {Statistics.linkCount}");
                    writer.WriteLine($"Statistics: NewArticles  = {Statistics.newArticles}");
                    writer.WriteLine($"Statistics: OldArticles  = {Statistics.oldArticles}");
                }
                Thread.Sleep(this.sleepTime);
            }
        }

        private void Parse() {

            int lineCount = File.ReadAllLines(rssAddresses).Length;
            Task[] arr = new Task[lineCount];
            int i = 0;
            using(StreamReader reader = new StreamReader(rssAddresses)) {
                while (reader.Peek() >= 0) {
                    string line = reader.ReadLine();
                    if (line == null || line.Length == 0)
                    {
                        break;
                    }
                    Task task = Task.Run(() => GenereateLinks(line));
                    arr[i++] = task;
                }  
            }
            Task.WaitAll(arr); 
        }
        void GenereateLinks(string url) {
            
            using (StreamWriter writer = new StreamWriter("logs.txt", true))
            {
                lock(_objStart){
                writer.WriteLine($"RSS feed {url} started parsing");
                }
                var links = fileReader.ReadUrl(url);
                foreach(string link in links) 
                {
                    Monitor.Enter(_syncObject);
                    if(!Is_processed(link)) {
                        Statistics.newArticles++;
                        writer.WriteLine($"New Article => {link}");
                        using(var process_writer = new StreamWriter(processed_articles, true)) {
                            process_writer.WriteLine(link);
                        }
                        GetArticleHTML(link);
                        writer.WriteLine($"Article => {link} is added");
                    }
                    else {
                        Statistics.oldArticles++;
                    }
                    Monitor.Exit(_syncObject);
                }
                lock(_objEnd){
                    Statistics.linkCount++;
                    writer.WriteLine($"|| {url} is processed");
                }
            }  
        }
////////////////////////////
    void GetArticleHTML(string url)
        {
            string path = "./ArticlesHTML";

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
	    using (var webClient = new WebClient())
	    {
                string filename = url.Replace('/', '_') + ".html";
		 webClient.DownloadFileTaskAsync(url, path + "/" + filename);
	    }

       }
//////////////////////////////////////

        private bool Is_processed(string link) {
            string processed_line;
            using (StreamReader reader = new StreamReader(processed_articles)) {
                while((processed_line = reader.ReadLine()) != null && processed_line.Length != 0) {
                    if(processed_line == link) {
                        return true;           
                    }
                }
            }
            return false;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            const int N = 3;
            RSSParser parser = new RSSParser("rss_list.txt", "processed_articles.txt", N);

        }
    }
}
