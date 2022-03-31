/*
    1. Подготовка - done
    Найти десяток новостных порталов с RSS лентой, записать их в файл. - done

    2. Задание - done
    Раз в N минут нужно читать файл со списком адресов RSS лент (rss_list.txt), а так же файл со списоком уже обработанных ссылок (processed_articles.txt). - done
    Каждую RSS-ленту нужно скачать, распарсить, достать из неё список ссылок на новостные статьи. - done
    По тем ссылкам, которые ещё не было ранее обработаны, нужно скачать html содержимое. - done
    Содежимое нужно сохранить на диск, а так же записать в файл processed_articles.txt информацию о том что ссылка была обработана. - done

    3. Логи - done
    В процессе работы нужно писать логи в консоль: лента обработана, статья скачена и т.д.  - done

    После каждой итерации нужно выводить статистику: сколько лент обработано, сколько новых новостей, сколько старых. - done


    10 баллов
    Мягкий дедлайн: 31.03.2022 23:59
    Жесткий дедлайн: 12.05.2022 23:59
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using System.Globalization;
using System.Threading;

namespace news_portal {
    class RSS_Reader {
        public List<string> Read(string url) {
            List<string> links = new List<string>();
            WebRequest request = WebRequest.Create(url);
 
            WebResponse response = request.GetResponse();
            XmlDocument doc = new XmlDocument();
            try {
                doc.Load(response.GetResponseStream());
                XmlElement rssElem = doc["rss"];
                if (rssElem == null) {
                    return links;
                }
                XmlElement chanElem = rssElem["channel"];
                XmlNodeList itemElems = rssElem["channel"].GetElementsByTagName("item");
                if (chanElem != null) {
                    foreach (XmlElement itemElem in itemElems) {
                        links.Add(itemElem["link"].InnerText);                   
                    }
                }
            } catch (XmlException) {}
            
            return links;
        }
    }

    class Observer {
        private object _lock = new object();
        private int sleep_time;
        private FileLogger processed_article_writer = new FileLogger("processed_articles.txt");
        private FileLogger worker_logger = new FileLogger("logs.txt");
        private RSS_Reader rss_reader;

        private int old_articles = 0;
        private int rss_lents_processed = 0;
        private int new_articles = 0;
        public Observer(int sleep_time = 1) {
            this.sleep_time = sleep_time * 1000 * 60;
            this.rss_reader = new RSS_Reader();
        }
        public void Observe() {
            while (true) {
                new_articles = 0;
                old_articles = 0;
                rss_lents_processed = 0;
                UpdateNews();
                worker_logger.Logging($"За текущую итерацию получено {new_articles} новых статей и {old_articles} старых статей. Всего обработано - {new_articles + old_articles} статей");
                worker_logger.Logging($"За текущую итерацию обработано {rss_lents_processed} rss лент");
                Thread.Sleep(this.sleep_time);
            }
        }
        public void UpdateNews() {
            List<Task<int>> tasks = new List<Task<int>>();
            using (StreamReader reader = new StreamReader("rss_list.txt")) {
                while (reader.Peek() >= 0) {
                    string str = reader.ReadLine();
                    if (str == null || str.Length == 0) {
                        break;
                    }
                    Task<int> task = new Task<int>(() => MethodForThread(str));
                    task.Start();
                    tasks.Add(task);
                }
            }
            Task.WaitAll(tasks.ToArray());
        }
        int MethodForThread(string url) {
            worker_logger.Logging("RSS лента " + url + " взята в обработку!");
            List<string> links = rss_reader.Read(url);
            foreach (string link in links) {
                // save info about article
                WebRequest request = WebRequest.Create(link);
                WebResponse response = request.GetResponse();
                
                // update article info
                    using (StreamReader reader = new StreamReader("processed_articles.txt")) {
                        bool is_processed = false;
                        Monitor.Enter(_lock);
                        while (reader.Peek() >= 0) {
                            if (reader.ReadLine() == link) {
                                is_processed = true;
                                break;
                            }
                        }
                        try {
                            if (!is_processed) {
                                    new_articles++;
                                    worker_logger.Logging("Появилась новая статья : " + link);
                                    processed_article_writer.Logging(link);
                                    ProcessUnprocessedArticle(link);
                                    worker_logger.Logging("Статья " + link + " скачана!");
                            } else {
                                old_articles++;
                            }
                        } finally {
                            Monitor.Exit(_lock);
                        }
                        
                    }
            }
            rss_lents_processed++;
            worker_logger.Logging("RSS лента " + url + " обработана!");
            return 0;
        }
        async void ProcessUnprocessedArticle(string link) {
            // process unprocessed_article
            Uri uri = new Uri(link);
            string path = $"./articles/{uri.Host}";
            
            // create directory
            if (!Directory.Exists(path)) {
                try {
                    Directory.CreateDirectory(path);
                } catch(Exception exc) {
                    worker_logger.Logging($"Перехвачено исключение: {exc.Message}");
                } 
            }
            
            // download
            try {
                using (WebClient web_client = new WebClient()) {
                    string filename = link.Replace('/', '_') + ".html";
                    await web_client.DownloadFileTaskAsync(link, path + "/" + filename);
                }
            } catch(Exception exc) {
                worker_logger.Logging($"Перехвачено исключение: {exc.Message}");
            } 
            
        } 
   }
   class Program {

        public static void Main (string[] args) {
            Observer observer = new Observer(2);
            observer.Observe();
        }
    }
}
