namespace util;

public interface ILogger
{
    public void WriteLine(string str);
}

public class ConsoleLogger : ILogger
{
    public void WriteLine(string str)
    {
        Console.WriteLine($"{DateTime.Now.TimeOfDay} : {str}");
    }
}

public class FileLogger : ILogger, IDisposable
{
    private readonly StreamWriter _fileOutStream;

    public FileLogger(string logFilePath = "./log.txt")
    {
        try
        {
            var fs = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            _fileOutStream = new StreamWriter(fs);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot open {logFilePath} for writing : {e.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _fileOutStream.Dispose();
    }

    public void WriteLine(string str)
    {
        _fileOutStream.WriteLine($"{DateTime.Now.TimeOfDay} : {str}");
    }
}