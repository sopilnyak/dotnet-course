using System;
using System.Collections.Generic;

namespace Homework3.Task1
{
    // Решил сделать единственный статичный метод, поэтому класс не шаблонный
    public static class BatchIterator
    {
        public static IEnumerable<T[]> IterateBatches<T>(
            this IEnumerable<T> data_to_batch, int batch_size)
        {
            T[] batch = null;
            int current_size = 0;

            foreach (var item in data_to_batch)
            {
                if (batch == null)
                {
                    batch = new T[batch_size];
                }

                batch[current_size++] = item;

                if (current_size == batch_size)
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