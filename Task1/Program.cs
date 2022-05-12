using System;
using System.Collections;


//Реализовать логирование не получилось
public class ArraySort<T>
{

    public ArraySort(T[] array, Func<T, T, bool> Cmp)
    {
        this.array = array;
        comparing = Cmp;
        Console.WriteLine("The Array is covered and is ready for being sorted");
    }

    public T[] array { get; set; }  //array of any type
    public Func<T, T, bool> comparing { get; set; } //return True if first element is bigger, than second
    
    public void Sort()
    {
        if (array.Length < 1000)
        {
            Console.WriteLine("The Array will be sorted with BubbleStort");
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            BubbleStort();
            startTime.Stop();
            var resultTime = startTime.Elapsed;
            Console.WriteLine("\nThe Array was sorted in: {0} milliseconds", 
                String.Join(" ", resultTime.Milliseconds));
        }
        else
        {
            Console.WriteLine("The Array will be sorted with MergeSort");
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            MergeSort(0, array.Length - 1);
            startTime.Stop();
            var resultTime = startTime.Elapsed;
            Console.WriteLine("\nThe Array was sorted in: {0} milliseconds", 
                String.Join(" ", resultTime.Milliseconds));
        }
    }

    private void BubbleStort()
    {
        T temp;
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (comparing(array[i], array[j]))
                {
                    temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        }
    }

    private void MergeSort(int left, int right)
    {
        int mid;
        if (right > left)
        {
            mid = (right + left) / 2 + 1;
            MergeSort(left, mid - 1);
            MergeSort(mid, right);
            T[] temp = new T[array.Length];
            int i, left_end, num_elements, tmp_pos;
            left_end = mid - 1;
            tmp_pos = left;
            num_elements = (right - left + 1);
            while ((left <= left_end) && (mid <= right))
            {
                if (comparing(array[left], array[mid]))
                    temp[tmp_pos++] = array[mid++];
                else
                    temp[tmp_pos++] = array[left++];

            }
            while (left <= left_end)
                temp[tmp_pos++] = array[left++];
            while (mid <= right)
                temp[tmp_pos++] = array[mid++];
            for (i = 0; i < num_elements; i++)
            {
                array[right] = temp[right];
                right--;
            }
        }
    }
}

/// <summary>
/// /////////////////////////
/// </summary>


///Here I have a test
class Program
{
    static bool cmp(int a, int b)
    {
        return a > b;
    }
    private static void Main(string[] args)
    {
        Random rnd = new Random();
        int[] arr = new int[2000];
        for (int i = 0; i < 2000; ++i)
        {
            arr[i] = rnd.Next(10);
        }
        //Console.WriteLine("\nМассив: {0}", String.Join(" ", arr));
        ArraySort<int>  cover = new ArraySort<int>(arr, cmp);
        cover.Sort();
        Console.WriteLine("\nМассив: {0}", String.Join(" ", arr));
    }
}


