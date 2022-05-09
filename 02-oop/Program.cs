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

public interface ILogger {
    public void Log(string s);
}

public class ConsoleLogger: ILogger {
    public void Log(string s) {
        System.Console.WriteLine(s);
    }
}

public class FileLogger: ILogger {
    private string filename_;
    public FileLogger(string filename) {
        filename_ = filename;
    }

    public void Log(string s) {
        var writer = new StreamWriter(filename_, true);
        writer.WriteLine(s);
        writer.Close();
    }

    /**
    Пожалуйста, расскажите, почему подход 

        private System.IO.StreamWriter writer;

        public FileLogger(string filename) {
            writer = new StreamWriter(filename, true);
        }

        public void Log(string s) {
            writer.WriteLine(s);
        }

        ~FileLogger() {
            writer.Close();
        }

    не работает?:(
    **/
}

public interface ISorter<T> {
    void Sort(T[] arr);
}

public class MergeSorter<T>: ISorter<T> where T: System.IComparable {
    public void Sort(T[] arr) {
        if (arr.Length <= 1) {
            return;
        }
        T[] first_slice = arr[0..(arr.Length / 2)];
        T[] second_slice = arr[(arr.Length / 2)..];
        Sort(first_slice);
        Sort(second_slice);

        uint i = 0, j = 0, k = 0;
        while(i < arr.Length) {
            if (k >= second_slice.Length || j < first_slice.Length && first_slice[j].CompareTo(second_slice[k]) < 0) {
                arr[i] = first_slice[j];
                j++;
            } else {
                arr[i] = second_slice[k];
                k++;
            }
            i++;
        }
    }
}

public class BubbleSorter<T>: ISorter<T> where T: System.IComparable {
    public void Sort(T[] arr) {
        for(uint i = 0; i < arr.Length; ++i) {
            var min_idx = MinIdx(arr, i);
            T tmp = arr[i];
            arr[i] = arr[min_idx];
            arr[min_idx] = tmp;
        }
    }

    private uint MinIdx(T[] arr, uint start) {
        uint min_idx = start;
        for(uint i = start + 1; i < arr.Length; ++i) {
            if (arr[i].CompareTo(arr[min_idx]) < 0) {
                min_idx = i;
            }
        }
        return min_idx;
    }
}

public class SortMachine {
    private ILogger logger;
    public SortMachine(ILogger Logger) {
       logger = Logger;
    }

    public void Sort<T>(T[] arr) where T: System.IComparable {
        logger.Log("------------------------------------------------------");
        logger.Log($"Got the array with length {arr.Length}.");
        ISorter<T> sorter;
        string sorting_method;
        logger.Log($"Let's choose a sorting method.");
        logger.Log($"Flipping a coin...");

        if (arr.Length * arr.Length <= 2 * arr.Length * System.Math.Log2(arr.Length)) {
            sorter = new BubbleSorter<T>();
            sorting_method = "Bubble";
        } else {
            sorter = new MergeSorter<T>();
            sorting_method = "Merge";
        }
        logger.Log($"Choosen method: {sorting_method} sort.");

        sorter.Sort(arr);

        logger.Log($"Successfully sorted!");
        logger.Log("------------------------------------------------------\n");

    }
}

public class Program {
    public static void Main(string[] argv) {
        int[] arr = {420, 42, 69, 7, 13, 666, 1337, 228};
        float[] arr_pomenshe = {4.5F, 1F, 3.14F};

        var console_logger = new ConsoleLogger();
        var file_logger = new FileLogger("log.out");
        var console_sorter = new SortMachine(console_logger);
        var file_sorter = new SortMachine(file_logger);

        console_sorter.Sort(arr);
        file_sorter.Sort(arr_pomenshe);

        if (!isSorted(arr) || !isSorted(arr_pomenshe)) {
            console_logger.Log("Error. Don't worry: only 10% of programmers can write a binary search.");
        }
    }

    public static bool isSorted<T>(T[] arr) where T: System.IComparable {
        for (uint i = 1; i < arr.Length; ++i) {
            if (arr[i - 1].CompareTo(arr[i]) > 0) {
                return false;
            }
        }
        return true;
    }
}