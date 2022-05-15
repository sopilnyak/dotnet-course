using NUnit.Framework;

namespace Testing;

public class BatchIteratorTest
{
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6 }, 3)] // Тест с кратным батчем
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6 }, 4)] // Тест с некратным батчем
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6 }, 7)] // Тест с огромным батчем
    [TestCase(new int[] { }, 3)]                  // Тест с пустым изначальным списком
    [TestCase(new String[] { "a", "b", "c",}, 2)] // Тест на другие типы
    [TestCase(new float[] { 1.3f, 1.8f, 1.23f }, 3)] // Тест на другие типы

    public void BatchTest<T>(IEnumerable<T> data, int batchSize)
    {
        var dataList = data.ToList();
        
        int dataIndex = 0;
        var iterator = new BatchIterator<T>(dataList, batchSize).GetEnumerator();

        foreach (var batch in iterator)
        {
            int thisBatchSize = 0;
            foreach (var batchItem in batch)
            {
                Assert.AreEqual(batchItem, dataList[dataIndex]); // Айтемы батча совпадают айтемами на их позиции из листа
                dataIndex++;
                thisBatchSize++;
            }
            Assert.True(thisBatchSize == batchSize || dataIndex == dataList.Count); // Либо считали батч размера batchSize, либо достигли края листа
        }
        Assert.AreEqual(dataIndex, dataList.Count); // Всё считали
    }
}