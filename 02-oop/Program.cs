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

namespace OOP
{
    class Program
    {
        // tests
        public static void Main(string[] args)
        {
            // Test person array
            Person[] person_array = {
                    new Person {Name = "Nataliya", Age = 30},
                    new Person {Name = "Mikhail", Age = 20},
                    new Person {Name = "Vladimir", Age = 40},
            };
            var firstLogger = new ConsoleLogger();
            var person_sorter = new ArraySorter<Person>(person_array, firstLogger);
            person_sorter.Sort();
            foreach (var person in person_sorter.Array)
            {
                Console.WriteLine($"{person.Name} - {person.Age}");
            }

            Console.WriteLine("======================================");
            // Test int array
            var int_array = new int[] { 3, 1, 2 };
            var secondLogger = new FileLogger("log.txt");
            ArraySorter<int> int_sorter = new ArraySorter<int>(int_array, secondLogger);
            int_sorter.Sort();
            foreach (var number in int_sorter.Array)
            {
                Console.WriteLine(number);
            }
            Console.WriteLine("======================================");
            // Test big int array
            var big_int_array = new int[] { 3, 1, 2, 10, 9, 7, 8, 4, 5, 6, 11, 13 };

            var thirdLogger = new Logger("log1.txt");
            ArraySorter<int> big_int_sorter = new ArraySorter<int>(big_int_array, thirdLogger);
            big_int_sorter.Sort();
            foreach (var number in big_int_sorter.Array)
            {
                Console.WriteLine(number);
            }
        }
    }
}
