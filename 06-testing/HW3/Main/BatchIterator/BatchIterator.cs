using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainEx.Main.BatchIterator
{
    public class BatchIterator
    {
        public int Size { get; set; }

        public BatchIterator(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("Batch size should not be 0");
            }
            Size = size;
        }

        /**
         * input: collection that should be interated
         * output: the same collection iterated by batch
         */
        public IEnumerable<IEnumerable<U>> Batch<U>(IEnumerable<U> collection)
        {
            List<U> batch = new List<U>();

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection)
            {
                batch.Add(item);
                if (batch.Count == Size)
                {
                    yield return batch;
                    batch = new List<U>();
                }
            }

            if (batch.Count > 0) 
            {
                yield return batch;
            } 
        }

         /**
         * input: collection that should be interated
         * output: the same collection iterated by batch
         */
        public IEnumerable<IEnumerable<U>> Batch2<U>(IEnumerable<U> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            using (var iterator = collection.GetEnumerator())
            
            while (iterator.MoveNext())
            {
                var batch = new U[Size];
            
                batch[0] = iterator.Current;
                int count = 1;
                for (int i = 1; i < Size && iterator.MoveNext(); i++)
                {
                    batch[i] = iterator.Current;
                    count++;
                }
                if (count < Size)
                {
                    Array.Resize(ref batch, count);
                }
                yield return batch;
            }

            
        }

    }
}
