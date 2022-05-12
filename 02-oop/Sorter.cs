using System;

public interface ISorter<ArrayItemType> where ArrayItemType : IComparable 
{
    public bool CheckFit(ArrayItemType[] array); 
    public void Sort(ArrayItemType[] array);
    public string GetAlgorithmName { get; }
}


public class ComplexSorter<ArrayItemType> where ArrayItemType : IComparable
{
    private ISorter<ArrayItemType>[] _sorters;
    private ILogger _logger;

    public ComplexSorter(ISorter<ArrayItemType>[] sorters, ILogger logger)
    {
        _sorters = sorters;
        _logger = logger;
    }

    public void Sort(ArrayItemType[] array)
    {
        _logger.Log($"Получен массив из {array.Length} элементов");
        foreach (var sorter in _sorters)
        {
            if (sorter.CheckFit(array))
            {
                _logger.Log($"Выбран алгоритм {sorter.GetAlgorithmName}");
                
                var watch = System.Diagnostics.Stopwatch.StartNew();
                sorter.Sort(array);
                watch.Stop();

                _logger.Log($"Отсортировано за {watch.ElapsedMilliseconds} времени");

                break;
            }
        }
    }
}

public class QuickSort<ArrayItemType> : ISorter<ArrayItemType> where ArrayItemType : IComparable
{
    public string GetAlgorithmName => "Quick_Sort";

    public bool CheckFit(ArrayItemType[] array) => (array.Length >= 20);

    private void SortFromTo(int from, int to, ArrayItemType[] array)
    {
        if (from < to)
        {
            var mid = from + 1;
            for (var midPos = from + 1; midPos < to; midPos++)
            {
                if (array[midPos].CompareTo(array[from]) <= 0)
                {
                    (array[midPos], array[mid]) = (array[mid++], array[midPos]);
                }
            }
            (array[mid - 1], array[from]) = (array[from], array[mid - 1]);

            SortFromTo(from, mid - 1, array);
            SortFromTo(mid, to, array);
        }
    }

    public void Sort(ArrayItemType[] array)
    {
        SortFromTo(0, array.Length, array);
    }
}

public class BubbleSort<ArrayItemType> : ISorter<ArrayItemType> where ArrayItemType : IComparable
{
    public string GetAlgorithmName => "Bubble_Sort";

    public bool CheckFit(ArrayItemType[] array) => (array.Length <= 100);

    public void Sort(ArrayItemType[] array)
    {
        var to = array.Length - 1;
        bool isSorted = false;
        while (!isSorted)
        {
            isSorted = true;
            for (int pairPos = 0; pairPos < to; pairPos++)
            {
                if (array[pairPos + 1].CompareTo(array[pairPos]) < 0)
                {
                    isSorted = false;
                    (array[pairPos], array[pairPos + 1]) = (array[pairPos + 1], array[pairPos]);   
                }
            }
        }
    }
}