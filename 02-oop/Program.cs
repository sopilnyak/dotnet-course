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

namespace Homework1
{
public class Log
{
    private static Stopwatch stopWatch;
    public static void StartLog<T>(T[] items, string algo_name)
    {
        Console.WriteLine($"получен массив из {items.Length} элементов");
        Console.WriteLine($"выбран алгоритм {algo_name}");
        stopWatch = new Stopwatch();
        stopWatch.Start();
    }
    public static void EndLog<T>(T[] items)
    {
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime =
            String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        Console.WriteLine($"отсортировано за {elapsedTime} времени");
        Console.Write('\n');
    }
}
public class SortAlgorithms
{
    private static int length;
    //////////////////////////////////////////
    private static void Swap<T>(ref T a, ref T b) where T : IComparable
    {
        T t = a;
        a = b;
        b = t;
    }
    private static void MergeSort_Recursive<T>(T[] numbers, T[] temp, int left, int right) where T : IComparable
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
    private static void DoMerge<T>(T[] numbers, T[] temp, int left, int mid, int right) where T : IComparable
    {
        // Console.WriteLine("cp2 [" + left + ", " + right + "]");
        int left_end, num_elements, tmp_pos;

        left_end = (mid - 1);
        tmp_pos = left;
        num_elements = (right - left + 1);

        while ((left <= left_end) && (mid <= right))
        {
            if (numbers [left]
                    .CompareTo(numbers[mid]) <= 0)
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

    private static void MergeSort<T>(T[] items) where T : IComparable
    {
        length = items.Length;
        T[] temp = new T[length];
        MergeSort_Recursive(items, temp, 0, length - 1);
    }
    ////////////////////////////////////////////
    private static void BubbleSort<T>(T[] source) where T : IComparable
    {
        length = source.Length;
        bool sorted = false;

        for (int iteration = 0; iteration <= length * length; iteration++) // Worst case is n^2
        {
            bool swapped = false;
            for (int i = 0; i < length - 1; i++)
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
    //////////////////////////////////////////
    private static void ShellSort<T>(T[] array) where T : IComparable
    {
        int i, j, increment;
        length = array.Length;
        T temp;

        increment = length / 2;
        while (increment > 0)
        {
            for (i = 0; i < length; i++)
            {
                j = i;
                temp = array[i];
                while ((j >= increment) && (array [j - increment]
                                                .CompareTo(temp) > 0))
                {
                    array[j] = array[j - increment];
                    j = j - increment;
                }
                array[j] = temp;
            }
            if (increment == 2)
                increment = 1;
            else
                increment = increment * 5 / 11;
        }
    }
    public static void SelectSort<T>(T[] arr) where T : IComparable
    {
        if (arr.Length <= 500)
        {
            Log.StartLog(arr, "BubbleSort");
            BubbleSort(arr);
            Log.EndLog(arr);
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
                Log.StartLog(arr, "ShellSort");
                ShellSort(arr);
                Log.EndLog(arr);
            }
            else
            {
                Log.StartLog(arr, "MergeSort");
                MergeSort(arr);
                Log.EndLog(arr);
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
        string[] randomStrings1 = generateArrayOfString(400);
        SortAlgorithms.SelectSort(randomStrings1);
        string[] randomStrings2 = generateArrayOfString(50000);
        SortAlgorithms.SelectSort(randomStrings2);

        int[] randomNumbers1 = generateArray(80000);
        SortAlgorithms.SelectSort(randomNumbers1);
        int[] randomNumbers2 = generateArray(123200);
        SortAlgorithms.SelectSort(randomNumbers2);
        int[] randomNumbers3 = generateArray(1300000);
        SortAlgorithms.SelectSort(randomNumbers3);
        int[] randomNumbers4 = generateArray(6000000);
        SortAlgorithms.SelectSort(randomNumbers4);
    }
}
}
