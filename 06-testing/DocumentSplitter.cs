namespace Testing
{
    public class DocumentSplitter
    {
        public static (List<Document>, List<Document>) SplitTrainTest(Document[] data, double trainSize, int seed = 87)
        {
            var rnd = new Random(seed);
            var trainLen = Convert.ToInt32(Math.Floor(data.Length * trainSize));
            var readyForSplit = from doc in data
                                orderby (doc.Title, doc.CreatedUtc)
                                select doc;

            var splitted = (from doc in readyForSplit
                            orderby (rnd.NextInt64())
                            select doc).ToArray();
            var train = new List<Document>();
            var test = new List<Document>();
            for (int i = 0; i < data.Length; i++)
            {
                if (i < trainLen)
                {
                    train.Add(splitted[i]);
                }
                else
                {
                    test.Add(splitted[i]);
                }
            }
            return (train, test);
        }
    }
}
