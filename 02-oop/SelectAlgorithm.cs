using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    public class SelectAlgorithm
    {
        
        public static void SelectSort<T>(T[] arr, bool is_infile) where T : IComparable
        {
            ISorter parser;
            if (arr.Length <= 500)
            {
                parser = new BubbleSort();
                Log.StartLog(arr, "BubbleSort", is_infile);
                parser.Sort(arr);
                Log.EndLog(arr, is_infile);
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
                    Log.StartLog(arr, "ShellSort", is_infile);
                    parser.Sort(arr);
                    Log.EndLog(arr, is_infile);
                }
                else
                {
                    parser = new MergeSort();
                    Log.StartLog(arr, "MergeSort", is_infile);
                    parser.Sort(arr);
                    Log.EndLog(arr, is_infile);
                }
            }
        }
    }
}