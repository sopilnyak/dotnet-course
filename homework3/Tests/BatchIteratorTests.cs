using NUnit.Framework;
using Homework3.Task1;

namespace Homework3.Task1Tests
{
    public class BatchIteratorTests
    {
        [TestCase(new int[] { 1, 2, 3, 4, 5 }, 1)]
        [TestCase(new int[] { 1, 2, 3, 4, 5 }, 5)]
        [TestCase(new int[] { 1, 2, 3, 4, 5 }, 7)]
        [TestCase(new int[] { }, 3)]
        [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4)]
        [TestCase(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 3)]
        [TestCase(new string[] { "a", "aba", "abacaba", "abacabadaba" }, 3)]
        public void TestBasics<T>(T[] data_to_batch, int batch_size)
        {
            var data_iterator = data_to_batch.GetEnumerator();
            var batch_iterator = data_to_batch
                                 .IterateBatches(batch_size)
                                 .GetEnumerator();

            if (!batch_iterator.MoveNext())
            {
                Assert.True(data_to_batch.Length == 0);
                return;
            }

            for (int steps_count = 0; steps_count < data_to_batch.Length; steps_count++)
            {
                var batch = batch_iterator.Current;

                foreach (var item in batch)
                {
                    Assert.True(data_iterator.MoveNext());
                    Assert.AreEqual(data_iterator.Current, item);
                }

                var current_batch_size = batch.Length;
                if (!batch_iterator.MoveNext())
                {
                    break;
                }
                else
                {
                    Assert.AreEqual(current_batch_size, batch_size);
                }
            }

            Assert.False(data_iterator.MoveNext());
        }
    }
}