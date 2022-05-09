using System.Collections;

namespace Testing.HW3;

public class BatchIterator<T> : IEnumerable<IEnumerable<T>>
{
    private readonly IEnumerable<T> _data;
    private readonly int _batchSize;
    private readonly bool _dropLast;

    public BatchIterator(IEnumerable<T> data, int batchSize, bool dropLast = true)
    {
        _data = data;
        _batchSize = batchSize;
        _dropLast = dropLast;
    }

    public IEnumerator<IEnumerable<T>> GetEnumerator()
    {
        var start = 0;
        while ((start + _batchSize < _data.Count()) ^ (!_dropLast & start < _data.Count()))
        {
            yield return _data.Skip(start).Take(_batchSize);
            start += _batchSize;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

