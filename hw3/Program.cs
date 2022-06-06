using hw3.Task2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Testing
{
    public class Program
    {
        public static void Main()
        {
            CreateHostBuilder().Build().RunAsync().Wait();
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) => services.AddScoped<Document>());
    }
}
