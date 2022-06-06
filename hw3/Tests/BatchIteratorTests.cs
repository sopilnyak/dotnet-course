using NUnit.Framework;
using hw3.Task1;

namespace hw3.Tests; 

public class BatchIteratorTests {
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3)]
    [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 5)]
    [TestCase(new int[] { 1, 2, 3, 4, 5 }, 8)]
    [TestCase( "Hello, abyrvalg! What's up?", 5)] 
    public void BatchTest<T>(T[] data, int batch_size) {
        var data_iter = data.GetEnumerator();
        var batch_iter = BatchIterator<T>.NextBatch(data, batch_size).GetEnumerator();

        int cur_idx = 0;
        
        while (batch_iter.MoveNext()) {
            var batch = batch_iter.Current;

            foreach (var item in batch) {
                Assert.True(data_iter.MoveNext());
                Assert.AreEqual(data_iter.Current, item);
                cur_idx++;
            }
            
            Assert.True(batch.Length == batch_size || data.Length - cur_idx == batch.Length);
        }
        
        Assert.AreEqual(data.Length, cur_idx);
    }
}
