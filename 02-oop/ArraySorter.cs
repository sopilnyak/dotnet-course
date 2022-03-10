using System;
using System.Diagnostics;

namespace HW1
{
    public class ArraySorter<T> : IArraySort<T> where T : IComparable<T>
    {
        private T[] _items;
        private ILog _logger;

        public ArraySorter(T[] items, ILog logger)
        {
            _items = items;
            _logger = logger;
        }

        public void Sort()
        {
            _logger.Log("Получен массив из {0} элементов", _items.Length);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (_items.Length < 10)
            {
                _logger.Log("Выбран алгоритм сортировки вставками");
                InsertionSort(_items);
            }
            else
            {
                _logger.Log("Выбран алгоритм сортировки выбором");
                SelectionSort(_items);
            }

            stopwatch.Stop();
            _logger.Log("Отсортировано за {0} времени", stopwatch.Elapsed);
        }

        public static void SelectionSort(T[] items)
        {
            T temp;
            int smallest;
            int n = items.Length;
            for (int i = 0; i < n - 1; i++)
            {
                smallest = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (items[j].CompareTo(items[smallest]) > 0)
                    {
                        smallest = j;
                    }
                }

                temp = items[smallest];
                items[smallest] = items[i];
                items[i] = temp;
            }
        }

        private static void InsertionSort(T[] items)
        {
            int n = items.Length;
            T val;
            bool flag;
            for (int i = 1; i < n; i++)
            {
                val = items[i];
                flag = false;
                for (int j = i - 1; j >= 0 && flag != true;)
                {
                    if (items[j].CompareTo(val) > 0)
                    {
                        items[j + 1] = items[j];
                        j--;
                        items[j + 1] = val;
                    }
                    else flag = true;
                }
            }
        }
    }
}