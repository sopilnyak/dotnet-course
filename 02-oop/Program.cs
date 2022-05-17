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
using OOP.Sorting;
class Program
{
    public static void Main()
    {
        //int[] list = new []{1, 3, 232, 545, 32, 5, 23};
        string[] list = new[] {"boogy-woogy", "Arra", "a", "aboba",  "dsfdgas" };
        
        Console.Write("Исходный массив: ");
        for (int i = 0; i < list.Length; i++)
        {
            Console.Write("[" + i + "]: " + list[i] + " ");
        }
        Console.WriteLine();
        var comp = new Comp<string>(list, consoleLog: true, fileLog:true, pathLog:"text.txt");
        comp.Sort();
        Console.WriteLine();
        Console.Write("Отсортированный массив: ");
        for (int i = 0; i < list.Length; i++)
        {
            Console.Write("[" + i + "]: " + list[i] + " ");
        }
    }
}
