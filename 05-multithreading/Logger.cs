// interface for logging settings
namespace news_portal {
    interface ILogger {
        void Logging(string message);
    }


    public class FileLogger: ILogger {
        public string filename; 
        public FileLogger(string filename) {
            this.filename = filename;
        }
        public void Logging(string message) {
            using (StreamWriter writer = new StreamWriter(this.filename, true)) {
                writer.WriteLine(message);
            }
        } 
    }

    public class Logger: ILogger {
        public string filename; 
        public Logger(string filename) {
            this.filename = filename;
        }
        public void Logging(string message) {
            Console.WriteLine(message);
            using (StreamWriter writer = new StreamWriter(this.filename, true)) {
                writer.WriteLine(message);
            }
        }
    }

    public class ConsoleLogger: ILogger {
        public ConsoleLogger() {}
        public void Logging(string message) {
            Console.WriteLine(message);
        }
    }

    public class EmptyLogger: ILogger {
        public EmptyLogger() {}
        public void Logging(string message) {}
    }
}