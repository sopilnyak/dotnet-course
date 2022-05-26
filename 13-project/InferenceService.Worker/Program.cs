using InferenceService.Worker;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(services =>
{
	services.AddHostedService<Worker>();
	services.AddSingleton<ILogger>(serviceProvider => serviceProvider.GetRequiredService<ILogger<Worker>>());
});

builder.ConfigureLogging(logging =>
{
	logging.ClearProviders();
	logging.AddSimpleConsole(options =>
	{
		options.SingleLine = true;
		options.TimestampFormat = "[HH:mm:ss] ";
	});
});

var app = builder.Build();

app.Run();
