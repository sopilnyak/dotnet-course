using NUnit.Framework;
namespace Testing.Tests;

public class BatchIteratorTester {
    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 4)]
    [TestCase(new int[]{1, 2, 3, 4, 5, 6}, 5)]
    [TestCase(new string[]{"a", "aba", "abac", "abaca", "abacab", "abacaba"}, 2)]
    public void TestSequence<T>(T[] data, int batch_size) {
        var iterator = data.GetEnumerator();
        var my_iterator = BatchIterator<T>.GetNextBatch(data, batch_size).GetEnumerator();
        var count = 0;  

        while (my_iterator.MoveNext()) {
            var batch = my_iterator.Current;
            foreach (T elem in batch) {
                Assert.True(iterator.MoveNext());
                Assert.AreEqual(iterator.Current, elem);
                count++;
            }
        }
        Assert.AreEqual(count, data.Length);
    }

    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 1)]
    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 2)]
    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 3)]
    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 4)]
    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 5)]
    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 6)]
    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 7)]
    [TestCase(new int[]{1, 2, 3, 4, 5, 6, 7, 8}, 8)]
    [TestCase(new int[]{1, 2, 3}, 1)]
    [TestCase(new int[]{1, 2, 3}, 2)]
    [TestCase(new int[]{1, 2, 3}, 3)]
    public void TestBatchLen<T>(T[] data, int batch_size) {
        var iterator = data.GetEnumerator();
        var my_iterator = BatchIterator<T>.GetNextBatch(data, batch_size).GetEnumerator();
        var iteration = 0;
        while (my_iterator.MoveNext()) {
            var batch = my_iterator.Current;
            var batch_len = 0;
            foreach (T elem in batch) {
                Assert.True(iterator.MoveNext());
                Assert.AreEqual(iterator.Current, elem);
                batch_len++;
            }
            var expected_batch_len = batch_size;
            if (batch_size * (iteration + 1) > data.Length) {
                expected_batch_len = data.Length % batch_size;
            }
            Assert.AreEqual(batch_len, expected_batch_len);
            iteration++;
        }

        Assert.AreEqual(iteration, (data.Length / batch_size) + (data.Length % batch_size == 0 ? 0: 1));
    }
}
