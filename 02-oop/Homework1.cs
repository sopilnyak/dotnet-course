using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class Program {
     static void Main(){

        int[] arr = {2, 1, 3, 2, 9, 8, 7, 6};
        string[] arr2 = {"adad", "wqw", "aaa"};
        
        Console.WriteLine("Введите 0, если хотите вывести результат в консоль");
        Console.WriteLine("Или 1, если хотите записать результат в файл");
        Console.WriteLine("Если хотите записать результат в файл и в консоль, введите 2 или другое целое число");

        int k = Convert.ToInt32(Console.ReadLine());

        Logger<int> logger = new Logger<int>(arr);

        switch (k)
        {
            case 0:
                logger.PrintToConsole();
                break;
            case 1:
                logger.PrintToFile();
                break;
            default:
                logger.PrintToFile();
                logger.PrintToConsole();
                break;
        }
     }
}

public interface ISort<T> where T : IComparable<T>
{
    public void Sort(T[] m, int a, int b);
}

public class QuickSort<T>: ISort<T> where T : IComparable<T>
{ 
    public void Sort(T[] a, int l, int r)
    {
        T temp;
        T x = a[l + (r - l) / 2];

        int i = l;
        int j = r;

        while (i <= j)
        {
            while (a[i].CompareTo(x) < 0) i++;
            while (a[j].CompareTo(x) > 0) j--;
            if (i <= j)
            {
                temp = a[i];
                a[i] = a[j];
                a[j] = temp;
                i++;
                j--;
            }
        }
        if (i < r)
            Sort(a, i, r);
 
        if (l < j)
            Sort(a, l, j);
    }
}

public class BubbleSort<T> : ISort<T> where T : IComparable<T>
{
    public void Sort(T[] m, int a, int b)
    {
        for (int i = a; i < b; i++)
        for (int j = a; j < b - i - 1; j++)
        {
            if (m[j].CompareTo(m[j + 1]) > 0)
            {
                T temp = m[j];
                m[j] = m[j + 1];
                m[j + 1] = temp;
            }
        }
    }
}

public class Sorter<T> where T : IComparable<T>
{
    private T[] list;
    public string algtype;
    public TimeSpan ts;
    public Sorter(T[] arr)
    {
        list = arr;
    }

    public void Sort()
    {
        if (list.Length > 1000)
        {
            QuickSort<T> s = new QuickSort<T>();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            s.Sort(list, 0, list.Length - 1);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            algtype = "Выбран алгоритм быстрой сортировки";
        }
        else
        {
            BubbleSort<T> s = new BubbleSort<T>();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            s.Sort(list, 0, list.Length);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            algtype = "Выбран алгоритм сортировки пузырьком";
        }
    }
}

public class Logger<T> where T : IComparable<T>
{

    private T[] list_; 
    private T[] sorted_list_;

    public Logger(T[] list)
    {
        list_ = list;
    }

    public void PrintToConsole(){
        Console.WriteLine("Запись в консоль...");
        Console.WriteLine("Получен массив из {0} элементов: ", list_.Length);

        foreach (var a in (list_))
        {
            Console.Write(a + " ");
        }
        
        Console.WriteLine();

        sorted_list_ = list_;

        Sorter<T> c0 = new Sorter<T>(sorted_list_);
        c0.Sort();
        
        Console.WriteLine(c0.algtype);
        Console.Write("Runtime: " );
        Console.WriteLine(c0.ts.ToString());

        Console.WriteLine("Отсортированный массив: ");

        foreach (var a in (sorted_list_))
        {
            Console.Write(a + " ");
        }
    }

    public void PrintToFile(string path = "sort.txt")
    {
        Console.WriteLine("Запись в файл...");
                
        FileStream file1 = new FileStream(path, FileMode.Create);
                
        using (StreamWriter writer = new StreamWriter(file1))
        {
            writer.WriteLineAsync($"Получен массив из {list_.Length} элементов: ");

            foreach (var a in (list_))
            {
                writer.Write(a + " ");
            }
        
            writer.WriteLineAsync("\n");

            sorted_list_ = list_;

            Sorter<T> c = new Sorter<T>(sorted_list_);
            c.Sort();

            writer.WriteLineAsync(c.algtype);
            writer.WriteAsync("Runtime: " );
            writer.WriteLineAsync(c.ts.ToString());

            writer.WriteLineAsync("Отсортированный массив: ");

            foreach (var a in (sorted_list_))
            {
                writer.WriteAsync(a + " ");
            }
                    
            writer.Close();
        }
                
        Console.WriteLine($"Название файла: {path}");
    }

}