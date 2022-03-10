/*
    Нужно реализовать систему классов и интерфейсов, которая умеет сортировать массив объектов любого типа (см. интерфейс IComparable).
    
    Требования:
     - Поддержка минимум двух алгоритмов сортировки.
     - Выбор алгоритма сортировки на основании исходного массива (ради упрощения, выбирать алгоритм можно учитывая количество элементов).
     - Логгирование хода работы программы: «получен массив из N элементов», «выбран алгоритм K», «отсортировано за M времени».
     - Поддержка настройки механизма логгирования: в консоль и/или на диск.

    10 баллов
    Мягкий дедлайн: 10.03.2022 23:59
    Жесткий дедлайн: 12.05.2022 23:59
*/
// See https://aka.ms/new-console-template for more information-
using System.Diagnostics;

class Program
{
    public static void Main()
    {
        //tests with int and string
        Console.WriteLine("Hello, World!");
        //int[] list = new []{1, 3, 232, 545, 32, 5, 23};
        string[] list = new[] {"boogy-woogy", "Arra", "a", "aboba",  "dsfdgas" };
        
        Console.Write("Исходный массив: ");
        for (int i = 0; i < list.Length; i++)
        {
            Console.Write("[" + i.ToString() + "]: " + list[i] + " ");
        }
        Console.WriteLine();
        Comp<string> comp = new Comp<string>(list, consoleLog: true, fileLog:true, pathLog:"text.txt");
        comp.Sort();
        Console.WriteLine();
        Console.Write("Отсортированный массив: ");
        for (int i = 0; i < list.Length; i++)
        {
            Console.Write("[" + i.ToString() + "]: " + list[i] + " ");
        }
    }
}


interface ILogger {
    void Logging(string message);
}

public class FileLogger: ILogger {
    public string pathLog; 
    public FileLogger(string filename) {
        pathLog = filename;
    }
    public void Logging(string message) {
        using (StreamWriter writer = new StreamWriter(pathLog, true)) {
            writer.WriteLine(message);
        }
    } 
}

public class Logger: ILogger {
    private string PathLog; 
    public Logger(string filename) {
        PathLog = filename;
    }
    public void Logging(string message) {
        Console.WriteLine(message);
        using (StreamWriter writer = new StreamWriter(PathLog, true)) {
            writer.WriteLine(message);
        }
    }
}

public class ConsoleLogger: ILogger {
    public ConsoleLogger() {}
    public void Logging(string message) {
        Console.WriteLine(message);
    }
}

public class EmptyLogger: ILogger {
    public EmptyLogger() {}
    public void Logging(string message) {}
}

public interface ISort<T> where T : IComparable
{
    void Sort(T[] items);
}

public class BubbleSort<T> : ISort<T> where T : IComparable
{
    public void Sort(T[] items)
    {
        for (int write = 0; write < items.Length; write++) {
            for (int sort = 0; sort < items.Length - 1; sort++) {
                if (items[sort].CompareTo(items[sort + 1]) > 0) {
                    (items[sort + 1], items[sort]) = (items[sort], items[sort + 1]);
                }
            }
        }
    }
}

public class QuickSort<T> : ISort<T> where T : IComparable
{
    public void Sort(T[] items)
    {
        _QuickSort(items, 0, items.Length);
    }
    static void _QuickSort(T[] data, int left, int right)
    {
        int i = left - 1,
            j = right;

        while (true)
        {
            T d = data[left];
            do i++; while (data[i].CompareTo(d) < 0);
            do j--; while (data[j].CompareTo(d) > 0);

            if (i < j)
            {
                (data[i], data[j]) = (data[j], data[i]);
            }
            else
            {
                if (left < j)    _QuickSort(data, left, j);
                if (++j < right) _QuickSort(data, j, right);
                return;
            }
        }
    }
}

public class Comp<T> where T : IComparable
{
    private T[] _items { get; set; }
    private ISort<T> sorter;
    private ILogger logger;
    public Comp(T[] items, bool fileLog = false, bool consoleLog = false, string pathLog = "log.txt")
    {
        if (fileLog && consoleLog)
            logger = new Logger(pathLog);
        else if (fileLog)
            logger = new FileLogger(pathLog);
        else if (consoleLog)
            logger = new ConsoleLogger();
        else
            logger = new EmptyLogger();
        
        _items = items;
        var size = items.Length;
        logger.Logging("Получен массив из " + size.ToString() + " элементов.");
        if (size < 1000)
            sorter = new BubbleSort<T>();
        else
            sorter = new QuickSort<T>();
        logger.Logging("Выбран алгоритм " + sorter.ToString() + ".");
    }

    public void Sort()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        ////////
        sorter.Sort(_items);
        ////////
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;
        // Format and display the TimeSpan value.
        string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        logger.Logging("Отсортированно за " + elapsedTime);
    }
}