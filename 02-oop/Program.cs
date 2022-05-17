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

interface ILogger {
    public void Log(string message);
}

class FileLogger : ILogger {
    private StreamWriter writer;
    public FileLogger(string filename) {
        this.writer = new StreamWriter(filename, true);
    }

    public void Log(string message) {
        this.writer.WriteLine(message);
        this.writer.Flush();
    }
}

class NopeLogger : ILogger {
    public void Log(string message) {}

    public static readonly NopeLogger instance = new NopeLogger();
}

class StdErrLogger : ILogger {
    public void Log(string message) {
        Console.Error.WriteLine(message);
    }
}

interface ISortAlgorithm<T> {
    public void Sort(T[] array);
    public string GetName();
}

class MergeSort<T> : ISortAlgorithm<T> where T:IComparable {
    public void Sort(T[] array) {
        SortImpl(array, 0, array.Length);
    } 

    public string GetName() {
        return "MergeSort";
    }

    private static void SortImpl(T[] array, int from, int to) {
        if (from + 1 == to) {
            return;
        }
        int mid = (from + to) / 2;
        SortImpl(array, from, mid);
        SortImpl(array, mid, to);
        Merge(array, from, mid, to);
    }

    private static void Merge(T[] array, int from, int mid, int to) {
        T[] res = new T[to - from];
        int res_it = 0;
        int left_it = from;
        int right_it = mid;
        while (res_it < (to - from)) {
            if (left_it == mid) {
                res[res_it] = array[right_it];
                right_it++;
            } else if (right_it == to) {
                res[res_it] = array[left_it];
                left_it++;
            } else if (array[left_it].CompareTo(array[right_it]) <= 0) {
               res[res_it] = array[left_it];
               left_it++;
            } else {
                res[res_it] = array[right_it];
                right_it++;
            }
            res_it++;
        }
        Array.Copy(res, 0, array, from, res.Length); 
    }
}

class BubbleSort<T> : ISortAlgorithm<T> where T:IComparable {
    public void Sort(T[] array) {
        for (int i = 0; i + 1 < array.Length; i++) {
            for(int j = 0; j + i + 1 < array.Length; j++) {
                if (array[j].CompareTo(array[j + 1]) > 0) {
                    T tmp = array[j];
                    array[j] = array[j+1];
                    array[j+1] = tmp;
                }
            }    
        }
    }

    public string GetName() {
        return "BubbleSort";
    }
}

class Sorter<T> where T:IComparable {
    public static void Sort(T[] array, ILogger log) {
        log.Log(String.Format("Get array with size {0}", array.Length));
        ISortAlgorithm<T> algo = array.Length < BubbleSortMaxLength ? new BubbleSort<T>() : new MergeSort<T>();
        log.Log(String.Format("Chosen alogrithm: {0}", algo.GetName()));


        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        algo.Sort(array);

        stopWatch.Stop();
        log.Log(String.Format("Time: {0:000}ms", stopWatch.Elapsed.TotalMilliseconds));
    }

    private static int BubbleSortMaxLength = 1000;
}

class IsSorted<T> where T:IComparable {
    public static bool Check(T[] array) {
        for (int i = 0; i + 1 < array.Length; i++) {
            if (array[i].CompareTo(array[i + 1]) > 0) {
                return false;
            }
        }
        return true;
    } 
}

class Tester<T> where T:IComparable {
    public static void DoTest(T[] array) {
        ILogger logger = new StdErrLogger();
        Sorter<T>.Sort(array, logger);        
        System.Diagnostics.Debug.Assert(IsSorted<T>.Check(array));
        logger.Log("Test OK");
    }
}

class Program {
    public static void Main(string[] args) {
        SmallTest();
        LargeTest();
    }

    private static void SmallTest() {
        Tester<int>.DoTest(new int[]{5, 6, 5, 4, 3, 2, 2, 3, 1});
        Tester<float>.DoTest(new float[]{5, 6, 5, 4, 3, 2, 2, 3, 1});
    }

    private static void LargeTest() {
        foreach (int size in new int[]{90, 99, 100, 105, 200, 400, 500, 800, 5000, 10000}) {
            int[] array = new int[size];
            for (int i = 0; i < array.Length; ++i) {
                array[i] = i * (i + 1) * (i % 2 == 0 ? 1 : -1);
            }
            Tester<int>.DoTest(array);
        }
    }
}