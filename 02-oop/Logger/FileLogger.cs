namespace OOP.Logger;

public class FileLogger : ILogger
{
    private readonly StreamWriter _writer;

    public FileLogger(string filename)
    {
        _writer = new StreamWriter(filename);
        _writer.AutoFlush = true;
    }

    public void WriteLine(string str)
    {
        _writer.WriteLine(str);
    }
}