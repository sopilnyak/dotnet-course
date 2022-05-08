namespace Testing.hw3.Task2;
using System;
using System.Collections.Generic;
using System.Linq;

public class Document
{
    public string Title { get; init; }
    public DateTime CreatedUtc { get; init; }
    public string ClassName { get; init; }
    
    public Document(string title, DateTime createdUtc, string className)
    {
        Title = title;
        CreatedUtc = createdUtc;
        ClassName = className;
    }
    
    public static (List<Document>, List<Document>) SplitTrainTest(List<Document> documents, double trainSize)
    {
        int trainCount = (int)(trainSize * documents.Count);

        documents = documents.OrderBy(document => document.Title).ThenBy(document => document.CreatedUtc).ToList();

        var random = new Random();
        documents = documents.OrderBy(document => random.Next()).ToList();
        return (documents.Take(trainCount).ToList(), documents.Skip(trainCount).ToList());
    }
}