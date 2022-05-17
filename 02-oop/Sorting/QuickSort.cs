namespace OOP.Sorting;

public class QuickSort<T> : ISort<T> where T : IComparable
{
    public void Sort(T[] items)
    {
        _QuickSort(items, 0, items.Length);
    }
    static void _QuickSort(T[] data, int left, int right)
    {
        int i = left - 1,
            j = right;

        while (true)
        {
            T d = data[left];
            do i++; while (data[i].CompareTo(d) < 0);
            do j--; while (data[j].CompareTo(d) > 0);

            if (i < j)
            {
                (data[i], data[j]) = (data[j], data[i]);
            }
            else
            {
                if (left < j)    _QuickSort(data, left, j);
                if (++j < right) _QuickSort(data, j, right);
                return;
            }
        }
    }
}