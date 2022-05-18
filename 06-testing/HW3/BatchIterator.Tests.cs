using NUnit.Framework;

namespace Testing.HW3;

public class TestBatchIterator
{
    private static int CalcBatchSize(int dataSize, int batchSize, int batchIndex)
    {
        if (batchSize * (batchIndex + 1) < dataSize)
        {
            return batchSize;
        }

        return dataSize - batchSize * batchIndex;
    }

    private static int CalcElementIndex(int batchSize, int batchIndex, int batchElementIndex)
    {
        return batchSize * batchIndex + batchElementIndex;
    }

    [TestCase(new[] { 1, 2, 3, 4 }, 2, true)]
    [TestCase(new[] { 1, 2, 3, 4 }, 2, false)]
    [TestCase(new[] { 1, 2, 3, 4, 5, 6, 7 }, 3, true)]
    [TestCase(new[] { 1, 2, 3, 4, 5, 6, 7 }, 3, true)]
    [TestCase(new[] { 1, 2, 3, 4 }, 5, true)]
    [TestCase(new[] { 1, 2, 3, 4 }, 5, false)]
    [TestCase(new int[] { }, 3, true)]
    [TestCase(new int[] { }, 3, false)]
    [TestCase(new [] { "1", "2", "3", "4", "5" }, 2, true)]
    [TestCase(new [] { "1", "2", "3", "4", "5" }, 2, false)]
    public void BatchSizeTest<T>(IEnumerable<T> data, int batchSize, bool dropLast)
    {
        var dataList = data.ToList();
        var batchIterator = new BatchIterator<T>(dataList, batchSize, dropLast);
        var batchIndex = 0;

        foreach (var batch in batchIterator)
        {
            Assert.AreEqual(
                CalcBatchSize(dataList.Count, batchSize, batchIndex),
                batch.Count());
            ++batchIndex;
        }
    }

    [TestCase(new[] { 1, 2, 3, 4 }, 2, true)]
    [TestCase(new[] { 1, 2, 3, 4 }, 2, false)]
    [TestCase(new[] { 1, 2, 3, 4, 5 }, 2, true)]
    [TestCase(new[] { 1, 2, 3, 4, 5 }, 2, true)]
    [TestCase(new[] { 1, 2, 3, 4 }, 5, true)]
    [TestCase(new[] { 1, 2, 3, 4 }, 5, false)]
    [TestCase(new int[] { }, 3, true)]
    [TestCase(new int[] { }, 3, false)]
    [TestCase(new [] { "1", "2", "3", "4", "5" }, 2, true)]
    [TestCase(new [] { "1", "2", "3", "4", "5" }, 2, false)]
    public void BatchDataTest<T>(IEnumerable<T> data, int batchSize, bool dropLast)
    {
        var dataList = data.ToList();
        var batchIterator = new BatchIterator<T>(dataList, batchSize, dropLast);
        var batchIndex = 0;

        foreach (var batch in batchIterator)
        {
            var batchList = batch.ToList();
            var batchElementIndex = 0;
            foreach (var elem in batchList)
            {
                Assert.AreEqual(
                    dataList[CalcElementIndex(batchSize, batchIndex, batchElementIndex)],
                    elem);
                ++batchElementIndex;
            }

            ++batchIndex;
        }
    }
    
    [TestCase(new[] { 1, 2}, 0, true)]
    [TestCase(new[] { 1, 2}, 0, false)]
    [TestCase(new[] { 1, 2}, -1, true)]
    [TestCase(new[] { 1, 2}, -1, false)]
    [TestCase(new int[] {}, 0, true)]
    [TestCase(new int[] {}, 0, false)]
    [TestCase(new int[] {}, -1, true)]
    [TestCase(new int[] {}, -1, false)]
    public void InvalidBatchSizeTest<T>(IEnumerable<T> data, int batchSize, bool dropLast)
    {
        var dataList = data.ToList();
        Assert.Throws<ArgumentException>(() => new BatchIterator<T>(dataList, batchSize, dropLast));
    }
}