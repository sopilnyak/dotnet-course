using System.Diagnostics;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

// Заметьте, что нет никакой точки входа в приложение - прямо как в питоне.
// На самом деле это обман - компилятор создает точку входа за нас. До C# 9 мы бы написали что-то подобное:
// static class Program
// {
//     static void Main(string[] args)
//     {
//         // И тут наш код
//     }
// }

#region Ссылочные и значимые типы: заглянем в кучу

// Строка - ссылочный тип, значит строки хранятся в куче.
var s = new string('A', 1000000);
// Заглянем в управляемую кучу (F5 -> Diagnostic Tools -> Memory Usage -> Take Snapshot -> View Heap) и найдем там нашу строку.
// Внимание: чтобы в куче отобразились строки, нужно снять галочку Show Just My Code.
Debugger.Break();

#endregion

#region Ссылочные и значимые типы: передадим в метод

// Объявим SomeValueType как struct (значимый тип)...
var valueType = new SomeValueType { Field = 0 };
// ...проверим, что он наследуется от базового класса System.ValueType...
Console.WriteLine($"valueType is value type? {valueType is ValueType}");
// ...и попробуем изменить значение поля в методе.
Operations.Increment(valueType);
Console.WriteLine($"valueType.Field new value: {valueType.Field}");
// Значение не поменялось, потому что значимые типы передаются по значению.

// А теперь объявим SomeReferenceType как class (ссылочный тип) и проделаем то же самое.
var refType = new SomeReferenceType { Field = 0 };
Console.WriteLine($"refType is value type? {refType is ValueType}");
Operations.Increment(refType);
Console.WriteLine($"refType.Field new value: {refType.Field}");
// Видим новое значение у поля Field - а всё потому что он передался по ссылке.

// Но значимый тип тоже можно передать по ссылке - для этого у параметра есть ключевое слово ref.
Operations.Increment(ref valueType);
Console.WriteLine($"valueType.Field new value with ref: {valueType.Field}");

#endregion

#region Аргументы методов: out и params

Operations.Get100500(out SomeValueType outValueType);
Console.WriteLine($"outValueType.Field: {outValueType.Field}");

string joined = Operations.Join("Cat", "Dog");
Console.WriteLine(joined);

#endregion

#region Ссылочные и значимые типы: попробуем скопировать

var valueTypeCopy = valueType;
valueTypeCopy.Field = 2;
Console.WriteLine($"{nameof(valueType.Field)} after copy: {valueType.Field}");
var refTypeCopy = refType;
refTypeCopy.Field = 2;
Console.WriteLine($"{nameof(refType.Field)} after copy: {refType.Field}");

#endregion

#region Ссылочные и значимые типы: кто сколько занимает памяти

// Посмотрим теперь, кто сколько занимает памяти.
// Для значимого типа достаточно вызвать Unsafe.SizeOf<T>.
// Вообще говоря, так делать не очень хорошо, потому что размер структуры зависит от платформы,
// на которой запущена программа - ею управляет Just-In-Time (JIT) компилятор.
Console.WriteLine(Unsafe.SizeOf<SomeValueType>());
// Структура из двух интов занимает 4 + 4 = 8 байт - ровно столько, сколько нужно его полям.

// Для ссылочных типов запросить размер вообще невозможно, поэтому обратимся к профилировщику.
Debugger.Break();

#endregion

#region Ссылочные и значимые типы: null

// null - выделенное значение для ссылочных типов, нулевая ссылка.
refType = null;
//Console.WriteLine(refType.Field); // бросает System.NullReferenceException
//valueType = null; // не компилируется

#endregion

#region Немного про значимые типы и конструкторы

// До C# 10 у значимых типов (структур) невозможно было определить конструктор без параметров или инициализировать поля.
// В C# 10 эту возможность ввели, но с ограничениями.

// Создадим такой массив.
var intervals = new Interval[1000];

// CLR умеет эффективно заполнять такие массивы, не вызывая конструктор для каждого из 1000 элементов,
// а просто заполняя нулями память.
// Но определять конструкторы без параметров тоже хотелось, поэтому решили сделать так:

var i1 = new Interval();
Console.WriteLine(i1); // [0, int.MaxValue]

var i2s = new Interval[2];
Console.WriteLine(string.Join(", ", i2s)); // [0, 0], [0, 0]

var i3 = default(Interval); // default - оператор, который позволяет получить значение по умолчанию у любого типа.
Console.WriteLine(i3); // [0, 0]

#endregion

#region Неявная типизация и target-typed new expressions

var guid = new Guid();
Console.WriteLine(guid);

Cat cat1 = new Cat();
Cat cat2 = new();
// Target-typed new expressions удобны, когда нужно определить поле, инициализированное не null
// class Cat
// {
//     public Paw LeftPaw = new();
// }

#endregion

#region Приведение типов

int number = 0;
object numberObj = number;
long numberLong = number;
// int x = numberLong; // ошибка компиляции - приведение не расширяющее

// Явное приведение
Animal animal = new Cat();
var a = (Cat)animal;
int b = (int)number;

// Operations.Feed(new Dog()); // System.InvalidCastException

