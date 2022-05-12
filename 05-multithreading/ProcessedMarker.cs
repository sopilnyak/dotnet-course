public static class ProcessedMarker
{
    static string processed_articles = "data/processed_articles.txt";
    static ReaderWriterLock locker = new ReaderWriterLock();
    const int delay = 2000;

    public static string[] GetProcessed()
    {
        string[] result = new string[] { };
        locker.AcquireWriterLock(delay);
        result = File.ReadAllLines(processed_articles);
        locker.ReleaseWriterLock();
        return result;
    }

    public static void MarkProcessed(this Uri link)
    {
        locker.AcquireWriterLock(delay);
        System.IO.File.AppendAllLines(processed_articles, new[] { link.ToString() });
        locker.ReleaseWriterLock();
    }
}