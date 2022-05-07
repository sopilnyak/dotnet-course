using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    public class BubbleSort : ISorter {
        private int lenght;
        
        private void Swap<T>(ref T a, ref T b) where T : IComparable
        {
            (a, b) = (b, a);
        }
        
        ////////////////////////////////////////////
        public void Sort<T>(T[] source) where T : IComparable
        {
            lenght = source.Length;
            bool sorted = false;

            for (int iteration = 0; iteration <= lenght * lenght; iteration++) // Worst case is n^2
            {
                bool swapped = false;
                for (int i = 0; i < lenght - 1; i++)
                {
                    if (source [i]
                            .CompareTo(source[i + 1]) == 1)
                    {
                        Swap(ref source[i], ref source[i + 1]);
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    sorted = true;
                    break;
                }
            }
            if (!sorted)
                throw new ArgumentException("Too few iterations to sort array.");
        }
    }
}