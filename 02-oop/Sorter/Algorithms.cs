namespace OOP.Sorter;

internal static class Algorithms<T> where T : IComparable
{
    internal static AlgoName ChooseAlgo(IList<T> array)
    {
        return array.Count < 1000 ? AlgoName.Selection : AlgoName.Quick;
    }

    private static int Partition(IList<T> array, int left, int right)
    {
        var pivot = array[right];
        var i = left - 1;

        for (var j = left; j < right; ++j)
        {
            if (array[j].CompareTo(pivot) >= 0) continue;

            ++i;
            (array[i], array[j]) = (array[j], array[i]);
        }

        (array[i + 1], array[right]) = (array[right], array[i + 1]);
        return i + 1;
    }

    private static void RecursiveQuickSort(IList<T> array, int left, int right)
    {
        if (left >= right) return;

        var pi = Partition(array, left, right);
        RecursiveQuickSort(array, left, pi - 1);
        RecursiveQuickSort(array, pi + 1, right);
    }

    internal static void QuickSort(IList<T> array)
    {
        RecursiveQuickSort(array, 0, array.Count - 1);
    }

    internal static void SelectionSort(IList<T> array)
    {
        for (var i = 0; i < array.Count; ++i)
        {
            var minInd = i;

            for (var j = i + 1; j < array.Count; ++j)
            {
                if (array[j].CompareTo(array[i]) < 0)
                {
                    minInd = j;
                }
            }

            if (minInd != i)
            {
                (array[i], array[minInd]) = (array[minInd], array[i]);
            }
        }
    }
}