#endregion

#region Операторы is и as

// Оператор is проверяет тип и возвращает bool.

bool isObject = animal is object; // true
Console.WriteLine($"Animal is object? {isObject}");

bool isInt = animal is int; // false
Console.WriteLine($"Animal is int? {isInt}");

// Оператор as, если возможно, приводит к типу.
Animal animalAsCat = animal as Cat;
Console.WriteLine($"Animal as Cat: {animalAsCat}");

// Если привести невозможно, возвращает null.
// В отличие от явного приведения оператор as не генерирует исключение.
Dog animalAsDog = animal as Dog;
Console.WriteLine($"Animal as Dog: {animalAsDog}");

// as может работать только со ссылочными и Nullable типами.

#endregion

#region Pattern matching

switch (animal)
{
    case Cat cat:
        cat.Meow();
        break;
    case Dog dog:
        dog.Bark();
        break;
    default: // это всегда выполняется последним
        Console.WriteLine("Unknown animal");
        break;
    case null:
        throw new ArgumentNullException(nameof(animal));
}

// В блоке case всегда должно использоваться одно из: break, goto case, return или throw

#endregion

#region Boxing / unboxing

// Можно создать обертку над объектом значимого типа в куче

// Так как создается новый объект, это очень дорогое занятие. boxing может занять в 20 раз больше времени,
// чем простое присвоение ссылки, а unboxing - в 4 раза больше.

// На практике это значит, что надо избегать non-generic коллекций, таких как ArrayList или Hashtable.
// Сравните:

var arraySize = 1000000;
var arrayList = new ArrayList(arraySize);
for (int i = 0; i < arrayList.Capacity; i++)
{
    arrayList.Add(i);
}

var list = new List<int>(arraySize);
for (int i = 0; i < list.Capacity; i++)
{
    list.Add(i);
}

#endregion

#region Операторы

// Индексы и диапазоны
string hello = "hello";
Console.WriteLine(hello[^1]); // последний элемент
Console.WriteLine(hello[1..3]); // элементы с 1 по 3 (не включительно)
Console.WriteLine(hello[..3]); // элементы с 0 по 3 (не включительно)
Console.WriteLine(hello[^3..^1]); // элементы с 3 с конца до предпоследнего включительно

// Операторы и/или имеют по два варианта: & и &&, | и ||
// Отличие их в том, что в первом случае всегда выполняются обе части, а во втором - правая выполняется только
// в случае необходимости.
string str = null;
bool isEmpty = str != null && str.Length == 0;
// bool isEmpty1 = str != null & str.Length == 0; // Породит исключение System.NullReferenceException

// Null-conditional operator
// Проверяет объект на null перед доступом к полю/свойству/методу/индексу, и если он null, то возвращает null.
string substr = str?.Substring(0, 1); // то же самое, что str == null ? null : str.Substring(0, 1);
substr = str?[..1]; // работает и с операторами

// Null-coalescing operator
// Если левая часть не null, вернет ее, иначе вернет правую часть
Console.WriteLine(str ?? "<unknown>");
// Причем если правая часть не нужна, она не выполняется
Console.WriteLine(hello ?? throw new ArgumentException(nameof(str)));

// Null-coalescing assignment operator
str ??= "<unknown>";
Console.WriteLine(str);

#endregion

#region NuGet

// Подключим библиотеку Newtonsoft.Json для работы с JSON и попробуем сериализовать и десериализовать объект в JSON.

//var doge = new Dog
//{
//    Name = "Doge",
//    Age = 16
//};

//var serialized = JsonConvert.SerializeObject(doge);
//Console.WriteLine(serialized);

//var deserializedDoge = JsonConvert.DeserializeObject<Dog>(serialized);
//Console.WriteLine(deserializedDoge.Name);

#endregion

#region Определения классов

public class SomeReferenceType
{
    public int Field = 0;
    public int Another = 0;
}

public struct SomeValueType
{
    public int Field;
    public int Another;
}

public struct Interval
{
    public int Left;
    public int Right = int.MaxValue;

    public override string ToString() => $"[{Left}, {Right}]";
}

public class Animal
{
    public string Name;
    public int Age;

    public void Eat() { }
}

public class Cat : Animal
{
    public void Meow()
    {
        Console.WriteLine("Meow!");
    }
}

public class Dog : Animal
{
    public void Bark()
    {
        Console.WriteLine("Gav!");
    }
}

public class Operations
{
    public static void Increment(SomeValueType obj)
    {
        obj.Field += 1;
    }

    public static void Increment(SomeReferenceType obj)
    {
        obj.Field += 1;
    }

    public static void Increment(ref SomeValueType obj)
    {
        obj.Field += 1;
    }

    public static void Get100500(out SomeValueType obj)
    {
        obj = new SomeValueType { Field = 100500 };
    }

    public static string Join(params string[] strings)
    {
        var sb = new StringBuilder();
        foreach (var s in strings)
        {
            sb.Append(s);
        }

        return sb.ToString();
    }

    public static void Feed(Animal animal)
    {
        var cat = (Cat)animal;
        cat.Meow();
    }
}

#endregion
