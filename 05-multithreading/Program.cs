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
//using System.Linq;
//using System.Text.RegularExpressions;
using System;
using System.Xml;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.Collections.Concurrent;

class ProcessedPages {
    private ConcurrentDictionary<string, bool> processed_articles;
    private SemaphoreSlim guard;

    StreamWriter writer;

    public ProcessedPages(string file) {
        this.guard = new SemaphoreSlim(1, 1);
        this.processed_articles = new ConcurrentDictionary<string, bool>();
        this.writer = new StreamWriter(file, true);

        StreamReader reader = new StreamReader(file);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            this.processed_articles.TryAdd(line, true);
        }
    }

    async public Task<bool> Update(string url) {
        if (this.processed_articles.TryAdd(url, true)) {
            await this.guard.WaitAsync();
            try
            {
                await this.writer.WriteLineAsync(url);
                await this.writer.FlushAsync();
            }
            finally
            {
                this.guard.Release();
            }
            return true;
        }
        return false;
    }
}


class Program
{
    static private readonly string rss_list = "rss_list.txt";
    static private readonly string processed_file = "processed_articles.txt";
    static private readonly string news_folder = "news";

    static int total_count;
    static int downloaded;
    static private ProcessedPages processed = new ProcessedPages(Program.processed_file);
    public static void Main()
    {
        if (!Directory.Exists(Program.news_folder))
        {
            Directory.CreateDirectory(Program.news_folder);
        }

        while(true) {
            Program.total_count = 0;
            Program.downloaded = 0;
            Task.Run(() => Program.UpdateAll()).Wait();
            Console.WriteLine($"total pages: {Program.total_count}, downloaded now: {Program.downloaded}, previously downloaded: {Program.total_count - Program.downloaded}");
            Console.WriteLine("Iteratoin fineshed, sleep for 5s");
            Console.WriteLine("");
            Thread.Sleep(5000);

        }
    }


    static async public Task UpdateAll()
    {
        StreamReader reader = new StreamReader(Program.rss_list);

        string? line;
        List<Task> tasks = new List<Task>();
        while ((line = reader.ReadLine()) != null)
        {
            tasks.Add(Program.UpdateFeed(line));
        }
        Task.WaitAll(tasks.ToArray());
    }

    static async public Task UpdateFeed(string feed_url)
    {
        Console.WriteLine($"Processing {feed_url} feed");

        List<Task> tasks = new List<Task>();
        XmlReader reader = XmlReader.Create(feed_url);
        SyndicationFeed feed = await Task.Run(() => SyndicationFeed.Load(reader));
        reader.Close();
        foreach (SyndicationItem item in feed.Items)
        {
            tasks.Add(Program.ProcessPage(item.Links.First().Uri.ToString()));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"Finished processing {feed_url} feed");
    }

    static async public Task ProcessPage(string page_url)
    {
        Program.total_count += 1;
        if (!await Program.processed.Update(page_url)) {
            return;
        } 
        Console.WriteLine($"Page {page_url} dowloading");
        try
        {
            using (WebClient client = new WebClient())
            {
                var file = $"{Program.news_folder}/{page_url.Replace("/", "-").Replace(".html", "")}.html";
                await client.DownloadFileTaskAsync(page_url, file);
            }
            Program.downloaded += 1;
            Console.WriteLine($"Page {page_url} downloaded");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error whiele processing page {page_url}: {e.Message}");
        }

    }
}