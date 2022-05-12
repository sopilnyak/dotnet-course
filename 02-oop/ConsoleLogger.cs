using System;

namespace HW1
{
    public class ConsoleLogger : ILog
    {
        public void Log(string format, object arg0)
        {
            Console.WriteLine(format, arg0);
        }

        public void Log(string value)
        {
            Console.WriteLine(value);
        }
    }
}