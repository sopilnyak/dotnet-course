using OOP.Logger;

namespace OOP.Sorter;

public class Sorter<T> : ISorter<T> where T : IComparable
{
    private readonly ILogger _logger;

    public Sorter()
    {
        _logger = new ConsoleLogger();
    }

    public Sorter(string filename)
    {
        _logger = new FileLogger(filename);
    }

    public Sorter(ILogger logger)
    {
        _logger = logger;
    }

    public void Sort(IList<T> array)
    {
        var start = DateTime.Now;

        _logger.WriteLine($"Accepted array with size {array.Count}");

        var algo = Algorithms<T>.ChooseAlgo(array);
        switch (algo)
        {
            case AlgoName.Selection:
                _logger.WriteLine("Selection sort was chosen");
                Algorithms<T>.SelectionSort(array);
                break;
            case AlgoName.Quick:
                _logger.WriteLine("Quick sort was chosen");
                Algorithms<T>.QuickSort(array);
                break;
            default:
                _logger.WriteLine("Internal error: could not choose algorithm");
                return;
        }

        var end = DateTime.Now;

        _logger.WriteLine("Sorted array successfully");
        _logger.WriteLine($"Duration: {end - start}");
    }
}