using Microsoft.Extensions.Hosting;

namespace Testing.DependencyInjection;

public class Worker : BackgroundService
{
    private readonly IMessageWriter _messageWriter;

    public Worker(IMessageWriter messageWriter) => _messageWriter = messageWriter;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _messageWriter.Write("Worker is executing");
            await Task.Delay(1000, stoppingToken);
        }
    }
}
