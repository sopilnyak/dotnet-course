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

using System.Collections.Concurrent;
using System.ServiceModel.Syndication;
using System.Timers;

using System.Xml;
public class RSSWorker
{
    public RSSWorker(string sources_file, string articles_file)
    {
        if (!Directory.Exists(articles_folder))) {
            Directory.CreateDirectory(articles_folder);
        }

        sources = File.ReadLines(sources_file).Select(source => new Uri(source));
        this.articles_file = articles_file;
        written_articles = new ConcurrentDictionary<Uri, byte>();
        processed_articles = new ConcurrentDictionary<Uri, byte>();
        var articles = File.ReadLines(articles_file);
        foreach (var article in articles)
        {
            written_articles[new Uri(article)] = 0;
        }
    }


    public async Task start()
    {
        Console.WriteLine("Processing sources");
        await Task.WhenAll(
            this.sources
            .Select(source => process(source)
        ));
    }

    public void stop()
    {
        Console.WriteLine("New articles: " + processed_articles.Count().ToString());
        Console.WriteLine("Total articles: " + (written_articles.Count + processed_articles.Count).ToString());
        write();
    }

    private void write()
    {
        using (var file = File.AppendText(articles_file))
        {
            var removed_articles = new ConcurrentDictionary<Uri, byte>();
            foreach (var content in processed_articles)
            {
                file.WriteLine(content.Key);
                written_articles[content.Key] = content.Value;
            }
            processed_articles.Clear();
        }
    }

    private async Task<string> get(Uri link)
    {
        try
        {
            Console.WriteLine("Proccessing " + link.ToString());
            var result = await new HttpClient().GetStringAsync(link);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while getting content from " + link);
            return "";
        }
    }

    private async Task writeArticle(SyndicationItem item)
    {
        var link = item.Links[0].Uri;
        if (!processed_articles.ContainsKey(link) && !written_articles.ContainsKey(link))
        {
            processed_articles.TryAdd(link, 0);
            var content = await get(link);
            if (content == "") return;
            Console.WriteLine("Got data from " + link);
            var link_str = link.ToString();
            foreach(var sym in ":/\\*?\"|")
            {
                link_str = link_str.Replace(sym, '_');
            }
            var filename = articles_folder + link_str + ".html";

            await File.WriteAllTextAsync(filename, content);
        }
    }

    private async Task process(Uri url)
    {
        Console.WriteLine("Processing " + url);
        SyndicationFeed feed;
        try
        {
            feed = SyndicationFeed.Load(XmlReader.Create(url.ToString()));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while processing " + url);
            return;
        }
        Console.WriteLine("Processing " + url + " finished successfully");

        await Task.WhenAll(feed.Items.Select(item => writeArticle(item)));
    }

    private string articles_folder = "articles/";
    private IEnumerable<Uri> sources;
    private ConcurrentDictionary<Uri, byte> written_articles;
    private ConcurrentDictionary<Uri, byte> processed_articles;
    private string articles_file;
} 
 
class Program {
    public static async Task Main(string[] args)
    {
        var N = Int32.Parse(args[0]);
        var worker = new RSSWorker("rss_list.txt", "processed_articles.txt");

        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            keepRunning = false;
            Console.WriteLine("Ctrl+C handled, wait for shutting down");
        };

        while (keepRunning)
        {
            setTimer(N);
            await worker.start();
            while (!timer_end) System.Threading.Thread.Sleep(0);
            worker.stop();
        }
    }


    private static bool keepRunning = true;
    private static System.Timers.Timer aTimer;
    private static bool timer_end = false;
    private static void setTimer(int minutes)
    {
        timer_end = false;
        aTimer = new System.Timers.Timer(minutes * 60 * 1000);
        aTimer.Elapsed += onTimedEvent;
        aTimer.AutoReset = false;
        aTimer.Enabled = true;
    }

    private static void onTimedEvent(Object source, ElapsedEventArgs e)
    {
        timer_end = true;
    }
}