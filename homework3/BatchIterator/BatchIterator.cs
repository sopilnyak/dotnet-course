using System;
using System.Collections.Generic;

namespace Homework3.BatchIterator
{
    public class BatchIterator<T> : IBatchIterator<T>
    {
        public IEnumerable<T> DataToBatch { get; set; }
        public uint BatchSize { get; set; }

        public BatchIterator(IEnumerable<T> data_to_batch, uint batch_size)
        {
            DataToBatch = data_to_batch;
            BatchSize = batch_size;
        }

        public IEnumerator<T[]> GetEnumerator()
        {
            T[] batch;
            uint current_size = 0;

            foreach (var item in DataToBatch)
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
                Array.Resize<T>(batch, current_size);
                yield return batch;
            }

            yield break;
        }
    }
}