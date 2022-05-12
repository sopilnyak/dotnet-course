using NUnit.Framework;
using HW3.Task1;
using Task1;
using System;

namespace HW3.Task1.Tests
{
    public class BatchIteratorTests
    {
        [TestCase(new int[] {}, 5)]
        [TestCase(new int[] {1}, 2)]
        [TestCase(new int[] { 12, 36, 22, 11, 5 }, 1)]
        [TestCase(new int[] { 8, 1, 4, 4, 4 }, 5)]
        [TestCase(new int[] { 9, 2, 65, 4, 5 }, 7)]
        [TestCase(new int[] { 12, 44, 33, 4, 4, 1, 7, 8, 5, 4 }, 7)]
        [TestCase(new int[] { 21, 78, 46, 4, 5, 6, 7, 8 }, 5)]
        [TestCase(new string[] { "Pablo", "Arsen", "Paulo", "Jordan", "Messi", "Leo" }, 4)]
        [TestCase(new string[] { "Ronaldo", "Ibrahimovich", "Casemiro", "Alba", "Mkhitaryan", "Hernandez" }, 10)]
        [TestCase(new string[] { "Tuchel", "Shevchenko", "Maldini", "Gvardiola", "Klop", "Emery", "Xavi" }, 3)]
        public void Divide_Array_CorrectBatches<T>(T[] arr, int batchSize)
        {
            BatchIterator<T> batchIterator = new BatchIterator<T>();
            var arr_iterator = arr.GetEnumerator();
            var batch_iterator = batchIterator.IterateBatches(arr, batchSize).GetEnumerator();
            int it = 0;
            batch_iterator.MoveNext();
            var batch = batch_iterator.Current;
            if(batch == null) {
                Assert.AreEqual(arr.Length, 0);   
            }
            while(it != arr.Length)
            {
                foreach (var item in batch)
                {
                    ++it;
                    arr_iterator.MoveNext();
                    Assert.AreEqual(arr_iterator.Current, item);
                }
                var current_batchSize = batch.Length;
                if (batch_iterator.MoveNext()) {
                    Assert.AreEqual(current_batchSize, batchSize);
                    batch = batch_iterator.Current;
                }
            }
            Assert.True(!arr_iterator.MoveNext());
            batch_iterator.Dispose();
        }
    }
}