namespace HW1
{
    public interface ILog
    {
        void Log(string format, object arg0);
        void Log(string value);
    }
}