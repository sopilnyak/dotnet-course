using System;
using NUnit.Framework;

using Homework3.Task2;
using static Homework3.Task2.DocumentSplitter;


namespace Homework3.Task2Tests
{
    public class DocumentSplitter
    {
        [TestCase(0.5)]
        public void TestBasics(double train_proportion)
        {
            var documents = new List<Document> {
                new Document("first", new DateTime(2007, 7, 7, 7, 47, 0), "class name"),
                new Document("second", new DateTime(2007, 7, 7, 7, 47, 1), "class name"),
                new Document("third", new DateTime(2007, 7, 7, 7, 47, 2), "class name"),
                new Document("third_1", new DateTime(2007, 7, 7, 7, 47, 2), "class name")
            };
            var (train, test) = SplitTrainTest(documents, train_proportion);

            int expected_train_size = Convert.ToInt32(Math.Floor(documents.Count * train_proportion));
            Assert.AreEqual(train.Count, expected_train_size);

            int expected_test_size = documents.Count - expected_train_size;
            Assert.AreEqual(test.Count, expected_test_size);

            var returned_elements = train.Concat(test).ToList();
            Assert.True(documents.All(returned_elements.Contains));
        }
    }
}