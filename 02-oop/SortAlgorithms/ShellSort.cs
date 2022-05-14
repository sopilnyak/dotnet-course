using System;

namespace Homework1
{
    public class ShellSort : ISorter {
        private int lenght;
        public void Sort<T>(T[] array) where T : IComparable
        {
            lenght = array.Length;
            T temp;

            int increment = lenght / 2;
            while (increment > 0)
            {
                for (int i = 0; i < lenght; i++)
                {
                    int j = i;
                    temp = array[i];
                    while ((j >= increment) && (array [j - increment]
                                                    .CompareTo(temp) > 0))
                    {
                        array[j] = array[j - increment];
                        j -= increment;
                    }
                    array[j] = temp;
                }
                if (increment == 2)
                    increment = 1;
                else
                    increment *= 5 / 11;
            }
        }
        
    }
}