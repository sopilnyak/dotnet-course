using System;
using System.Collections.Generic;
using System.Linq;


namespace Testing.HW3
{
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
            var trainCount = (int)(trainSize * documents.Count);
            var random = new Random();
            
            documents = documents
                .OrderBy(document => document.Title)
                .ThenBy(document => document.CreatedUtc)
                .ThenBy(_ => random.Next())
                .ToList();

            return (documents.Take(trainCount).ToList(), documents.Skip(trainCount).ToList());
        }
    }
}