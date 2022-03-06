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

namespace X{ 
// class for tests

class Person : IComparable
{
    public string Name { get;}
    public int Age { get; set; }
    public Person(string name, int age)
    {
        Name = name; Age = age;
    }
    public int CompareTo(object? o)
    {
        if(o is Person person) return Age.CompareTo(person.Age);
        else throw new ArgumentException("Некорректное значение параметра");
    }
}

// interface for logging settings
interface ILogger {
    void Logging(string message);
}


public class FileLogger: ILogger {
    public string filename; 
    public FileLogger(string filename) {
        this.filename = filename;
    }
    public void Logging(string message) {
        using (StreamWriter writer = new StreamWriter(this.filename, true)) {
            writer.WriteLine(message);
        }
    } 
}

public class Logger: ILogger {
    public string filename; 
    public Logger(string filename) {
        this.filename = filename;
    }
    public void Logging(string message) {
        Console.WriteLine(message);
        using (StreamWriter writer = new StreamWriter(this.filename, true)) {
            writer.WriteLine(message);
        }
    }
}

public class ConsoleLogger: ILogger {
    public ConsoleLogger() {}
    public void Logging(string message) {
        Console.WriteLine(message);
    }
}

public class EmptyLogger: ILogger {
    public EmptyLogger() {}
    public void Logging(string message) {}
}

public class ArraySorter<T> where T:IComparable {
    public ArraySorter(T[] array, bool console_log = false, bool file_log = false, string filename = "log.txt") {
        this.array = array;
        if (console_log && file_log) {
            this.logger = new Logger(filename);
        } else if (console_log) {
            this.logger = new ConsoleLogger();
        } else if (file_log) {
            this.logger = new FileLogger(filename);
        } else {
            this.logger = new EmptyLogger();
        }
        logger.Logging($"получен массив из {array.Length} элементов");

    }
    public void Sort() {
        var startTime = System.Diagnostics.Stopwatch.StartNew();
        if (this.array.Length > 10) {
            logger.Logging("Выбран алгоритм быстрой сортировки");
            QuickSort();
        } else {
            logger.Logging("Выбран алгоритм пузырьковой сортировки");
            BubbleSort();
        }
        startTime.Stop();
        var resultTime = startTime.Elapsed;

        string time_for_logging = String.Format("Отсортировано за {0}.{1} секунд",
            resultTime.Seconds,
            resultTime.Milliseconds);
        logger.Logging(time_for_logging);
    }
    void BubbleSort() {
        int len = this.array.Length;
        for (int i = 0; i < len; i++) {
            for (int j = i + 1; j < len; j++) {
                if (this.array[i].CompareTo(this.array[j]) > 0) {
                    (this.array[i], this.array[j]) = (this.array[j], this.array[i]);
                }
            }
        }
    }
    void QuickSort() {
        QuickSortImpl(this.array, array.Length);
    }

    void QuickSortImpl(T[] data, int len) {
        int ind = len / 2;
        int left_len = 0,right_len = 0;
        if (len <= 1) {
            return;
        }
        T[] left_array = new T[len];
        T[] right_array = new T[len];
        T pivot = data[ind];
        for (int i = 0; i < len; i++) {
            if (i != ind) {
                if (data[i].CompareTo(pivot) < 0) {
                    left_array[left_len] = data[i];
                    left_len++;
                }
                else{
                    right_array[right_len] = data[i];
                    right_len++;
                }
            }
        }
        QuickSortImpl(left_array, left_len);
        QuickSortImpl(right_array, right_len);
        for (int cnt = 0; cnt < len; cnt++) {
            if (cnt < left_len) {
                data[cnt] = left_array[cnt];
            }
            else if (cnt==left_len) {
                data[cnt] = pivot;
            }
            else{
                data[cnt] = right_array[cnt - (left_len + 1)];
            }
        }
    }
    public T[] array {get; set;}
    ILogger logger;
}


    class Program {
        // tests
        public static void Main (string[] args) {
            // Test person array
            Person[] person_array = {new Person("Nataliya", 30), new Person("Mikhail", 20), new Person("Vladimir", 40)};
            ArraySorter<Person> person_sorter = new ArraySorter<Person>(person_array, true);
            person_sorter.Sort();
            foreach (Person person in person_sorter.array)
            {
                Console.WriteLine($"{person.Name} - {person.Age}");
            }

            Console.WriteLine("======================================");
            // Test int array
            int[] int_array = {3, 1, 2};
            ArraySorter<int> int_sorter = new ArraySorter<int>(int_array, false, true);
            int_sorter.Sort();
            foreach (int number in int_sorter.array)
            {
                Console.WriteLine(number);
            }
            Console.WriteLine("======================================");
            // Test big int array
            int[] big_int_array = {3, 1, 2, 10, 9, 7, 8, 4, 5, 6, 11, 13};
            ArraySorter<int> big_int_sorter = new ArraySorter<int>(big_int_array, true, true, "log1.txt");
            big_int_sorter.Sort();
            foreach (int number in big_int_sorter.array)
            {
                Console.WriteLine(number);
            }       
        }
    }
}