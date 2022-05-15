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

using loggers;
using sorters;

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
            throw new ArgumentException("Объект не является типом Cat");
        }
        public string Name { get; }
        public int Weight { get; }
    }

    public class SortedArrayWithLogger<T> where T : IComparable {
        public SortedArrayWithLogger(T[] arr, ILogger logger) {
            _logger = logger;
            Arr = arr;
            _logger.Log($"Получен массив из {arr.Length} элементов");
        }

        public void Sort() {
            var watch = new System.Diagnostics.Stopwatch();
            if (Arr.Length < LimitOnItems) {
                _logger.Log("Выбран алгоритм сортировки пузырьком");
                var bubbleSort = new BubbleSort<T>();
                watch.Start();
                bubbleSort.Sort(Arr);
                watch.Stop();
            }
            else {
                _logger.Log("Выбран алгоритм быстрой сортировки");
                var quickSort = new QuickSort<T>();
                watch.Start();
                quickSort.Sort(Arr);
                watch.Stop();
            }

            _logger.Log(watch.Elapsed.Ticks <= 10000
                ? $"Отсортировано за {watch.Elapsed.Ticks / 10} микросекунд"
                : $"Отсортировано за {watch.Elapsed.Milliseconds} миллисекунд");
        }

        private ILogger _logger;
        public T[] Arr { get; }
        private const int LimitOnItems = 20;
    }
    public class Program {
        public static void TestCats() {
            // Тест на тип Comparable
            Cat[] cats = {
                new Cat("DD", 6),
                new Cat("LuLu", 4),
                new Cat("LaLa", 5),
            };

            ILogger logger = new ConsoleLogger();
            logger.Log("==================================");
            logger.Log("Test on IComparable type");
            logger.Log("==================================");

            var sortedArr = new SortedArrayWithLogger<Cat>(cats, logger);
            
            sortedArr.Sort();

            foreach (var cat in sortedArr.Arr) {
                logger.Log($"Name: {cat.Name}, Weight: {cat.Weight}");
            }
            logger.Log("==================================");
            logger.Log("Test PASSED!");
            logger.Log("==================================");
        }

        public static void TestIntArray() {
            // Тест с большим кол-вом элементов
            Random random = new Random(42);

            int len = 1000000;

            int[] arr = new int[len];
            
            for (int i = 0; i < len; ++i) {
                arr[i] = random.Next(0, 100);
            }

            ILogger logger = new FileLogger("/home/jidge/dotnet-course/02-oop/log.txt");
            logger.Log("==================================");
            logger.Log("Test on large amount of elements");
            logger.Log("==================================");

            var sortedArr = new SortedArrayWithLogger<int>(arr, logger);
            
            sortedArr.Sort();

            var item = sortedArr.Arr[0];
            
            foreach (var elem in sortedArr.Arr) {
                if (item > elem) {
                    logger.Log("==================================");
                    logger.Log("Test FAILED! Reason: Array is not sorted!");
                    logger.Log("==================================");
                    return;
                }
                item = elem;
            }

            logger.Log("Array is sorted!");
            logger.Log("==================================");
            logger.Log("Test PASSED!");
            logger.Log("==================================");
        }
        
        public static void Main(string[] args) {
            TestCats();
            TestIntArray();
        }
    }
}