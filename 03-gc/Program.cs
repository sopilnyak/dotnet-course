using System.Text;

Thread.Sleep(TimeSpan.FromSeconds(30));

var stringSize = 100000000;
Console.WriteLine("allocate large string");
Test.AllocateString(stringSize);

Thread.Sleep(TimeSpan.FromSeconds(15));
Console.WriteLine("GC collect");
GC.Collect(2);

Thread.Sleep(TimeSpan.FromSeconds(5));

Console.WriteLine("allocate large StringBuilder");
Test.AllocateStringBuilder(stringSize);

Thread.Sleep(TimeSpan.FromSeconds(15));
Console.WriteLine("GC collect");
GC.Collect(2);

var strings = new List<string>();
var rnd = new Random();
for (var i = 0; i < 1000; i++)
{
    var random = rnd.Next(1000000, 10000000);
    var s = Test.AllocateString(random);

    if (i % 2 == 0)
    {
        strings.Add(s);
    }
}

Thread.Sleep(TimeSpan.FromHours(1));

// Финализаторы
ClassWithFinalizerTest.Test();
Console.WriteLine($"Before GC: {GC.GetGCMemoryInfo().FinalizationPendingCount}");
GC.Collect();
Console.WriteLine($"After GC: {GC.GetGCMemoryInfo().FinalizationPendingCount}");
GC.WaitForPendingFinalizers();
GC.Collect();
Console.WriteLine($"After WaitForPendingFinalizers: {GC.GetGCMemoryInfo().FinalizationPendingCount}");

public static class Test
{
    public static string AllocateString(int size)
    {
        return new string('A', size);
    }

    public static StringBuilder AllocateStringBuilder(int size)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < size; i++)
        {
            sb.Append('A');
        }

        return sb;
    }
}

public class ClassWithFinalizer
{
    ~ClassWithFinalizer()
    {
        Console.WriteLine("Finalizer start");
        Thread.Sleep(TimeSpan.FromSeconds(5));
        Console.WriteLine("Finalizer end");
    }
}

public static class ClassWithFinalizerTest
{
    public static void Test()
    {
        var test = new ClassWithFinalizer();
    }
}
