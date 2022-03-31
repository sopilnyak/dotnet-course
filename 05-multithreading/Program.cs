using System.Xml;
using System.ServiceModel.Syndication;
using System.Text;
using System.Collections.Concurrent;

public interface Ilog {
    void Log(string m);
}

public class ConsoleLog: Ilog {
    public ConsoleLog() {}
    public void Log(string m) {
        Console.WriteLine(m);
    }
}
public class RSSProcessor {
    private IEnumerable<string> feeds;
    private ConcurrentDictionary<Uri, Uri> processed_links;
    private Ilog logger = new ConsoleLog();
    private string content_path;
    private string base_path;
    public RSSProcessor(string content_path, string feed_path, string base_path = "./news") {
        this.feeds = File.ReadLines(feed_path);
        this.content_path = content_path;
        this.base_path = base_path;
        this.processed_links = new ConcurrentDictionary<Uri, Uri>(File.ReadLines(content_path).Select(item => new KeyValuePair<Uri, Uri>(new Uri(item), new Uri(item))));
    }

    public async Task start(int N, int iterations) {
        logger.Log("Rss processor has started");
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(N));
        int downloaded = 0;
        int current_count = processed_links.Count;
        for (int i = 0; i < iterations; i++) {
            await Task.WhenAll(this.feeds.Select(feed => worker(feed, base_path)));
            
            downloaded = processed_links.Count - current_count;
            current_count = processed_links.Count;
            logger.Log($"Polling complete, waiting for the next round\n downloads on this epoch = {downloaded} \n total articles in the bank = {current_count} ");
            flush();
            await timer.WaitForNextTickAsync();
        }
    }

    public void flush() {
        using (StreamWriter writer = File.CreateText(content_path)) {
            foreach (var content in this.processed_links.AsEnumerable()) {
                writer.Write(content.Key.ToString() + "\n");
            }
        }
        
    }

    private async Task<string> GetContent(Uri link) {
        using (HttpClient client = new HttpClient()) {
            logger.Log($"downloading {link}");
            return await client.GetStringAsync(link);
        }
    }

    private async Task ProcessContent(Uri link, string base_path, string name) {
         if (!processed_links.ContainsKey(link)) {
            string content;
            try {
                content = await GetContent(link);
            } catch (HttpRequestException e) {
                logger.Log($"Exception while trying to download {link}.\n Exception: {e.ToString()}");
                return;
            }
            
            logger.Log($"Got file from {link}");
            try {
                using (StreamWriter writer = File.CreateText(base_path + "/" + link.AbsolutePath))
                {
                    await writer.WriteAsync(content);
                }
            } catch (System.IO.DirectoryNotFoundException) {
                Directory.CreateDirectory(base_path);
                using (StreamWriter writer = File.CreateText(base_path + "/" + name))
                {
                    await writer.WriteAsync(content);
                }
            } catch (Exception e) {
                logger.Log($"Unexpected error! \n Exception: {e.ToString()}");
                return;
            }

            processed_links.TryAdd(link, link);
        }
    }

    private async Task worker(string url, string base_path) {
        logger.Log($"working on {url}");
        SyndicationFeed feed = new SyndicationFeed();
        try {
            XmlReader reader = XmlReader.Create(url);
            feed = SyndicationFeed.Load(reader);
        } catch (Exception e) {
            logger.Log($"Exception while trying to get RSS feed for {url}.\n Exception: {e.ToString()}");
            return;
        }
        logger.Log($"feed data for {url} acquired");

        await Task.WhenAll(feed.Items.Select(item =>
            ProcessContent(item.Links[0].Uri, base_path + "/" + feed.Title.Text, item.Title.Text.Replace("/", " "))));
    }

}


class Program {
    public static async Task Main (string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Please specify path to cached content links and cached feeds");
            return;
        } else if (args.Length == 1) {
            Console.WriteLine("Please specify path to cached feeds");
            return;
        }
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var processor = new RSSProcessor(args[0], args[1]);
        await processor.start(1, 10);
        processor.flush();
    }
}
