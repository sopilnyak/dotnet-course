using NUnit.Framework;
namespace Testing.Tests;

public class SplitTrainTester {
    [TestCase(0.2)]
    [TestCase(0.3)]
    [TestCase(0.7)]
    public void TestSplit(double train_len) {
        var docs = new Document[]{
            new Document("a", new DateTime(2022, 4, 24, 0, 0, 0), "name"),
            new Document("b", new DateTime(2022, 4, 24, 0, 0, 1), "name"),
            new Document("c", new DateTime(2022, 4, 24, 0, 0, 2), "name"),
            new Document("d", new DateTime(2022, 4, 24, 0, 0, 3), "name"),
            new Document("e", new DateTime(2022, 4, 24, 0, 0, 4), "name"),
            new Document("f", new DateTime(2022, 4, 24, 0, 0, 5), "name"),
            new Document("g", new DateTime(2022, 4, 24, 0, 0, 6), "name"),
            new Document("h", new DateTime(2022, 4, 24, 0, 0, 7), "name"),
            new Document("i", new DateTime(2022, 4, 24, 0, 0, 8), "name"),
            new Document("j", new DateTime(2022, 4, 24, 0, 0, 9), "name"),
        };

        var (train, test) = DocumentSplitter.SplitTrainTest(docs, train_len);
        Assert.AreEqual(Convert.ToInt32(Math.Floor(train_len * docs.Length)), train.Count);
        Assert.AreEqual(docs.Length - Convert.ToInt32(Math.Floor(train_len * docs.Length)), test.Count);
        var res = train.Concat(test).ToArray();
        
        Assert.True(res.Length == docs.Length);
        Assert.True(res.All(docs.Contains));
        Assert.True(docs.All(res.Contains));
    }
}
