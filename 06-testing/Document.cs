namespace Testing
{
    public class Document
    {
        public string Title { get; init; }
        public DateTime CreatedUtc { get; init; }
        public string ClassName { get; init; }
    }

    public static class DocumentWorker
    {
        public static (List<Document>, List<Document>) SplitTrainTest(List<Document> documents, double trainSize)
        {
            var sortedByData = from doc in documents
                               orderby (doc.CreatedUtc, doc.Title)
                               select doc;

            var sortedByRandom = (from doc in sortedByData
                                  orderby ((new Random(documents.Count)).Next())
                                  select doc).ToList();

            var trainAmount = (int)(trainSize * sortedByRandom.Count);
            return (sortedByRandom.Take(trainAmount).ToList(), sortedByRandom.Skip(trainAmount).ToList());
        }
    }
}
 