using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Document
{
    public Document(string title, DateTime created_utc, string class_name)
    {
        Title = title;
        CreatedUtc = created_utc;
        ClassName = class_name;
    }
    public string Title { get; init; }
    public DateTime CreatedUtc { get; init; }
    public string ClassName { get; init; }

    public static (List<Document>, List<Document>) SplitTrainTest(List<Document> documents, double trainSize)
    {
        int train_count = (int)(trainSize * documents.Count);

        documents = documents.OrderBy(document => document.Title).ThenBy(document => document.CreatedUtc).ToList();
        
        var random = new Random(documents[0].CreatedUtc.Millisecond);
        documents = documents.OrderBy(document => random.Next()).ToList();
        return (documents.Take(train_count).ToList(), documents.Skip(train_count).ToList());
    }
}


