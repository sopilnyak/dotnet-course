using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    public class Timer
    {
        private static Stopwatch stopWatch;
        public static void StartLog<T>(T[] items, string algo_name, ILogger logger)
        {
            string line1 = "получен массив из " + items.Length + " элементов" + "\r\n";
            string line2 = "выбран алгоритм " + algo_name + "\r\n";
            line1 = line1 + line2;
            logger.Log(line1);
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }
        public static void EndLog<T>(T[] items, ILogger logger)
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            string line1 = "отсортировано за " + elapsedTime  +" времени" + "\r\n\n";
            logger.Log(line1);
            

        }
    }
}