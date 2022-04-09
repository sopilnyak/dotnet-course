using NUnit.Framework;
using Homework3.BatchIterator;

namespace Homework3.Tests {
    public class BatchIteratorTests
    {
        [TestCase(new int[] {1, 2, 3, 4, 5, 6, 7, 8}, 4)]
        [TestCase(new int[] {1, 2, 3, 4, 5, 6, 7, 8}, 3)]
        [TestCase(new int[] {}, 3)]
        [TestCase(new int[] {1, 2, 3, 4, 5}, 1)]
        [TestCase(new int[] {1, 2, 3, 4, 5}, 5)]
        [TestCase(new int[] {1, 2, 3, 4, 5}, 7)]
        [TestCase(new string[] {"a", "aba", "abacaba", "abacabadaba"}, 3)]
        public void TestBasics<T>(T[] data_to_batch, uint batch_size)
        {
            var enumerator = data_to_batch.GetEnumerator();
            var batch_iterator = BatchIterator<T>(data_to_batch, batch_size).GetEnumerator();

            uint batch_iterator_steps = 0;
            while (batch_iterator_steps < data_to_batch.Length) {
                batch_iterator_steps++;
            }

            Assert.False(enumerator.MoveNext());
        }
    }
}