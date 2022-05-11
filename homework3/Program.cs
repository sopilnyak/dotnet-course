using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Homework3.Task2;

namespace Homework3
{
    public class Program
    {
        public static void Main()
        {
            CreateHostBuilder().Build().RunAsync().Wait();
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) => 
                    services.AddScoped<Document>());
    }
}
