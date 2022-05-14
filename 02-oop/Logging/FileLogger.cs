using System;
using System.Diagnostics;
using System.IO;

namespace Homework1
{
    public class FileLogger : ILogger {
        public void Log(string message) {
             using(StreamWriter stream = new StreamWriter("logs.txt", true)) {
                stream.Write(message);
             }
        }
    }
}