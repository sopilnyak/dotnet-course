using NUnit.Framework;
using hw3.Task2;

namespace hw3.Tests; 

public class SplitterTests {
    public List<Document> CreateLists() {
        return new List<Document> {
            new Document() {
                Title = "Hello",
                CreatedUtc = new DateTime(1970, 1, 1),
                ClassName = "Linux"
            },
            new Document() {
                Title = "Raft",
                CreatedUtc = new DateTime(2015, 4, 15),
                ClassName = "TFTDS"
            },
            new Document() {
                Title = "Paxos",
                CreatedUtc = new DateTime(1998, 5, 5),
                ClassName = "TFTDS"
            },
            new Document() {
                Title = "Apple Pie",
                CreatedUtc = new DateTime(1955, 1, 6),
                ClassName = "Cooking"
            }
        };
    }
    
    
    [TestCase(.5)]
    [TestCase(.3)]
    public void SplitTest(double train_size) {
        var documents = CreateLists();

        var (train_set, test_set) = DocumentSplitter.SplitTrainTest(documents, train_size);

        Assert.AreEqual(documents.Count, train_set.Count + test_set.Count);
        
        Assert.AreEqual(Convert.ToInt32(Math.Floor(documents.Count * train_size)), train_set.Count);

        var merged = train_set.Concat(test_set).ToList();

        Assert.True(documents == merged);
    }
}
