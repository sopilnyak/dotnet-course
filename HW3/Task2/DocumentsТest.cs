using System;
using NUnit.Framework;
using System.Collections.Generic;
using Task2;
using System.Linq;

namespace HW3.Task2.Tests
{
    public class DocumentSplitter
    {
        [TestCase(0.3)]
        [TestCase(0.8)]
        [TestCase(0.4)]
        [TestCase(0.5)]
        public void CreateList_Split_CorrectSplit(double trainSize)
        {
            var documents = new List<Document> {
                new Document(){Title = "Capture", CreatedUtc = new DateTime(2021, 10, 5, 1, 30, 3), ClassName = "Art"},
                new Document(){Title = "Football", CreatedUtc = new DateTime(2022, 11, 4, 1, 32, 5), ClassName = "Sport"},
                new Document(){Title = "Math", CreatedUtc = new DateTime(2021, 12, 6, 1, 33, 2), ClassName = "Science"},
                new Document(){Title = "Classic", CreatedUtc = new DateTime(2020, 3, 7, 6, 32, 3), ClassName = "Music"},
                new Document(){Title = "Basketball", CreatedUtc = new DateTime(2018, 1, 5, 6, 25, 4), ClassName = "Sport"},
                new Document(){Title = "Rock", CreatedUtc = new DateTime(2019, 2, 3, 3, 15, 4), ClassName = "Music"}
            };

            var (train_data, test_data) = Document.SplitTrainTest(documents, trainSize);

            int protected_trainSize = (int)(documents.Count * trainSize);
            int protected_testSize = documents.Count - protected_trainSize;
            var protected_elements = train_data.Concat(test_data);

            Assert.True(train_data.Count == protected_trainSize);
            Assert.True(test_data.Count == protected_testSize);
            Assert.True(documents.All(protected_elements.Contains));
        }
    }
}