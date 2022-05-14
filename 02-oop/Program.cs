using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{

    public class Program
    {
        // Generate of ints and strings have different algorithms(that's why function is not Generic)
        private static int[] generateArray(int count)
        {
            Random random = new Random();
            int[] values = new int[count];
            for (int i = 0; i < count; ++i)
                values[i] = random.Next();
            return values;
        }
        private static string[] generateArrayOfString(int count)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string[] values = new string[count];
            string randString = "";
            var random = new Random();
            for (int j = 0; j < count; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    randString += chars[random.Next(chars.Length)];
                }
                values[j] = randString;
                randString = "";
            }
            return values;
        }
        public static void Main()
        {
            Console.WriteLine("Where to write logs [f]ile/[c]onsole");
            string input = Console.ReadLine();
            ILogger logger;
            if(input[0] == 'f') {
                logger = new FileLogger();
            } else {
                logger = new ConsoleLogger();
            }

            File.Delete("logs.txt");
            string[] randomStrings1 = generateArrayOfString(400);
            SelectAlgorithm.SelectSort(randomStrings1, logger);
            string[] randomStrings2 = generateArrayOfString(50000);
            SelectAlgorithm.SelectSort(randomStrings2, logger);

            int[] randomNumbers1 = generateArray(80000);
            SelectAlgorithm.SelectSort(randomNumbers1, logger);
            int[] randomNumbers2 = generateArray(123200);
            SelectAlgorithm.SelectSort(randomNumbers2, logger);
            int[] randomNumbers3 = generateArray(1300000);
            SelectAlgorithm.SelectSort(randomNumbers3, logger);
            int[] randomNumbers4 = generateArray(6000000);
            SelectAlgorithm.SelectSort(randomNumbers4, logger);
        }
    }
}
