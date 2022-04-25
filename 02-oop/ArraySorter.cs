namespace OOP
{
    public interface SortAlgorithm<T> where T : IComparable
    {
        void Sort(T[] array);
    }

    public class BubbleSort<T> : SortAlgorithm<T> where T : IComparable
    {
        public void Sort(T[] array)
        {
            int len = array.Length;
            for (int i = 0; i < len; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    if (array[i].CompareTo(array[j]) > 0)
                    {
                        (array[i], array[j]) = (array[j], array[i]);
                    }
                }
            }
        }
    }

    public class QuickSort<T> : SortAlgorithm<T> where T : IComparable
    {
        public void Sort(T[] array)
        {
            QuickSortImpl(array, array.Length);
        }
        void QuickSortImpl(T[] data, int len)
        {
            int ind = len / 2;
            int leftLen = 0, rightLen = 0;
            if (len <= 1)
            {
                return;
            }
            var left_array = new T[len];
            var right_array = new T[len];
            var pivot = data[ind];
            for (int i = 0; i < len; i++)
            {
                if (i != ind)
                {
                    if (data[i].CompareTo(pivot) < 0)
                    {
                        left_array[leftLen] = data[i];
                        leftLen++;
                    }
                    else
                    {
                        right_array[rightLen] = data[i];
                        rightLen++;
                    }
                }
            }
            QuickSortImpl(left_array, leftLen);
            QuickSortImpl(right_array, rightLen);
            for (int cnt = 0; cnt < len; cnt++)
            {
                if (cnt < leftLen)
                {
                    data[cnt] = left_array[cnt];
                }
                else if (cnt == leftLen)
                {
                    data[cnt] = pivot;
                }
                else
                {
                    data[cnt] = right_array[cnt - (leftLen + 1)];
                }
            }
        }
    }

    public class ArraySorter<T> where T : IComparable
    {
        public T[] Array { get; }
        private ILogger _logger;

        private SortAlgorithm<T> _sorter;
        public ArraySorter(T[] array, ILogger logger)
        {
            Array = array;
            _logger = logger;
            _logger.Logging($"получен массив из {array.Length} элементов");
            ChooseSortAlgo();
        }
        void ChooseSortAlgo()
        {
            if (Array.Length > 10)
            {
                _logger.Logging("Выбран алгоритм быстрой сортировки");
                _sorter = new QuickSort<T>();
            }
            else
            {
                _logger.Logging("Выбран алгоритм пузырьковой сортировки");
                _sorter = new BubbleSort<T>();
            }
        }
        public void Sort()
        {
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            _sorter.Sort(Array);
            startTime.Stop();
            var resultTime = startTime.Elapsed;

            string time_for_logging = String.Format("Отсортировано за {0}.{1} секунд",
                resultTime.Seconds,
                resultTime.Milliseconds);
            _logger.Logging(time_for_logging);
        }
    }
}
