namespace Testing.hw3.Task1;

public class BatchIterator<T>
{
    private readonly IEnumerable<T> _data;
    private readonly int _batchSize;
    private readonly bool _dropLast;

    public BatchIterator(IEnumerable<T> data, int batchSize, bool dropLast = false)
    {
        this._data = data;
        this._batchSize = batchSize;
        this._dropLast = dropLast;
    }
    
    public IEnumerable<T[]> GetEnumerator()
    {
        T[] batch = new T[_batchSize];
        int i = 0;
        foreach (T item in _data)
        {
            batch[i++] = item;
            if (i == _batchSize)
            {
                yield return batch;
                batch = new T[_batchSize];
                i = 0;
            }
        }

        if (i > 0 && !_dropLast)
        {
            yield return batch[0..i];
        }

        yield break;
    }
}