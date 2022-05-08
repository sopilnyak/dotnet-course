using NUnit.Framework;
using Testing.hw3.Task1;

namespace Testing.hw3.Tests;

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
        var batchIter = new BatchIterator<T>(enumerable, batchSize, dropLast).GetEnumerator();
        int i = 0;
            
        foreach(var batch in batchIter)
        {
            int size = 0;
            foreach(var item in batch)
            {
                Console.WriteLine(enumerable[i]?.ToString() + " " + item?.ToString());
                Assert.AreEqual(enumerable[i++], item);
                size++;
            }
            Assert.True(size == batchSize || i == size);
        }
        Assert.AreEqual(i, enumerable.Count);
    }
}
