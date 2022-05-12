using System;

public enum LoggerFlag
{
    None = 0,
    Articles = 1,
    Feeds = 2,
    Errors = 4,
    Loop = 8,
    Important = Errors | Loop,
    All = Articles | Feeds | Errors | Loop
}

public class Logger
{
    private LoggerFlag _flag = LoggerFlag.All;
    public Logger(LoggerFlag f = LoggerFlag.All) => _flag = f;
    public void Log(string str, LoggerFlag f)
    {
        if ((_flag & f) != LoggerFlag.None)
        {
            Console.WriteLine(str);
        }
    }
}
