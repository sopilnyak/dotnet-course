using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using hw3;

public class BatchIteratorTest {
    [TestCase(10, 1)]
    [TestCase(10, 3)]
    [TestCase(10, 5)]
    [TestCase(10, 7)]
    [TestCase(100, 10)]
    [TestCase(100, 49)]
    [TestCase(100, 75)]
    [TestCase(1000, 1)]
    [TestCase(1000, 101)]
    [TestCase(1000, 500)]
    [TestCase(1000, 750)]
    [TestCase(1000000, 1001)]
    public void TestInt(int size, int batch_size)
    {
        int[] arr = new int[size];
        for(int j = 0; j <size; ++j)
        {
            arr[j] = j;
        }
        IEnumerable<int> enumerable = arr;
        var batch_iter = enumerable.ToBatchIterator(batch_size).GetEnumerator();
        int curr_idx = 0;

        foreach(var batch in batch_iter)
        {
            int curr_batch_size = 0;
            foreach(var item in batch)
            {
                Console.WriteLine(arr[curr_idx].ToString() + " " + item.ToString());
                Assert.AreEqual(arr[curr_idx++], item);
                curr_batch_size++;
            }
            Assert.True(curr_batch_size == batch_size || curr_idx == size);
        }
        Assert.AreEqual(curr_idx, size);
    }
}
