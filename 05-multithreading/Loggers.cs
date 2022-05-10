namespace logger {
    public interface ILogger {
        public void Log(string msg);
    }

    public class ConsoleLogger : ILogger {
        public ConsoleLogger() { }

        public void Log(string msg) {
            Console.WriteLine(msg);
        }
    }

    public class FileLogger : ILogger {
        public FileLogger(string filename) {
            _filename = filename;
        }

        public void Log(string msg) {
            using var file = new StreamWriter(_filename, true);
            file.WriteLine(msg);
        }

        private readonly string _filename;
    }

    public class NoLogger : ILogger {
        public NoLogger() { }
        public void Log(string msg) { }
    }
}
