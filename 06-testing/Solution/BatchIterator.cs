using System.Collections;

namespace Testing.Solution;

public class BatchIterator<T> : IEnumerable<IEnumerable<T>>
{
    private readonly IEnumerable<T> _data;
    private readonly int _dataSize;
    private readonly int _batchSize;

    public BatchIterator(IEnumerable<T> data, int batchSize)
    {
        _data = data;
        _dataSize = _data.Count();
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