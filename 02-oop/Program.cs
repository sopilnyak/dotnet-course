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

class ConsoleLogger : ILogger {
    public void Log(string message) {
        Console.WriteLine(message);
    }
}

class FileLogger : ILogger {
    public string filename;

    public FileLogger(string filename) {
        this.filename = filename;
    }

    public void Log(string message) {
        StreamWriter writer = new StreamWriter(this.filename, true);
        writer.WriteLine(message);
        writer.Close();
    }
}

class Sort<T> where T: IComparable {
    public static T[] sort(T[] arr, ILogger logger) {
        logger.Log($"Получен массив из {arr.Length} элементов");
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        T[] ans;
        if (arr.Length % 2 == 0) {
            logger.Log("Выбран алгоритм BubbleSort");
            ans = BubbleSort<T>.sort(arr);
        } else {
            logger.Log("Выбран алгоритм SelectionSort");
            ans = SelectionSort<T>.sort(arr);
        }
        watch.Stop();
        logger.Log($"Отсортировано за {watch.ElapsedMilliseconds} милисекунд");
        return ans;
    }
}

class BubbleSort<T> where T: IComparable {
    public static T[] sort(T[] arr) {
        T[] ans = new T[arr.Length];
        Array.Copy(arr, ans, arr.Length);
        for (int i = 0; i < arr.Length - 1; ++i) {
            for (int j = 0; j < arr.Length - i - 1; ++j) {
                if (ans[j].CompareTo(ans[j + 1]) > 0) {
                    T tmp = ans[j];
                    ans[j] = ans[j + 1];
                    ans[j + 1] = tmp;
                }
            }
        }
        return ans;
    }
}

class SelectionSort<T> where T: IComparable {
    public static T[] sort(T[] arr) {
        T[] ans = new T[arr.Length];
        Array.Copy(arr, ans, arr.Length);
        for (int i = 0; i < arr.Length - 1; ++i) {
            int min = i;
            for (int j = i + 1; j < arr.Length; ++j) {
                if (ans[min].CompareTo(ans[j]) > 0) {
                    min = j;
                }
            }
            T tmp = ans[i];
            ans[i] = ans[min];
            ans[min] = tmp;
        }
        return ans;
    }
}

class Program {
    public static void Main(string[] args) {
        Console.WriteLine("Hello");
        int[] arr = new int[1000];
        for (int i = 0; i < 1000; ++i) {
            arr[i] = i * 42 % 17;
        }
        int[] ans = Sort<int>.sort(arr, new FileLogger("mray.txt"));
        Console.WriteLine(ans.Length);
        for (int i = 0; i < ans.Length; ++i) {
            Console.Write(ans[i]);
            Console.Write(" ");
        }
        Console.WriteLine();

        arr = new int[1001];
        for (int i = 0; i < 1001; ++i) {
            arr[i] = i * 42 % 17;
        }
        ans = Sort<int>.sort(arr, new FileLogger("mray.txt"));
        Console.WriteLine(ans.Length);
        for (int i = 0; i < ans.Length; ++i) {
            Console.Write(ans[i]);
            Console.Write(" ");
        }
        Console.WriteLine();
    }
}