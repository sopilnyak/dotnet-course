using System.IO;

namespace HW1
{
    public class FileLogger : ILog
    {
        private readonly string _path;

        public FileLogger(string path)
        {
            this._path = path;
        }

        public void Log(string format, object arg0)
        {
            StreamWriter sw = new StreamWriter(_path, true);
            sw.WriteLine(format, arg0);
            sw.Close();
        }

        public void Log(string value)
        {
            StreamWriter sw = new StreamWriter(_path, true);
            sw.WriteLine(value);
            sw.Close();
        }
    }
}