using System;

public interface ILogger
{
    public void Log(string str);
}

public class ConsoleLogger : ILogger
{
    public void Log(string str) => Console.WriteLine(str);
}

public class FileLogger : ILogger
{
    public string Path { get; set; }

    public FileLogger(string path) => Path = path;

    public void Log(string str)
    {
        using var file = new System.IO.StreamWriter(Path, true);
        file.WriteLine(str);
    }
}




