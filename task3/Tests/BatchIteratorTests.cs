using NUnit.Framework;
using System.Collections;

namespace Testing.Tests;

public class BatchIteratorTests
{
    [Test]
    public void Empty() {
        foreach(var batch in new BatchIterator<int>(new int[]{}, 1)) {
            Assert.Fail();
        };
        foreach(var batch in new BatchIterator<int>(new int[]{}, 10)) {
            Assert.Fail();
        };
    } 

    [Test]
    public void Undersize() {
        var sourceArray = new int[]{1, 2, 3};
        var batchIter = new BatchIterator<int>(sourceArray, 10);
        var batchEnumerator = batchIter.GetEnumerator();
        Assert.True(batchEnumerator.MoveNext());

        Assert.AreEqual(sourceArray, new List<int>(batchEnumerator.Current));
        Assert.False(batchEnumerator.MoveNext());
    }

    [Test]
    public void Exceptions() {
        Assert.Catch<ArgumentOutOfRangeException>(() => new BatchIterator<int>(new int[]{}, 0));
        Assert.Catch<ArgumentOutOfRangeException>(() => new BatchIterator<int>(new int[]{}, -1));
   }

    [Test]
    public void Simple() {
        var iter = new BatchIterator<int>((new int[] {0, 1, 2, 3, 4, 5, 6}), 2);
        int batchId = 0;
        foreach(var batch in iter) {
            int elemId = 0;
            foreach(int i in batch) {
                Assert.AreEqual(batchId * 2 + elemId, i);
                elemId += 1;
            }
            elemId = 0;
            foreach(int i in batch) {
                Assert.AreEqual(batchId * 2 + elemId, i);
                elemId += 1;
            }

            batchId += 1;
        }
    }

    [TestCase(new int[] { 0, 1, 2, 3, 4, 5, 6 }, 3)]
    [TestCase(new double[] { 0, 1, 2, 3, 4, 5, 6 }, 3)]
    [TestCase(new string[] { "0", "1", "2", "3", "4", "5", "6" }, 3)]
    public void DifferentTypes<T>(T[] values, int batchSize) {
        var iter = new BatchIterator<T>(values, batchSize).GetEnumerator();
        for (int i = 0; i < values.Count(); i += batchSize) {
            Assert.True(iter.MoveNext());
            var batch = iter.Current.GetEnumerator();
            var end = values.Count() < (i + 1) * batchSize ? values.Count() : (i + 1) * batchSize;

            for (int j = i * batchSize; j <  end; ++j) {
                Assert.True(batch.MoveNext());
                Assert.AreEqual(values[j], batch.Current);
            }
        }
    }

    
    class CheckIterator : IEnumerable<int> {
        public int available;
        int total;
        public CheckIterator(int total) {
            this.total = total;
            this.available = 0;
        }
        public void AllowNext(int cnt) {
            this.available += cnt;
        }

        public IEnumerator<int> GetEnumerator() {
            var total = this.total;
            while (total > 0 && this.available > 0) {
                this.available -= 1;
                total -= 1;
                yield return 0;
            }
            Assert.AreEqual(0, total);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }     
    }

    [Test]
    public void testLazyness() {
        var iter = new CheckIterator(100);
        iter.AllowNext(1);
        Assert.AreEqual(1, iter.available);

        foreach(var value in iter) {
            Assert.AreEqual(0, value);
            Assert.AreEqual(0, iter.available);

            iter.AllowNext(1);
            Assert.AreEqual(1, iter.available);
        }

        var batchIter = new BatchIterator<int>(iter, 7);

        Assert.AreEqual(1, iter.available);
        foreach(var batch in batchIter) {
            Assert.AreEqual(0, iter.available);
            foreach(var value in batch) {
                Assert.AreEqual(value, 0);
                Assert.AreEqual(0, iter.available);
                iter.AllowNext(1);
                Assert.AreEqual(1, iter.available);
            }

            Assert.AreEqual(1, iter.available);
            iter.AllowNext(-1);
            Assert.AreEqual(0, iter.available);
            foreach(var value in batch) {
                Assert.AreEqual(value, 0);
                Assert.AreEqual(0, iter.available);
            }

            iter.AllowNext(1);
        }
    }
}
