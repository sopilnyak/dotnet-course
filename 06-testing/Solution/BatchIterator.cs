using System.Collections;

namespace Testing.Solution;

public class BatchIterator<T> : IEnumerable<IEnumerable<T>>
{
    private readonly IList<T> _data;
    private readonly int _dataSize;
    private readonly int _batchSize;

    public BatchIterator(IEnumerable<T> data, int batchSize)
    {
        if (batchSize <= 0)
        {
            throw new ArgumentException("Failed to create BatchIterator: batch size must be greater than 0");
        }

        _data = data.ToList();
        _dataSize = _data.Count;
        _batchSize = batchSize;
    } 

    public IEnumerator<IEnumerable<T>> GetEnumerator()
    {
        for (var start = 0; start < _dataSize; start += _batchSize)
        {
            yield return _data.Skip(start).Take(_batchSize);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}