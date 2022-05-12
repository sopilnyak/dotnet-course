using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TASK3.src.NewsReader
{
    public class RssReader
    {
        public String RssListPath { get; set; }
        public String ProcessedPath { get; set; }
        public String ContentSource { get; set; }


        private readonly String _projectRootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        private List<String> rssFeedsLinks;
        private HashSet<String> processedLinks = new HashSet<string>();

        private readonly String _htmlFileDir = "resources\\rssClient\\htmls\\html_";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private long processedFeeds = 0;
        private long newArticles = 0;
        private long oldArctiles = 0;

        public RssReader(string rssListPath, string processedPath, string contentSource)
        {
            RssListPath = rssListPath;
            ProcessedPath = processedPath;
            ContentSource = contentSource;

            //this.ClearProcessedLinksFile();
        }

        public void ReadRssFile()
        {
            string filePath = Path.Combine(_projectRootPath, RssListPath);
            processedFeeds = 0;
            newArticles = 0;
            oldArctiles = 0;
            rssFeedsLinks = File.ReadAllLines(filePath).ToList();
            log.Info(String.Format("Read {0}", RssListPath.Split('\\').Last()));
        }

        public void ReadProcessedFile()
        {
            string filePath = Path.Combine(_projectRootPath, ProcessedPath);
            processedLinks = File.ReadAllLines(filePath).ToHashSet();
            log.Info(String.Format("Read {0}", RssListPath.Split('\\').Last()));

        }

        public void ClearRssFeedsFile()
        {
            string filePath = Path.Combine(_projectRootPath, RssListPath);
            File.WriteAllText(filePath, string.Empty);
            log.Info(String.Format("Cleared {0}", RssListPath.Split('\\').Last()));

        }

        public void ClearProcessedLinksFile()
        {
            string filePath = Path.Combine(_projectRootPath, ProcessedPath);
            File.WriteAllText(filePath, string.Empty);
            log.Info(String.Format("Cleared {0}", ProcessedPath.Split('\\').Last()));
        }

        public void ClearedHtmlContent()
        {
            string filePath = Path.Combine(_projectRootPath, ContentSource);
            System.IO.DirectoryInfo di = new DirectoryInfo(filePath);
            
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        public void ParseRssFile()
        {
            List<String> allLinks = new List<string>();
            rssFeedsLinks.ForEach(feed => {
                allLinks.AddRange(ReadArticleLinks(feed));
                log.Info(String.Format("Parsed feed {0}", feed));
            });
            log.Info(String.Format("Finsihed parsing {0}", RssListPath.Split('\\').Last()));
        }

        private void SaveHtmlOnDisk(String res)
        {
            string path = Path.Combine(_projectRootPath, _htmlFileDir + DateTime.Now.ToFileTime() + ".html");

            try
            {
                // Create the file, or overwrite if the file exists.
                using (FileStream fs = File.Create(path))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(res);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private String TakeHtmlFromLink(String link)
        {
            String res;
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(link);
                myRequest.Timeout = 10_000;
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                res = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "";
        }

        private List<String> ReadArticleLinks(String feedURL)
        {
            List<String> articleLinks = new List<string>();

            XmlDocument rssXmlDoc = new XmlDocument();
            try
            {
                rssXmlDoc.Load(feedURL);
                processedFeeds++;
                log.Info(String.Format("Loaded feed {0}", feedURL));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            // Parse the Items in the RSS file
            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");

            StringBuilder rssContent = new StringBuilder();
            int i = 0;

            foreach (XmlNode rssNode in rssNodes)
            {
                XmlNode rssSubNode = rssNode.SelectSingleNode("link");
                string link = rssSubNode != null ? rssSubNode.InnerText : "";
                articleLinks.Add(link);

                // Add to processed set
                if (!processedLinks.Add(link))
                {
                    oldArctiles++;
                    log.Info(String.Format("Duplicate news article URL {0}", link));
                }
                else
                {
                    newArticles++;
                    string filePath = Path.Combine(_projectRootPath, ProcessedPath);

                    StreamWriter writer = new StreamWriter(filePath, true);
                    writer.Write(link + '\n');
                    writer.Close();
                    log.Info(String.Format("Added news article URL {0}", link));

                    String parsedHtml = this.TakeHtmlFromLink(link);

                    log.Info(String.Format("Downloaded news article html at URL {0}", link));


                    if (!parsedHtml.Equals(""))
                    {
                        SaveHtmlOnDisk(parsedHtml);
                        log.Info(String.Format("Saved on disk news article html at URL {0}", link));
                    } else
                    {
                        log.Info(String.Format("Ignored empty news article html at URL {0}", link));

                    }

                }

            }
            return articleLinks;

        }

        public void ShowStatistics()
        {
            Console.WriteLine("#################################################################");
            // nr proccessed feeds
            log.Info(String.Format("Processed Feeds: {0}", processedFeeds));
            // nr new articles
            log.Info(String.Format("New articles processed: {0}", newArticles));
            // nr old articles
            log.Info(String.Format("Old articles processed: {0}", oldArctiles));
            Console.WriteLine("#################################################################");
        }


    }
}
