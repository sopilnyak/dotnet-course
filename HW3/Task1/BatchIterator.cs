using System;
using System.Collections.Generic;

namespace Task1
{

    public class BatchIterator<T>
    {
        public IEnumerable<T[]> IterateBatches(IEnumerable<T> arr, int batchSize) {
            
            T[] batchArr = new T[batchSize];
            int it = -1;
            foreach (var item in arr)
            {
                batchArr[++it] = item;
                if (it + 1 == batchSize)
                {
                    yield return batchArr;
                    it = -1;
                }
            }
            if (it >= 0)
            {
                Array.Resize<T>(ref batchArr, it + 1);
                yield return batchArr;
            }
        }
    }
}
