namespace OOP.Logger;

public class ConsoleLogger : ILogger
{
    public void WriteLine(string str)
    {
        Console.WriteLine(str);
    }
}