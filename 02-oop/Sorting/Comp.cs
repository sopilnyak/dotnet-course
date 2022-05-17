namespace OOP.Sorting;
using OOP.Logging;
using System.Diagnostics;

public class Comp<T> where T : IComparable
{
    private T[] _items { get; }
    private ISort<T> sorter;
    private ILogger logger;
    public Comp(T[] items, bool fileLog = false, bool consoleLog = false, string pathLog = "log.txt")
    {
        if (fileLog && consoleLog)
            logger = new ConsoleFileLogger(pathLog);
        else if (fileLog)
            logger = new FileLogger(pathLog);
        else if (consoleLog)
            logger = new ConsoleLogger();
        else
            logger = new EmptyLogger();
        
        _items = items;
        var size = items.Length;
        logger.Log($"Получен массив из {size} элементов.");
        if (size < 1000)
            sorter = new BubbleSort<T>();
        else
            sorter = new QuickSort<T>();
        logger.Log($"Выбран алгоритм {sorter}.");
    }

    public void Sort()
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        sorter.Sort(_items);
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;
        // Format and display the TimeSpan value.
        string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        logger.Log($"Отсортированно за {elapsedTime}");
    }
}