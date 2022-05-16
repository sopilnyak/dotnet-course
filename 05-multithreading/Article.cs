using System.Text.RegularExpressions;

namespace _05_multithreading;

public class Article
{
    public string Name { get; }
    private readonly string _htmlPage;
    private static readonly HttpClient HttpClient = new();

    public Article(string name, string htmlPage)
    {
        Name = name;
        _htmlPage = htmlPage;
    }

    public async Task WriteToFile()
    {
        var fileName = Regex.Replace("./articles/" + Name + ".txt", @"\s+", "_");
        await using var writer = File.CreateText(fileName);
        await writer.WriteAsync(_htmlPage);
    }

    public static async Task<Article> Load(string name, string url)
    {
        var htmlPageString = await HttpClient.GetStringAsync(url);
        return new Article(name, htmlPageString);
    }
}