namespace Testing { 
    public class BatchIterator<T>
    {
        private readonly IEnumerable<T> _data;
        private readonly int _size;

        public BatchIterator(IEnumerable<T> data, int size)
        {
            this._data = data;
            this._size = size;
        }

        public IEnumerable<T[]> GetEnumerator()
        {
            int batch_size = 0;
            T[] batch = new T[_size];
            
            foreach (T item in _data)
            {
                batch[batch_size] = item;
                batch_size++;

                if (batch_size == _size)
                {
                    yield return batch;
                    batch = new T[_size];
                    batch_size = 0;
                }
            }

            if (batch_size != 0)
            {
                yield return batch[0..batch_size];
            }

            yield break;
        }
    }
}