using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainEx.Main.BatchIterator;
using NUnit.Framework;

namespace MainEx.Tests.BatchIteratorTests
{
    [TestFixture]
    public class BatchIteratorTest
    {

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void BatchIterator_InputIsZero_ReturnException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                new BatchIterator(0);
                }); ;
        }

        [Test]
        public void BatchIterator_InputValid_NoException()
        {
            Assert.NotNull(new BatchIterator(5));
        }

        [Test]
        public void Batch_InputSmallList_ReturnEnumerable()
        {
            BatchIterator batchIterator = new BatchIterator(2);
            List<int> testList = new List<int> {1, 2, 3, 4 };

            IEnumerable<IEnumerable<int>> batches = batchIterator.Batch(testList);

            Assert.NotNull(batches);
            Assert.AreEqual(2, batches.Count());
        }

        [Test]
        public void Batch_BatchSizeEqualListSize_SingleBatch()
        {
            BatchIterator batchIterator = new BatchIterator(4);
            List<int> testList = new List<int> { 1, 2, 3, 4 };

            IEnumerable<IEnumerable<int>> batches = batchIterator.Batch(testList);

            Assert.NotNull(batches);
            Assert.AreEqual(1, batches.Count());

        }

        [Test]
        public void Batch_BatchSizeGraterThanListSize_SingleBatch()
        {
            BatchIterator batchIterator = new BatchIterator(10);
            List<int> testList = new List<int> { 1, 2, 3, 4 };

            IEnumerable<IEnumerable<int>> batches = batchIterator.Batch(testList);

            Assert.AreEqual(1, batches.Count());

        }

        [Test]
        public void Batch_BigList()
        {
            int size = 100000;
            int batchSize = 10;

            BatchIterator batchIterator = new BatchIterator(batchSize);
            List<int> bigList = GetRandomList(size);

            IEnumerable<IEnumerable<int>> batches = batchIterator.Batch(bigList);

            Assert.AreEqual(size/batchSize, batches.Count());
        }

        [Test]
        public void Batch_BigListOfObjects()
        {
            int size = 10000000;
            int batchSize = 10;

            BatchIterator batchIterator = new BatchIterator(batchSize);
            List<object> bigList = GetRandomObjectList(size);

            IEnumerable<IEnumerable<object>> batches = batchIterator.Batch(bigList);

            Assert.AreEqual(size / batchSize, batches.Count());
        }

        [Test]
        public void Batch2_BigListOfObjects()
        {
            int size = 10000000;
            int batchSize = 10;

            BatchIterator batchIterator = new BatchIterator(batchSize);
            List<object> bigList = GetRandomObjectList(size);

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            IEnumerable<IEnumerable<object>> batches = batchIterator.Batch2(bigList);
            watch.Stop();

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

            Assert.AreEqual(size / batchSize, batches.Count());
        }



        private List<int> GetRandomList(int size)
        {
            var rand = new Random();
            var rtnlist = new List<int>();

            for (int i = 0; i < size; i++)
            {
                rtnlist.Add(rand.Next(1000));
            }
            return rtnlist;

        }

        private List<object> GetRandomObjectList(int size)
        {
            var rand = new Random();
            var rtnlist = new List<object>();

            for (int i = 0; i < size; i++)
            {

                rtnlist.Add(new {Val = rand.Next(1000) });
            }
            return rtnlist;
        }
    }
}
