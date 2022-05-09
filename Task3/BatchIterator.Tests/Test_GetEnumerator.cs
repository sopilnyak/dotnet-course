using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BatchIterator.Tests
{
    public class Tests
    {
        private List<string> names = new List<string>() { "Kirill", "Ana", "Felipe", "Sam", "Dany" };
        private int count;

        [SetUp]
        public void Setup()
        {
            count = 0;
        }

        [Test]
        public void Test_Batch_Sizes_Ar_Correct()
        {
            
            var batches = new BatchIterator<string>(names, 2);
            foreach (List<string> pair in batches)
            {
                Assert.True(pair.Count == (count < 4 ? 2: 1));

                count +=2;
            }
        }

        [Test]
        public void Test_GetEnumerator_Returns_Right_Data()
        {
            var batches = new BatchIterator<string>(names, 2);
            foreach (List<string> pair in batches)
            {
                Assert.True(pair[0] == names[count]);

                if (count < 4)
                {
                    Assert.True(pair[1] == names[count + 1]);
                }

                count += 2;
            }
        }
    }
}