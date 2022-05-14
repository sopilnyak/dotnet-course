using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    public class ConsoleLogger : ILogger {
        public void Log(string message) {
            Console.Write(message);   
        }
    }
}

