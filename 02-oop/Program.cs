namespace OOP;

public interface ISorter<T> where T : IComparable
{
    public void Sort(IList<T> array);
}

internal enum AlgoName
{
    Selection,
    Quick
}

internal static class ComparatorDetails<T> where T : IComparable
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

public class BasicSorter<T> : ISorter<T> where T : IComparable
{
    private readonly StreamWriter? _writer;


    protected BasicSorter()
    {
        _writer = null;
    }

    protected BasicSorter(string filename)
    {
        _writer = new StreamWriter(filename);
    }

    private void WriteLine(string str)
    {
        if (_writer == null)
        {
            Console.WriteLine(str);
            return;
        }

        _writer.WriteLine(str);
    }

    public void Sort(IList<T> array)
    {
        var start = DateTime.Now;

        WriteLine($"Accepted array with size {array.Count}");

        var algo = ComparatorDetails<T>.ChooseAlgo(array);
        switch (algo)
        {
            case AlgoName.Selection:
                WriteLine("Selection sort was chosen");
                ComparatorDetails<T>.SelectionSort(array);
                break;
            case AlgoName.Quick:
                WriteLine("Quick sort was chosen");
                ComparatorDetails<T>.QuickSort(array);
                break;
        }

        var end = DateTime.Now;

        WriteLine("Sorted array successfully");
        WriteLine($"Duration: {end - start}");
    }
}

public class SorterLogConsole<T> : BasicSorter<T> where T : IComparable
{
    public SorterLogConsole() : base()
    {
    }
}

public class SorterLogFile<T> : BasicSorter<T> where T : IComparable
{
    public SorterLogFile(string filename) : base(filename)
    {
    }
}