using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testing.DependencyInjection;

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
                .ConfigureServices((_, services) => services.AddHostedService<Worker>()
                    .AddScoped<IMessageWriter, MessageWriter>());
    }
}
