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


static class Program
{
    static void Main(string[] args)
    {
        ILogger[] loggers = { new ConsoleLogger(), new FileLogger("output.txt") };
        foreach (var logger in loggers)
        { 
            ISorter<int>[] sorters = { new BubbleSort<int>(), new QuickSort<int>() };
            var sorter = new ComplexSorter<int>(sorters, logger);

            Random random_generator = new System.Random();

            int[] array = { 5, 4, 3, 2, 1 };
            sorter.Sort(array);

            array = new int[50];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random_generator.Next(0, array.Length);
            }
            sorter.Sort(array);

            array = new int[20000];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random_generator.Next(0, array.Length);
            }
            sorter.Sort(array);
        }
    }
}