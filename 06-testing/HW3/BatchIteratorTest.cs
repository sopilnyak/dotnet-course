using System;
using System.Collections.Generic;
using System.Linq;
using Testing.HW3;
using NUnit.Framework;

namespace Testing.HW3
{
    public class BatchIteratorTest
    {
        [TestCase(new int[] {1, 2, 3, 4, 5, 6, 7, 8}, 4, true)]
        [TestCase(new int[] {1, 2, 3, 4, 5, 6, 7, 8}, 3, false)]
        [TestCase(new int[] {1, 2, 3, 4, 5, 6, 7, 8}, 3, true)]
        [TestCase(new int[] {}, 3, true)]
        [TestCase(new int[] {1, 2, 3, 4, 5, 6, 7, 8}, 9, true)]
        [TestCase(new int[] {1, 2, 3, 4, 5, 6, 7, 8}, 1, true)]
        [TestCase(new String[] {"boogy-woogy", "Arra", "a", "aboba",  "dsfdgas"}, 3, false)]
        public void BatchTest<T>(IEnumerable<T> data, int batchSize, bool dropLast)
        {
            var enumerable = data.ToList();
            if (dropLast == false && enumerable.Count % batchSize != 0)
            {
                enumerable.AddRange(new T[batchSize - enumerable.Count % batchSize]);
            } else if (dropLast == true && enumerable.Count % batchSize != 0)
            {
                enumerable = enumerable.Take(enumerable.Count - enumerable.Count % batchSize).ToList();
            }
            var batchIter = new BatchIterator<T>(enumerable, batchSize, dropLast).GetEnumerator();
            var i = 0;
            
            foreach(var batch in batchIter)
            {
                var size = 0;
                foreach(var item in batch)
                {
                    Assert.AreEqual(enumerable[i++], item);
                    size++;
                }
                Assert.True(size == batchSize || i == size);
            }
            Assert.AreEqual(i, enumerable.Count);
        }
    }
}