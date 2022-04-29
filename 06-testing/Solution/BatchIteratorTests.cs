using NUnit.Framework;

namespace Testing.Solution;

public class BatchIteratorTests
{
    private static int CurrBatchSize(int dataSize, int batchSize, int iterNum)
    {
        if (batchSize * (iterNum + 1) < dataSize)
        {
            return batchSize;
        }

        return dataSize - batchSize * iterNum;
    }
    
    private static int CurrIndex(int batchSize, int outerIterNum, int innerIterNum)
    {
        return batchSize * outerIterNum + innerIterNum;
    }

    [TestCase(new [] {1, 2, 3, 4}, 2)]
    [TestCase(new [] {1, 2, 3, 4, 5, 6, 7, 8}, 10)]
    [TestCase(new [] {1, 2, 3, 4, 5}, 3)]
    [TestCase(new [] {1}, 1)]
    [TestCase(new int[] {}, 4)]
    [TestCase(new [] {"a", "b", "c"}, 1)]
    [TestCase(new [] {"a", "b", "c", "d", "e"}, 2)]
    public void BatchIteratorTest<T>(IEnumerable<T> data, int batchSize)
    {
        var dataList = data.ToList();

        var iterator = new BatchIterator<T>(dataList, batchSize);
        var outerIterNum = 0;
        
        foreach (var batch in iterator)
        {
            var batchList = batch.ToList();

            Assert.AreEqual(CurrBatchSize(dataList.Count, batchSize, outerIterNum), batchList.Count);

            var innerIterNum = 0;
            foreach (var elem in batchList)
            {
                Assert.AreEqual(dataList.ElementAt(CurrIndex(batchSize, outerIterNum, innerIterNum)), elem);

                innerIterNum++;
            }

            outerIterNum++;
        }
    }
}

