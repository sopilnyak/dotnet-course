using OOP.Sorter;

namespace OOP;

public static class Program
{
    public static void Main()
    {
        var list = new List<int> {5, 1, 29, 0, 7, 6};
        var sorterConsoleLog = new Sorter<int>();
        sorterConsoleLog.Sort(list);

        var strList = new List<string> {"Peter", "Ann", "Joanna", "Rose", "Paul", "Harry", "Mary"};
        var sorterFileLog = new Sorter<string>("../../../logs.txt"); // file will appear in the same directory as Program.cs
        sorterFileLog.Sort(strList);
    }
}
