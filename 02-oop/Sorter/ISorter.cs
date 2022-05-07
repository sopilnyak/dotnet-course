namespace OOP.Sorter;

public interface ISorter<T> where T : IComparable
{
    public void Sort(IList<T> array);
}