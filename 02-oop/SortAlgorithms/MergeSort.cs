using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
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
}