namespace OOP.Logging;

public class ConsoleFileLogger: ILogger {
    public FileLogger fileLogger;
    public ConsoleLogger consoleLogger;
    public ConsoleFileLogger(string filename)
    {
        fileLogger = new FileLogger(filename);
        consoleLogger = new ConsoleLogger();
    }
    public void Log(string message) {
        consoleLogger.Log(message);
        fileLogger.Log(message);
    }
}