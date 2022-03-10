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
namespace MySortingUtility {

public interface Ilog {
    void Log(string m);
}

public class ConsoleLog: Ilog {
    public ConsoleLog() {}
    public void Log(string m) {
        Console.WriteLine(m);
    }
}

public class FileLog: Ilog {
    private string filename;
    public FileLog(string filename) {
        this.filename = filename;
    }
    public void Log(string m) {
        File.WriteAllText(filename, m);
    }
}


public class GenericArraySorter<T> where T:IComparable {
    private T[] array;
    private int size_constant;
    private bool is_sorted = false;
    private Ilog logger;
    public GenericArraySorter(T[] array, Ilog logger, int size_constant) {
        this.array = array;
        this.size_constant = size_constant;
        this.logger = logger;

        logger.Log($"Array with length {array.Length} accepted");
    }

    public GenericArraySorter(T[] array, int size_constant): this(array, new ConsoleLog(), size_constant){}
    public GenericArraySorter(T[] array): this(array, new ConsoleLog(), 100) {}

    public T[] Array {
        get {
            if (!is_sorted) Sort();
            return array;
        }
        set {
            array = value;
            is_sorted = false;
        }
    }

    void BubbleSort() {
        for (int i = 0; i < array.Length; i++) {
            for (int j = 0; j < array.Length; j++) {
                if (array[i].CompareTo(array[j]) < 0) {
                    (array[i], array[j]) = (array[j], array[i]);
                    }
                }
            }
        }

    void QuickSort() {
        int Partition(int low, int high) {

            T pivot = array[high];
            int i = (low - 1);
        
            for (int j = low; j <= high - 1; j++)
            {
                if (array[j].CompareTo(pivot) < 0)
                {
                    i++;
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }
            (array[i + 1], array[high]) = (array[high], array[i + 1]);

            return i + 1;
        }

        void Sort(int low, int high) {
            if (low < high) {
                int partition = Partition(low, high);
                Sort(low, partition - 1);
                Sort(partition + 1, high);
            }
        }

        Sort(0, array.Length - 1);
    }

    void Sort() {
        var startTime = System.Diagnostics.Stopwatch.StartNew();

        if (array.Length < size_constant) {
            BubbleSort();
            logger.Log("Using bubblesort");
        } else {
            QuickSort();
            logger.Log("Using quicksort");
        }

        startTime.Stop();
        var resultTime = startTime.Elapsed;

        is_sorted = true;
        
        logger.Log(String.Format("Sorted in {0}.{1} seconds", resultTime.Seconds, resultTime.Milliseconds));
    }
}

class Program {
        public static void Main (string[] args) {
            int[] array = {1, 2, 3, 2, 5, 4, 9, 6, 7};
            GenericArraySorter<int> sorter = new GenericArraySorter<int>(array);
            foreach (int number in sorter.Array)
            {
                Console.WriteLine($"{number}");
            } 

            int[] array2 = {1, 2, 3, 2, 5, 4, 9, 6, 7, 0 ,12, 45, 32, 23, 10, 5, 10};
            GenericArraySorter<int> sorter2 = new GenericArraySorter<int>(array2, 10);
            foreach (int number in sorter2.Array)
            {
                Console.WriteLine($"{number}");
            } 
        }
    }
}




