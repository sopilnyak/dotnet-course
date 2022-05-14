namespace Multithreading.Solution;

internal class RssInfo
{
    internal string Link { get; init; } = null!;

    internal string GetFeedName()
    {
        return Link.Split('/').Last().Split('.').First();
    }
}