/*
    Нужно реализовать систему классов и интерфейсов, которая умеет сортировать массив объектов любого типа (см. интерфейс IComparable).
    
    Требования:
     - Поддержка минимум двух алгоритмов сортировки.
     - Выбор алгоритма сортировки на основании исходного массива (ради упрощения, выбирать алгоритм можно учитывая количество элементов).
     - Логгирование хода работы программы: «получен массив из N элементов», «выбран алгоритм K», «отсортировано за M времени».
     - Поддержка настройки механизма логгирования: в консоль и/или на диск.

    10 баллов
    Мягкий дедлайн: 10.03.2022 23:59
    Жесткий дедлайн: 12.05.2022 23:59
*/
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
            bool is_infile = input[0] == 'f' ? true : false;

            File.Delete("logs.txt");
            string[] randomStrings1 = generateArrayOfString(400);
            SelectAlgorithm.SelectSort(randomStrings1, is_infile);
            string[] randomStrings2 = generateArrayOfString(50000);
            SelectAlgorithm.SelectSort(randomStrings2, is_infile);

            int[] randomNumbers1 = generateArray(80000);
            SelectAlgorithm.SelectSort(randomNumbers1, is_infile);
            int[] randomNumbers2 = generateArray(123200);
            SelectAlgorithm.SelectSort(randomNumbers2, is_infile);
            int[] randomNumbers3 = generateArray(1300000);
            SelectAlgorithm.SelectSort(randomNumbers3, is_infile);
            int[] randomNumbers4 = generateArray(6000000);
            SelectAlgorithm.SelectSort(randomNumbers4, is_infile);
        }
    }
}
