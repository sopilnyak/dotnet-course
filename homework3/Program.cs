using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Homework3.BatchIterator;

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
                .ConfigureServices((_, services) => services.AddHostedService<Worker>()
                    .AddScoped<IMessageWriter, MessageWriter>());
    }
}
