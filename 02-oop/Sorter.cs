using System.Diagnostics;
using util;

namespace OOP;

public class Sorter<T> where T : IComparable<T>
{
    private readonly IList<T> _listRef;
    private readonly ILogger _logger;
    private readonly Stopwatch _sw;

    public Sorter(IList<T> list, ILogger logger)
    {
        _listRef = list;
        _logger = logger;
        _sw = new Stopwatch();

        _logger.WriteLine("Array size : " + list.Count);
    }

    public void Sort()
    {
        _sw.Start();

        switch (ChooseAlgorithm())
        {
            case Algorithm.Bubble:
                _logger.WriteLine("Bubble sort algorithm is chosen");
                BubbleSort<T>.Sort(_listRef);
                break;

            case Algorithm.Merge:
                _logger.WriteLine("Merge sort algorithm is chosen");
                MergeSort<T>.Sort(_listRef);
                break;

            default:
                Debug.Assert(false, "Unknown sorting algorithm");
                break;
        }

        _sw.Stop();
        _logger.WriteLine("Elapsed time = " + _sw.Elapsed);
    }

    private Algorithm ChooseAlgorithm()
    {
        const uint maxCountForQuadAlg = 1000;

        return _listRef.Count < maxCountForQuadAlg ? Algorithm.Bubble : Algorithm.Merge;
    }

    private enum Algorithm
    {
        Bubble,
        Merge
    }
}

internal static class BubbleSort<T> where T : IComparable<T>
{
    public static void Sort(IList<T> list)
    {
        for (var i = 0; i < list.Count - 1; ++i)
        for (var j = 0; j < list.Count - 1 - i; ++j)
            if (list[j].CompareTo(list[j + 1]) == 1)
                Swap(list, j, j + 1);
    }

    private static void Swap(IList<T> list, int first, int second)
    {
        (list[first], list[second]) = (list[second], list[first]);
    }
}

internal static class MergeSort<T> where T : IComparable<T>
{
    public static void Sort(IList<T> list)
    {
        SortSubsection(list, 0, list.Count - 1);
    }

    private static void SortSubsection(IList<T> list, int begin, int end)
    {
        if (begin >= end) return;

        var mid = begin + (end - begin) / 2;
        SortSubsection(list, begin, mid);
        SortSubsection(list, mid + 1, end);
        Merge(list, begin, mid, end);
    }

    private static void Merge(IList<T> list, int begin, int mid, int end)
    {
        var leftSubsectionCount = mid - begin + 1;
        var rightSubsectionCount = end - mid;

        var leftSubsection = new T[leftSubsectionCount];
        var rightSubsection = new T[rightSubsectionCount];

        for (var i = 0; i < leftSubsectionCount; ++i) leftSubsection[i] = list[begin + i];

        for (var i = 0; i < rightSubsectionCount; ++i) rightSubsection[i] = list[mid + 1 + i];

        var leftIndex = 0;
        var rightIndex = 0;
        var globalIndex = begin;
        while (leftIndex < leftSubsectionCount && rightIndex < rightSubsectionCount)
        {
            if (leftSubsection[leftIndex].CompareTo(rightSubsection[rightIndex]) == -1)
            {
                list[globalIndex] = leftSubsection[leftIndex];
                ++leftIndex;
            }
            else
            {
                list[globalIndex] = rightSubsection[rightIndex];
                ++rightIndex;
            }

            ++globalIndex;
        }

        while (leftIndex < leftSubsectionCount)
        {
            list[globalIndex] = leftSubsection[leftIndex];
            ++leftIndex;
            ++globalIndex;
        }

        while (rightIndex < rightSubsectionCount)
        {
            list[globalIndex] = rightSubsection[rightIndex];
            ++rightIndex;
            ++globalIndex;
        }
    }
}