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
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    public class Log
    {
        private static Stopwatch stopWatch;
        public static void StartLog<T>(T[] items, string algo_name, bool is_infile)
        {
            string line1 = "получен массив из " + items.Length + " элементов" + "\r\n";
            string line2 = "выбран алгоритм " + algo_name + "\r\n";
            if (is_infile) {
                using(FileStream stream = new FileStream("logs.txt", FileMode.Append)){
                    byte[] array1 = System.Text.Encoding.Default.GetBytes(line1);
                    byte[] array2 = System.Text.Encoding.Default.GetBytes(line2);

                    stream.Write(array1, 0, array1.Length);
                    stream.Flush();
                    stream.Write(array2, 0, array2.Length);
                    stream.Flush();
                }
            }
            else
            {
                Console.Write(line1);
                Console.Write(line2);
            }
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }
        public static void EndLog<T>(T[] items, bool is_infile)
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            string line1 = "отсортировано за " + elapsedTime  +" времени" + "\r\n\n";

            if (is_infile) {
                using(FileStream stream = new FileStream("logs.txt", FileMode.Append)){
                    byte[] array1 = System.Text.Encoding.Default.GetBytes(line1);
                    stream.Write(array1, 0, array1.Length);
                    stream.Flush();
                }
            }
            else
            {
                Console.Write(line1);
                Console.Write('\n');
            }

        }
    }

    interface ISorter {
        void Sort<T>(T[] items) where T : IComparable;
    }

    public class MergeSort : ISorter {
        private int lenght;
        private void MergeSort_Recursive<T>(T[] numbers, T[] temp, int left, int right) where T : IComparable
        {
            int mid;
            if (right > left)
            {
                mid = (right + left) / 2;
                MergeSort_Recursive(numbers, temp, left, mid);
                MergeSort_Recursive(numbers, temp, (mid + 1), right);

                DoMerge(numbers, temp, left, (mid + 1), right);
            }
            return;
        }
        private void DoMerge<T>(T[] numbers, T[] temp, int left, int mid, int right) where T : IComparable
        {
            int left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
            {
                if (numbers[left].CompareTo(numbers[mid]) <= 0)
                    temp[tmp_pos++] = numbers[left++];
                else
                    temp[tmp_pos++] = numbers[mid++];
            }

            while (left <= left_end)
                temp[tmp_pos++] = numbers[left++];

            while (mid <= right)
                temp[tmp_pos++] = numbers[mid++];
            for (int i = 0; i < num_elements; i++)
            {
                numbers[right] = temp[right];
                right--;
            }
        }
        public void Sort<T>(T[] items) where T : IComparable
        {
            lenght = items.Length;
            var temp = new T[lenght];
            MergeSort_Recursive(items, temp, 0, lenght - 1);
        }
    }

    public class BubbleSort : ISorter {
        private int lenght;
        
        private void Swap<T>(ref T a, ref T b) where T : IComparable
        {
            (a, b) = (b, a);
        }
        
        ////////////////////////////////////////////
        public void Sort<T>(T[] source) where T : IComparable
        {
            lenght = source.Length;
            bool sorted = false;

            for (int iteration = 0; iteration <= lenght * lenght; iteration++) // Worst case is n^2
            {
                bool swapped = false;
                for (int i = 0; i < lenght - 1; i++)
                {
                    if (source [i]
                            .CompareTo(source[i + 1]) == 1)
                    {
                        Swap(ref source[i], ref source[i + 1]);
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    sorted = true;
                    break;
                }
            }
            if (!sorted)
                throw new ArgumentException("Too few iterations to sort array.");
        }
    }

    public class ShellSort : ISorter {
        private int lenght;
        public void Sort<T>(T[] array) where T : IComparable
        {
            lenght = array.Length;
            T temp;

            int increment = lenght / 2;
            while (increment > 0)
            {
                for (int i = 0; i < lenght; i++)
                {
                    int j = i;
                    temp = array[i];
                    while ((j >= increment) && (array [j - increment]
                                                    .CompareTo(temp) > 0))
                    {
                        array[j] = array[j - increment];
                        j -= increment;
                    }
                    array[j] = temp;
                }
                if (increment == 2)
                    increment = 1;
                else
                    increment *= 5 / 11;
            }
        }
        
    }


    public class SelectAlgorithm
    {
        
        public static void SelectSort<T>(T[] arr, bool is_infile) where T : IComparable
        {
            ISorter a;
            if (arr.Length <= 500)
            {
                a = new BubbleSort();
                Log.StartLog(arr, "BubbleSort", is_infile);
                a.Sort(arr);
                Log.EndLog(arr, is_infile);
            }
            else
            { // measure accuracy of sorted subarrays
                int sorted_cnt = 0;
                for (int i = 1; i < arr.Length; ++i)
                {
                    while (i < arr.Length && (arr [i]
                                                .CompareTo(arr[i - 1]) >= 0))
                    {
                        sorted_cnt++;
                        i++;
                    }
                }
                float accuracy = (float) sorted_cnt / (arr.Length - 1);
                if (accuracy > 0.5 && arr.Length < 200000)
                {
                    a = new ShellSort();
                    Log.StartLog(arr, "ShellSort", is_infile);
                    a.Sort(arr);
                    Log.EndLog(arr, is_infile);
                }
                else
                {
                    a = new MergeSort();
                    Log.StartLog(arr, "MergeSort", is_infile);
                    a.Sort(arr);
                    Log.EndLog(arr, is_infile);
                }
            }
        }
    }

    public class Program
    {
        // Generate of ints and strings have different algorithms(that's why function is not Generic)
        private static int[] generateArray(int count)
        {
            Random random = new Random();
            int[] values = new int[count];
            for (int i = 0; i < count; ++i)
                values[i] = random.Next();
            return values;
        }
        private static string[] generateArrayOfString(int count)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string[] values = new string[count];
            string randString = "";
            var random = new Random();
            for (int j = 0; j < count; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    randString += chars[random.Next(chars.Length)];
                }
                values[j] = randString;
                randString = "";
            }
            return values;
        }
        public static void Main()
        {
            Console.WriteLine("Where to write logs [f]ile/[c]onsole");
            string input = Console.ReadLine();
            bool is_infile = input[0] == 'f' ? true : false;

            File.Delete("logs.txt");
            string[] randomStrings1 = generateArrayOfString(400);
            SelectAlgorithm.SelectSort(randomStrings1, is_infile);
            string[] randomStrings2 = generateArrayOfString(50000);
            SelectAlgorithm.SelectSort(randomStrings2, is_infile);

            int[] randomNumbers1 = generateArray(80000);
            SelectAlgorithm.SelectSort(randomNumbers1, is_infile);
            int[] randomNumbers2 = generateArray(123200);
            SelectAlgorithm.SelectSort(randomNumbers2, is_infile);
            int[] randomNumbers3 = generateArray(1300000);
            SelectAlgorithm.SelectSort(randomNumbers3, is_infile);
            int[] randomNumbers4 = generateArray(6000000);
            SelectAlgorithm.SelectSort(randomNumbers4, is_infile);
        }
    }
}
