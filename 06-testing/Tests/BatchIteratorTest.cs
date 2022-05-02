using NUnit.Framework;
namespace Testing.Tests;

public class BatchIteratorTester
{
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6 }, 5)]
    [TestCase(new string[] { "a", "aba", "abac", "abaca", "abacab", "abacaba" }, 2)]
    public void TestSequence<T>(T[] data, int batchsize)
    {
        var iterator = data.GetEnumerator();
        var myIterator = BatchIterator<T>.GetNextBatch(data, batchsize).GetEnumerator();
        var count = 0;

        while (myIterator.MoveNext())
        {
            var batch = myIterator.Current;
            foreach (T elem in batch)
            {
                Assert.True(iterator.MoveNext());
                Assert.AreEqual(iterator.Current, elem);
                count++;
            }
        }
        Assert.AreEqual(count, data.Length);
    }

    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 2)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 3)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 5)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 6)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 7)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 8)]
    [TestCase(new int[] { 1, 2, 3 }, 1)]
    [TestCase(new int[] { 1, 2, 3 }, 2)]
    [TestCase(new int[] { 1, 2, 3 }, 3)]
    public void TestBatchLen<T>(T[] data, int batchSize)
    {
        var iterator = data.GetEnumerator();
        var myIterator = BatchIterator<T>.GetNextBatch(data, batchSize).GetEnumerator();
        var iteration = 0;
        while (myIterator.MoveNext())
        {
            var batch = myIterator.Current;
            var batchLen = 0;
            foreach (T elem in batch)
            {
                Assert.True(iterator.MoveNext());
                Assert.AreEqual(iterator.Current, elem);
                batchLen++;
            }
            var expectedBatchLen = batchSize;
            if (batchSize * (iteration + 1) > data.Length)
            {
                expectedBatchLen = data.Length % batchSize;
            }
            Assert.AreEqual(batchLen, expectedBatchLen);
            iteration++;
        }

        Assert.AreEqual(iteration, (data.Length / batchSize) + (data.Length % batchSize == 0 ? 0 : 1));
    }
}
