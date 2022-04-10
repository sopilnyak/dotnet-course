using System;

namespace Homework3.Task2
{
    public class Document
    {
        public Document(string title, DateTime created_utc, string class_name)
        {
            Title = title;
            CreatedUtc = created_utc;
            ClassName = class_name;
        }

        // для дебаг-вывода :)
        public override string ToString() {
            return "Title: " + Title + 
                   "; CreatedUtc: " + CreatedUtc.ToString() +
                   "; ClassName: " + ClassName;
        }

        public string Title { get; init; }
        public DateTime CreatedUtc { get; init; }
        public string ClassName { get; init; }
    }

    public static class DocumentSplitter
    {
        public static (List<Document>, List<Document>) SplitTrainTest(
            List<Document> documents, double trainSize)
        {
            var random = new Random(0);  // 0 - для воспроизводимости
            var shuffled_documents = documents.OrderBy(item => item.CreatedUtc)
                                .ThenBy(item => item.Title)
                                .OrderBy(item => random.Next())
                                .ToList();
            
            int train_length = Convert.ToInt32(Math.Floor(documents.Count * trainSize));
            int test_length = documents.Count - train_length;

            return (
                shuffled_documents.Take(train_length).ToList(),
                shuffled_documents.Skip(train_length).Take(test_length).ToList()
            );
        }
    }
}