using System;
using System.Collections.Generic;

namespace Homework3.BatchIterator
{
    public class BatchIterator<T> : IBatchIterator<T>
    {
        public int BatchSize { get; set; }

        public BatchIterator(int batch_size)
        {
            BatchSize = batch_size;
        }

        public IEnumerable<T[]> Iterate(IEnumerable<T> data_to_batch)
        {
            T[] batch = null;
            int current_size = 0;

            foreach (var item in data_to_batch)
            {
                if (batch == null)
                {
                    batch = new T[BatchSize];
                }

                batch[current_size++] = item;

                if (current_size == BatchSize)
                {
                    yield return batch;

                    batch = null;
                    current_size = 0;
                }
            }

            if (current_size > 0)
            {
                Array.Resize<T>(ref batch, current_size);
                yield return batch;
            }

            yield break;
        }
    }
}