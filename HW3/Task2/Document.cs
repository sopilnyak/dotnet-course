using System;
using System.Collections.Generic;
using System.Linq;

namespace Task2
{
    public class Document
    {
        private string title;
        private DateTime created_utc;
        private string class_name;


        public string Title { 
            get
            {
                return title;
            } 
            init
            {
                title = value;
            }
        }
        public DateTime CreatedUtc { 
            get
            {
                return created_utc;
            } 
            init
            {
                created_utc = value;
            }
        }
        public string ClassName { 
            get
            {
                return class_name;
            } 
            init
            {
                class_name = value;
            }
        }

        public static (List<Document>, List<Document>) SplitTrainTest(
            List<Document> documents, double trainSize)
        {
            var random = new Random(42); 
            int train_size = (int)(documents.Count * trainSize);
            var shuffled_documents = documents.OrderBy(item => item.Title)
                                .ThenBy(item => item.CreatedUtc)
                                .OrderBy(_ => random.Next())
                                .ToList();
            
            var train_data = shuffled_documents.Take(train_size).ToList();
            var test_data = shuffled_documents.Skip(train_size).ToList();
            return (train_data, test_data);
        }
    }
}