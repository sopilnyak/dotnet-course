using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    public class Log
    {
        private static Stopwatch stopWatch;
        public static void StartLog<T>(T[] items, string algo_name, bool is_infile)
        {
            string line1 = "получен массив из " + items.Length + " элементов" + "\r\n";
            string line2 = "выбран алгоритм " + algo_name + "\r\n";
            if (is_infile) {
                using(FileStream stream = new FileStream("logs.txt", FileMode.Append)){
                    byte[] array1 = System.Text.Encoding.Default.GetBytes(line1);
                    byte[] array2 = System.Text.Encoding.Default.GetBytes(line2);

                    stream.Write(array1, 0, array1.Length);
                    stream.Flush();
                    stream.Write(array2, 0, array2.Length);
                    stream.Flush();
                }
            }
            else
            {
                Console.Write(line1);
                Console.Write(line2);
            }
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }
        public static void EndLog<T>(T[] items, bool is_infile)
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            string line1 = "отсортировано за " + elapsedTime  +" времени" + "\r\n\n";

            if (is_infile) {
                using(FileStream stream = new FileStream("logs.txt", FileMode.Append)){
                    byte[] array1 = System.Text.Encoding.Default.GetBytes(line1);
                    stream.Write(array1, 0, array1.Length);
                    stream.Flush();
                }
            }
            else
            {
                Console.Write(line1);
                Console.Write('\n');
            }

        }
    }
}