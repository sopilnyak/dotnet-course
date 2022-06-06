namespace hw3.Task2; 

public class Document {
    public string Title { get; init; }
    public DateTime CreatedUtc { get; init; }
    public string ClassName { get; init; }
}

public static class DocumentSplitter {
    public static (List<Document>, List<Document>) SplitTrainTest(List<Document> documents, double trainSize) {
        Random random = new Random(42);

        var shuffled = documents.OrderBy(item => item.Title)
            .ThenBy(item => item.CreatedUtc).ThenBy(item => random.Next()).ToList();

        int train_len = Convert.ToInt32(Math.Floor(documents.Count * trainSize));

        return (
            shuffled.GetRange(0, train_len),
            shuffled.GetRange(train_len + 1, documents.Count)
        );
    }
}