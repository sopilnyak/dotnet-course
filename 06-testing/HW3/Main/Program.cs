using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainEx.Main.BatchIterator;
using MainEx.Main.SplitTrainTest;


namespace MainEx
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TrainTestModel model = new TrainTestModel();
            List<Document> documents = new List<Document> ();

            for (int i = 0; i< 10; i++)
            {
                Document d = new Document();
                d.Title = String.Format("doc{0}", i);
                d.ClassName = "1";
                d.CreatedUtc = new DateTime();

                documents.Add(d);

            }
            Console.WriteLine();


            TrainTestModel.SplitTrainTest(documents, 0.1);


            Console.ReadKey();
           
        }


    }


}
