/*
    Нужно реализовать систему классов и интерфейсов, которая умеет сортировать массив объектов любого типа (см. интерфейс IComparable).
    
    Требования:
     - Поддержка минимум двух алгоритмов сортировки.
     - Выбор алгоритма сортировки на основании исходного массива (ради упрощения, выбирать алгоритм можно учитывая количество элементов).
     - Логгирование хода работы программы: «получен массив из N элементов», «выбран алгоритм K», «отсортировано за M времени».
     - Поддержка настройки механизма логгирования: в консоль и/или на диск.
*/

using System;

namespace HW1
{
    class Program
    {
        static void Main(string[] args)
        {
            // пример работы
            int length = Convert.ToInt32(Console.ReadLine());
            int[] arr = new int[length];
            for (int i = 0; i < length; i++)
            {
                arr[i] = Convert.ToInt32(Console.ReadLine());
            }

            FileLogger fileLogger = new FileLogger("HW1/HW1/log.txt");

            //ConsoleLogger consoleLogger = new ConsoleLogger();

            ArraySorter<int> sorter = new ArraySorter<int>(arr, fileLogger);
            sorter.Sort();

            for (int i = 0; i < length; i++)
            {
                fileLogger.Log(arr[i].ToString());
            }
        }
    }
}