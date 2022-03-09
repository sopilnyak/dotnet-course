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

using System.Diagnostics;
using logger;
using sorts;

namespace logger {
    public interface ILogger {
        public void Log(string msg);
    }

    public class ConsoleLogger : ILogger {
        public ConsoleLogger() { }

        public void Log(string msg) {
            Console.WriteLine(msg);
        }
    }

    public class FileLogger : ILogger {
        public FileLogger(string filename) {
            _filename = filename;
        }

        public void Log(string msg) {
            using var file = new StreamWriter(_filename, true);
            file.WriteLine(msg);
        }

        private readonly string _filename;
    }

    public class NoLogger : ILogger {
        public NoLogger() { }
        public void Log(string msg) { }
    }

    // Не хочу нарушать SOLID и идти по пути антипаттернов
    //public class TunableLogger {}
}

namespace sorts {
    interface ISorter<T> {
        void Sort(T[] arr);
    }

    public class BubbleSort<T> : ISorter<T> where T: IComparable {
        public BubbleSort() { }

        public void Sort(T[] arr) {
            var swapped = true;

            while (swapped) {
                swapped = false;
                for (var i = 0; i < arr.Length - 1; ++i) {
                    if (arr[i].CompareTo(arr[i + 1]) <= 0) continue;
                    (arr[i], arr[i + 1]) = (arr[i + 1], arr[i]);
                    swapped = true;
                }
            }
        }
    }

    public class QuickSort<T> : ISorter<T> where T: IComparable {
        public QuickSort() {}

        public void Sort(T[] arr) {
            QuickSortRecursion(arr, 0, arr.Length - 1);
        }

        private void QuickSortRecursion(T[] arr, int start_idx, int end_idx) {
            int pivot_idx = (end_idx - start_idx + 1) / 2;
            T pivot = arr[pivot_idx];
            int begin = start_idx;
            int end = end_idx;
            while (begin <= end) {
                while (arr[begin].CompareTo(pivot) < 0) {
                    ++begin;
                }
                while (arr[end].CompareTo(pivot) > 0) {
                    --end;
                }
                if (begin <= end) {
                    (arr[begin], arr[end]) = (arr[end], arr[begin]);
                    ++begin;
                    --end;
                }
            }

            if (start_idx < begin - 1) {
                QuickSortRecursion(arr, start_idx, begin - 1);
            }

            if (begin < end_idx) {
                QuickSortRecursion(arr, begin, end_idx);
            }
        }
    }
}

namespace oop {
    public class Cat: IComparable {
        public Cat(string name, int weight) {
            Name = name;
            Weight = weight;
        }

        public int CompareTo(object? obj) {
            if (obj is Cat cat) {
                return Weight.CompareTo(cat.Weight);
            }
            throw new ArgumentException("Сравнивание несравнимых типов");
        }
        public string Name { get; }
        public int Weight { get; }
    }

    public class SortedArrayWithLogger<T> where T : IComparable {
        public SortedArrayWithLogger(T[] arr, logger.ILogger logger) {
            _logger = logger;
            this.arr = arr;
            _logger.Log($"Получен массив из {arr.Length} элементов");
        }

        public void Sort() {
            var watch = new System.Diagnostics.Stopwatch();
            if (arr.Length > _limit_on_items) {
                _logger.Log("Выбран алгоритм сортировки пузырьком");
                BubbleSort<T> bubbleSort = new BubbleSort<T>();
                watch.Start();
                bubbleSort.Sort(arr);
                watch.Stop();
            }
            else {
                _logger.Log("Выбран алгоритм быстрой сортировки");
                QuickSort<T> quickSort = new QuickSort<T>();
                watch.Start();
                quickSort.Sort(arr);
                watch.Stop();
            }
            _logger.Log($"Отсортировано за {watch.Elapsed} времени");
        }
        
        private logger.ILogger _logger;
        public T[] arr { get; }
        private const int _limit_on_items = 20;
    }
    public class Program {
        public static void Main(string[] args) {

            Cat[] cats = {
                new Cat("DD", 6),
                new Cat("LuLu", 4),
                new Cat("LaLa", 5),
            };

            ILogger logger = new ConsoleLogger();

            var sorted_arr = new SortedArrayWithLogger<Cat>(cats, logger);
            
            sorted_arr.Sort();

            foreach (var cat in sorted_arr.arr) {
                logger.Log($"Name: {cat.Name}, Weight: {cat.Weight}");
            }
            logger.Log("==================================");
            
        }
    }
}