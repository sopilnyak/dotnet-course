namespace OOP.Sorting;

public class BubbleSort<T> : ISort<T> where T : IComparable
{
    public void Sort(T[] items)
    {
        for (int write = 0; write < items.Length; write++) {
            for (int sort = 0; sort < items.Length - 1; sort++) {
                if (items[sort].CompareTo(items[sort + 1]) > 0) {
                    (items[sort + 1], items[sort]) = (items[sort], items[sort + 1]);
                }
            }
        }
    }
}