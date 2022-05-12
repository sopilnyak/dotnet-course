using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainEx.Main.SplitTrainTest;
using NUnit.Framework;


namespace MainEx.Tests.TrainTestTests
{
    [TestFixture]
    public class TrainTestTest
    {
        [TestCase(0.8)]
        [TestCase(0.67)]
        [TestCase(0.5)]
        public void SplitTrainTest_10Docs_TrainTest(double partition)
        {
            TrainTestModel model = new TrainTestModel();
            List<Document> documents = new List<Document>();

            for (int i = 0; i < 10; i++)
            {
                Document d = new Document();
                d.Title = String.Format("doc{0}", i);
                d.ClassName = "1";
                d.CreatedUtc = new DateTime();

                documents.Add(d);

            }

            (List<Document>, List<Document>) resultTuple = TrainTestModel.SplitTrainTest(documents, partition);

            int trainSize = Convert.ToInt32(Math.Round(partition * 10));
            int testSize = 10 - trainSize;

            Assert.NotNull(resultTuple);
            Assert.NotNull(resultTuple.Item1);
            Assert.NotNull(resultTuple.Item2);
            Assert.AreEqual(trainSize, resultTuple.Item1.Count());
            Assert.AreEqual(testSize, resultTuple.Item2.Count());

        }
    }


}
