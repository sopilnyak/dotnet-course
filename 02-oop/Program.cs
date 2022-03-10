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

using System;


public interface ILogger {
    public void Log(string message);
}


public interface ISorter<ArrayItemType> where ArrayItemType: IComparable {
    public string AlgorithmName { get; }
    public bool Fits(ref ArrayItemType[] array);
    public void Sort(ref ArrayItemType[] array);
}


public class ComplexSorter<ArrayItemType> where ArrayItemType: IComparable {
    private ISorter<ArrayItemType>[] sorters_;
    private ILogger logger_;

    public ComplexSorter(ISorter<ArrayItemType>[] sorters, ILogger logger) {
        sorters_ = sorters;
        logger_ = logger;
    }

    public void Sort(ref ArrayItemType[] array) {
        logger_.Log($"Получен массив из {array.Length} элементов");

        var is_sorted = false;
        foreach (var sorter in sorters_) {
            if (sorter.Fits(ref array)) {
                logger_.Log($"Выбран алгоритм {sorter.AlgorithmName}");
                var watch = System.Diagnostics.Stopwatch.StartNew();

                sorter.Sort(ref array);

                watch.Stop();
                logger_.Log($"Отсортировано за {watch.ElapsedMilliseconds} мс   ");

                is_sorted = true;
                break;
            }
        }

        if (!is_sorted) {
            throw new System.Exception("There is no sorting algorithm that fits passed array");
        }
    }

}


public class ConsoleLogger: ILogger {
    public void Log(string message) {
        Console.WriteLine(message); 
    }
}


public class FileLogger: ILogger {
    public FileLogger(string path_to_file) {
        PathToFile = path_to_file;
    }

    public string PathToFile { get; set; }
    
    public void Log(string message) {
        using var file = new System.IO.StreamWriter(PathToFile, true);
        file.WriteLine(message);
    }
}


public class BubbleSort<ArrayItemType>: ISorter<ArrayItemType> where ArrayItemType: IComparable {
    public string AlgorithmName { get { return "Bubble Sort"; } }

    public bool Fits(ref ArrayItemType[] array) {
        return array.Length < 16;
    }
    
    public void Sort(ref ArrayItemType[] array) {
        var array_modified = true;
        while (array_modified) {
            array_modified = false;

            for (int i = 0; i < array.Length - 1; i++) {
                if (array[i].CompareTo(array[i + 1]) > 0) {
                    (array[i], array[i + 1]) = (array[i + 1], array[i]);
                    array_modified = true;
                }
            }
        }
    }
}


public class QuickSort<ArrayItemType>: ISorter<ArrayItemType> where ArrayItemType: IComparable {
    public string AlgorithmName { get { return "Quick Sort"; } }

    public bool Fits(ref ArrayItemType[] array) {
        return true;
    }

    public void Sort(ref ArrayItemType[] array) {
        SortSegment(ref array, 0, array.Length);
    }

    private void SortSegment(ref ArrayItemType[] array, int begin, int end) {
        if (begin < end) {
            // Console.WriteLine($"{begin} {end}");

            var pivot = array[begin];
            var less_cursor = begin + 1;
            for (var i = begin + 1; i < end; i++) {
                if (array[i].CompareTo(pivot) <= 0) {
                    (array[i], array[less_cursor]) = (array[less_cursor], array[i]);
                    less_cursor ++;
                }
            }
            (array[less_cursor - 1], array[begin]) = (array[begin], array[less_cursor - 1]);

            SortSegment(ref array, begin, less_cursor - 1);
            SortSegment(ref array, less_cursor, end);
        }
    }
}


static class Program {
    static bool CheckArrayOrdering(ref int[] array) {
        for (var i = 0; i + 1 < array.Length; i++) {
            if (array[i] > array[i + 1]) {
                Console.WriteLine("Failure");
                return false;
            }
        }

        Console.WriteLine("OK");
        return true;
    }

    static void FillWithRandomNumbers(ref int[] array) {
        Random random_generator = new System.Random();
        for (int i = 0; i < array.Length; i++) {
            array[i] = random_generator.Next(0, 100000);
        }
    }

    static void Main(string[] args) {
        ISorter<int>[] sorters = {new BubbleSort<int>(), new QuickSort<int>()};
        var sorter = new ComplexSorter<int>(sorters, new ConsoleLogger());

        int[] tiny_array = {1, 9, 3, 7, 5, 6, 5};
        sorter.Sort(ref tiny_array);
        CheckArrayOrdering(ref tiny_array);

        int[] huge_array = new int[3000000];
        FillWithRandomNumbers(ref huge_array);
        sorter.Sort(ref huge_array);
        CheckArrayOrdering(ref huge_array);

        var sorter_with_file = new ComplexSorter<int>(sorters, new FileLogger("output.txt"));
        FillWithRandomNumbers(ref huge_array);
        sorter_with_file.Sort(ref huge_array);
        CheckArrayOrdering(ref huge_array);
    }
}
