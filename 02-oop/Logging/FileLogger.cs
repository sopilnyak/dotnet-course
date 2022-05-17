namespace OOP.Logging;

public class FileLogger: ILogger {
    private string _logPath; 
    public FileLogger(string filename) {
        _logPath = filename;
    }
    public void Log(string message)
    {
        using (StreamWriter writer = new StreamWriter(_logPath, true)) writer.WriteLine(message);
    } 
}