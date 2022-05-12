using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainEx.Main.SplitTrainTest
{
    public class TrainTestModel
    {

        public static (List<Document>, List<Document>) SplitTrainTest(List<Document> document, double trainSize)
        {
            List<Document> train = new List<Document>();
            List<Document> test = new List<Document>();

            int len = (int)Math.Round(Convert.ToDouble(document.Count()) * trainSize);

            var r = new Random();

            document = document
                .OrderBy(v => v.Title)
                .ThenBy(v => v.CreatedUtc)
                .ToList();

            document = document.OrderBy(i => r.Next()).ToList();


            train = document.Take(len).ToList();
            test = document.Skip(len).Take(document.Count() - len).ToList();

            return (train, test);
        }
    }
}
