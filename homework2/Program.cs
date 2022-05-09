/*
    1. Подготовка
    Найти десяток новостных порталов с RSS лентой, записать их в файл.

    2. Задание
    Раз в N минут нужно читать файл со списком адресов RSS лент (rss_list.txt), а так же файл со списоком уже обработанных ссылок (processed_articles.txt).
    Каждую RSS-ленту нужно скачать, распарсить, достать из неё список ссылок на новостные статьи.
    По тем ссылкам, которые ещё не было ранее обработаны, нужно скачать html содержимое.
    Содежимое нужно сохранить на диск, а так же записать в файл processed_articles.txt информацию о том что ссылка была обработана.

    3. Логи
    В процессе работы нужно писать логи в консоль: лента обработана, статья скачена и т.д. 
    После каждой итерации нужно выводить статистику: сколько лент обработано, сколько новых новостей, сколько старых.


    10 баллов
    Мягкий дедлайн: 31.03.2022 23:59
    Жесткий дедлайн: 12.05.2022 23:59
*/
using System.ServiceModel.Syndication;
using System.Xml;
using Logger;
using System.Collections.Concurrent;


namespace Program
{
    enum SavingStatus {
        Saved,
        AlreadySaved,
    }
    class ContentSaver
    {
        SemaphoreSlim semaphoreSlim;
        private ConcurrentDictionary<string, bool> saved;
        private StreamWriter writer;
        private string dir;
        private ILogger log;

        private static HttpClient client = new HttpClient();   // actually only one object should be created by Application

        public ContentSaver(string save_file, string pages_dir, ILogger log)
        {
            if (!Directory.Exists(pages_dir))
            {
                Directory.CreateDirectory(pages_dir);
            }
            this.dir = pages_dir;
            this.semaphoreSlim = new SemaphoreSlim(1, 1);

            this.saved = new ConcurrentDictionary<string, bool>();
            this.log = log;
            this.writer = new StreamWriter(save_file, true);

            using (StreamReader sr = new StreamReader(save_file))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    this.saved.TryAdd(line, true);
                }
            }

        }

        async public Task<SavingStatus> Save(string url)
        {
            if (this.saved.TryAdd(url, true))
            {

                await this.SavePage(url);

                await this.semaphoreSlim.WaitAsync();
                try
                {
                    await writer.WriteLineAsync(url);
                    await writer.FlushAsync();
                }
                finally
                {
                    this.semaphoreSlim.Release();
                }
                await this.log.LogAsync(String.Format("Page {0} saved", url));
                return SavingStatus.Saved;
            }
            return SavingStatus.AlreadySaved;
        }

        async public Task SavePage(string url)
        {
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                using (HttpContent content = response.Content)
                {
                    string result = await content.ReadAsStringAsync();

                    var writer = new StreamWriter(String.Format("{0}/{1}.html", this.dir, url.Replace("/", "_")), true);
                    await writer.WriteAsync(result);
                    await writer.FlushAsync();
                    writer.Close();
                }
            }
        }
    }

    class RssList
    {
        private ConcurrentBag<string> urls;
        public RssList(string file)
        {
            this.urls = new ConcurrentBag<string>();
            using (StreamReader sr = new StreamReader(file))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    this.urls.Add(line);
                }
            }
        }

        public string[] ToArray()
        {
            return this.urls.ToArray();
        }
    }

    class FeedProcesser
    {
        private ContentSaver saver;
        private ILogger log;
        public FeedProcesser(ContentSaver saver, ILogger log)
        {
            this.saver = saver;
            this.log = log;
        }

        async public Task Process(string url)
        {

            var feed = await Task.Run(() =>
            {
                using (XmlReader r = XmlReader.Create(url))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(r);
                    return feed;
                }
            });

            List<Task> tasks = new List<Task>();
            await log.LogAsync(String.Format("RSS Feed {0} loaded", url));
            int total_count = 0;
            int skipped = 0;

            foreach (SyndicationItem item in feed.Items)
            {
                total_count += 1;
                tasks.Add(this.saver.Save(item.Links.First().Uri.ToString()).ContinueWith(t => {
                    if (t.Result == SavingStatus.AlreadySaved) {
                        skipped += 1;
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion));
            }
            Task.WaitAll(tasks.ToArray());

            await log.LogAsync(String.Format("Total {0} pages, {1} new pages in {2}", total_count, total_count - skipped, url));
        }
    }

    class Program
    {
        static readonly string rssListFile = "rss_list.txt";
        static readonly string processedArticlesFile = "processed_articles.txt";

        static readonly string pagesDir = "pages";

        static readonly int update_delay = 10000;
        public static void Iteration()
        {
            ConsoleLogger logger = new ConsoleLogger();
            logger.Log("Starting iteration");

            var content_saver = new ContentSaver(Program.processedArticlesFile, Program.pagesDir, logger);
            var feed_processer = new FeedProcesser(content_saver, logger);
            var rss_list = new RssList(Program.rssListFile);

            var tasks = new List<Task>();
            foreach (string url in rss_list.ToArray())
            {
                tasks.Add(feed_processer.Process(url));
            }
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                logger.Log(String.Format("Unexpected fault :( \n {0}", e.Message));
            }
            logger.Log("Iteration finished");
        }

        public static void Main()
        {
            while (true)
            {
                Program.Iteration();
                Thread.Sleep(Program.update_delay);
            }

        }
    }
}