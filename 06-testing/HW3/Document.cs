namespace Testing.HW3;

public class Document
{
    public string Title { get; init; }
    public DateTime CreatedUtc { get; init; }
    public string ClassName { get; init; }

    public static (List<Document>, List<Document>) SplitTrainTest(List<Document> documents, double trainSize, int? seed = null)
    {
        if (trainSize is >= 0 and <= 1)
        {
            throw new ArgumentException($"{nameof(trainSize)} {trainSize} must between 0 and 1");
        }
        
        var sortedDocuments = documents
            .OrderBy(document => document.Title)
            .ThenBy(document => document.CreatedUtc);
        var randomGenerator = seed == null ? new Random() : new Random(seed.Value);
        var shuffledDocuments = sortedDocuments.OrderBy(_ => randomGenerator.Next()).ToList();
        var trainCount = (int)(documents.Count * trainSize);
        
        return (shuffledDocuments.Take(trainCount).ToList(), shuffledDocuments.Skip(trainCount).ToList());
    }
}
