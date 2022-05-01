using System.Linq;


static class Program
{
    static void Main(string[] args)
    {
    }
}

public class Document
{
    public string Title { get; init; }
    public DateTime CreatedUtc { get; init; }
    public string ClassName { get; init; }
}


public struct Work_With_Documents
{
    public (List<Document>, List<Document>) SplitTrainTest(List<Document>
       documents, double trainSize)
    {
        List<Document> orderedDocuments = (from document in documents
                               orderby document.Title, document.CreatedUtc
                               select document)
                               .ToList();


        List<Document> firstHalf = new List<Document>();
        List<Document> secondHalf = new List<Document>();
        for (int i = 0; i < orderedDocuments.Count; i++)
        {
            if (i < trainSize)
            {
                firstHalf.Add(orderedDocuments[i]);
            }
            else
            {
                secondHalf.Add(orderedDocuments[i]);
            }
        }
        return (firstHalf, secondHalf);
    }
}



