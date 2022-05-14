using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    public class SelectAlgorithm
    {
        
        public static void SelectSort<T>(T[] arr, ILogger logger) where T : IComparable
        {
            ISorter parser;
            if (arr.Length <= 500)
            {
                parser = new BubbleSort();
                Timer.StartLog(arr, "BubbleSort", logger);
                parser.Sort(arr);
                Timer.EndLog(arr, logger);
            }
            else
            { // measure accuracy of sorted subarrays
                int sorted_cnt = 0;
                for (int i = 1; i < arr.Length; ++i)
                {
                    while (i < arr.Length && (arr [i]
                                                .CompareTo(arr[i - 1]) >= 0))
                    {
                        sorted_cnt++;
                        i++;
                    }
                }
                float accuracy = (float) sorted_cnt / (arr.Length - 1);
                if (accuracy > 0.5 && arr.Length < 200000)
                {
                    parser = new ShellSort();
                    Timer.StartLog(arr, "ShellSort", logger);
                    parser.Sort(arr);
                    Timer.EndLog(arr, logger);
                }
                else
                {
                    parser = new MergeSort();
                    Timer.StartLog(arr, "MergeSort", logger);
                    parser.Sort(arr);
                    Timer.EndLog(arr, logger);
                }
            }
        }
    }
}