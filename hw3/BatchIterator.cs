using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw3
{
    public class BatchIterator<T>
    {
        private IEnumerable<T> data;
        private int batch_size;
        public BatchIterator(IEnumerable<T> data, int batch_size)
        {
            this.data = data;
            this.batch_size = batch_size;
        }

        public IEnumerable<T[]> GetEnumerator()
        {
            T[] batch = new T[batch_size];
            int count = 0;
            foreach (T item in data)
            {
                batch[count++] = item;
                if (count == batch_size)
                {
                    yield return batch;
                    batch = new T[batch_size];
                    count = 0;
                }
            }
            if (count > 0)
            {
                yield return batch[0..count];
            }
            yield break;
        }


    }
    public static class BatchIteratorGetter
    {
        public static BatchIterator<T> ToBatchIterator<T>(this IEnumerable<T> data, int batch_size)
        {
            return new BatchIterator<T>(data, batch_size);
        }
    }
}


