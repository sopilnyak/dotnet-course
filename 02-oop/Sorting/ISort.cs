namespace OOP.Sorting;

public interface ISort<T> where T : IComparable
{
    void Sort(T[] items);
}