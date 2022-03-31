
namespace Logger
{

    interface ILogger
    {
        public Task LogAsync(string message);
        public void Log(string message);
    }

    class FileLogger : ILogger
    {
        private StreamWriter writer;
        public FileLogger(string filename)
        {
            this.writer = new StreamWriter(filename, true);
        }

        public void Log(string message)
        {
            this.writer.Write(message);
            this.writer.Flush();
        }

        async public Task LogAsync(string message)
        {
            await this.writer.WriteLineAsync(message);
            await this.writer.FlushAsync();
        }
    }

    class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }
        async public Task LogAsync(string message)
        {
            await Console.Out.WriteLineAsync(message);
            await Console.Out.FlushAsync();
        }
    }

}