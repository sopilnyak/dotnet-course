using System;
using System.Collections.Generic;

namespace Testing.HW3
{
    public class BatchIterator<T>
    {
        private readonly IEnumerable<T> _bufferEnumerable;
        private readonly int _batchSize;
        private readonly bool _dropLast;

        public BatchIterator(IEnumerable<T> bufferEnumerable, int batchSize, bool dropLast = false)
        {
            this._bufferEnumerable = bufferEnumerable;
            this._batchSize = batchSize;
            this._dropLast = dropLast;
        }
    
        public IEnumerable<T[]> GetEnumerator()
        {
            var batch = new T[_batchSize];
            var i = 0;
            foreach (var item in _bufferEnumerable)
            {
                batch[i++] = item;
                if (i != _batchSize) continue;
                yield return batch;
                batch = new T[_batchSize];
                i = 0;
            }

            if (i > 0 && !_dropLast)
            {
                yield return new ArraySegment<T>(batch, 0, i).Array;
            }
        }
    }
}