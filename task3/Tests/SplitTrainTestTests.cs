using NUnit.Framework;
using System.Collections;

namespace Testing.Tests;

public class SplitTrainTestTests 
{
    [Test]
    public void Empty() {
        //var docs = new LIst<Document>{ new Document{"a", new DateTime(2022, 04, 16))}};
        var docs = new List<Document>();

        (var train, var test) = Document.SplitTrainTest(docs, 0);
        Assert.AreEqual(0, train.Count);
        Assert.AreEqual(0, train.Count);
    }

    [Test]
    public void JustWork() {
        var docs = new List<Document>();
        for(int i = 0; i < 100; ++i) {
            docs.Add(new Document("a", new DateTime(2022 + i, 04, 16), "b"));
        }

        var data = Document.SplitTrainTest(docs, 0.5);

        Assert.NotZero(data.test.Count());
        Assert.NotZero(data.train.Count());
        Assert.AreEqual(100, data.train.Count() + data.test.Count());

        foreach(var item in data.test) {
            Assert.True(data.test.Contains(item));
            Assert.False(data.train.Contains(item));
        }
    }

    [Test]
    public void Stability() {
        var docs = new List<Document>();
        for(int i = 0; i < 100; ++i) {
            docs.Add(new Document("a", new DateTime(2022 + i, 04, 16), "b"));
        }

        var data1 = Document.SplitTrainTest(docs, 0.5);
        var data2 = Document.SplitTrainTest(docs, 0.5);

        Assert.AreEqual(data1.test, data2.test);
        Assert.AreEqual(data1.train, data2.train);
    }
}
