namespace Testing {
    public class Document
    {
        public string Title { get; init; }
        public DateTime CreatedUtc { get; init; }
        public string ClassName { get; init; }

        public Document(string title, DateTime date, string name) {
            this.Title = title;
            this.CreatedUtc = date;
            this.ClassName = name;
        }
    }

    public class DocumentSplitter {
        public static (List<Document>, List<Document>) SplitTrainTest(Document[] data, double trainSize, int seed = 87) {
            var rnd = new Random(seed);
            var train_len = Convert.ToInt32(Math.Floor(data.Length * trainSize));
            var readyForSplit = from doc in data
                                 orderby (doc.Title, doc.CreatedUtc)
                                 select doc;

            var splitted = (from doc in readyForSplit
                                 orderby (rnd.NextInt64())
                                 select doc).ToArray();
            var train = new List<Document>();
            var test = new List<Document>();
            for (int i = 0; i < data.Length; i++) {
                if (i < train_len) {
                    train.Add(splitted[i]);
                } else {
                    test.Add(splitted[i]);
                }
            }
            return (train, test);
        }
    }
}
