
namespace OOP
{
    public interface ILogger
    {
        void Logging(string message);
    }


    public class FileLogger : ILogger
    {
        private string _filename;
        public FileLogger(string filename)
        {
            _filename = filename;
        }
        public void Logging(string message)
        {
            using (var writer = new StreamWriter(_filename, true))
                writer.WriteLine(message);

        }
    }

    public class Logger : ILogger
    {
        private string _filename;
        private ConsoleLogger _consoleLogger;
        private FileLogger _fileLogger;
        public Logger(string filename)
        {
            _filename = filename;
            _consoleLogger = new ConsoleLogger();
            _fileLogger = new FileLogger(filename);

        }
        public void Logging(string message)
        {
            _consoleLogger.Logging(message);
            _fileLogger.Logging(message);

        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Logging(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EmptyLogger : ILogger
    {
        public void Logging(string message) { }
    }
}