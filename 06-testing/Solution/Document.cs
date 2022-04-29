namespace Testing.Solution;

public class Document
{
    public string Title { get; init; }
    public DateTime CreatedUtc { get; init; }
    public string ClassName { get; init; }
    
    public static (List<Document>, List<Document>) SplitTrainTest(List<Document> documents, double trainSize)
    {
        var sortedDocuments = documents
            .OrderBy(document => document.Title)
            .ThenBy(document => document.CreatedUtc);

        var random = new Random();
        var randomDocuments = sortedDocuments.OrderBy(_ => random.Next()).ToList();

        var trainAbsSize = (int) (documents.Count * trainSize);

        return (randomDocuments.Take(trainAbsSize).ToList(), randomDocuments.Skip(trainAbsSize).ToList());
    }
}