using System.Text;
using InferenceService.Contracts;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

namespace InferenceService.Worker;

public class Worker : IHostedService
{
	private readonly ILogger<Worker> _logger;
	private readonly IConfiguration _configuration;

	public Worker(ILogger<Worker> logger, IConfiguration configuration)
	{
		_logger = logger ?? throw new ArgumentException(nameof(logger));
		_configuration = configuration ?? throw new ArgumentException(nameof(configuration));
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var factory = new ConnectionFactory
		{
			HostName = _configuration["RabbitMQ"], // RabbitMQ host
			DispatchConsumersAsync = true // чтобы колбек у потребителя был асинхронным
		};
		using var connection = factory.CreateConnection(); // соединение по сокету, аутентификация и т.п.
		using var channel = connection.CreateModel(); // канал - это абстракция, которая позволяет управлять очередями и т.п.

		channel.QueueDeclare(
			queue: WellKnownNames.QueueName, // название очереди в RabbitMQ
			durable: true, // чтобы очередь не удалялась при рестарте сервера
			exclusive: false, // очередь используется только одним соединением и удаляется при закрытии соединения
			autoDelete: false // очередь удаляется после того как отсоединится последний потребитель
		);

		// По умолчанию RabbitMQ назначает потребителя для сообщения, когда сообщение заходит в очередь.
		// Это может привести к тому, что воркер зависнет на большой задаче, а на него уже будут назначены
		// другие задачи, и тогда эти другие задачи будут долго обрабатываться, а остальные воркеры простаивать.
		// Настройка prefetchCount = 1 говорит о том, чтобы воркеру не назначалось новое сообщение,
		// пока он еще не обработал предыдущее.
		channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

		// Регистрируем потребителя
		var consumer = new AsyncEventingBasicConsumer(channel);
		// Устанавливаем колбек для события "сообщение пришло из очереди"
		consumer.Received += async (sender, eventArgs) =>
		{
			var body = eventArgs.Body.ToArray();
			var taskId = Encoding.UTF8.GetString(body); // сообщение, которое пришло
			_logger.LogInformation($"Received {taskId}");

			// Отметим в Redis, что взяли задачу
			using var redis = await ConnectionMultiplexer.ConnectAsync(_configuration["Redis"]);
			var db = redis.GetDatabase(0);

			var taskString = db.StringGet(taskId);
			var task = JsonConvert.DeserializeObject<WorkerTask>(taskString);
			if (task == null) throw new ArgumentNullException(nameof(task));

			task.Status = WorkerTaskStatus.Processing;
			db.StringSet(taskId, JsonConvert.SerializeObject(task));

			try
			{
				_logger.LogInformation($"Start processing task {task.TaskId}");
				var result = await ProcessTaskAsync(task.WorkItem, cancellationToken);
				_logger.LogInformation($"End processing task {task.TaskId}");
				task.Result = result;
				task.Status = WorkerTaskStatus.Success;

				// По умолчанию сообщение считается доставленным, как только оно отправлено.
				// Однако воркер может упасть посреди исполнения задачи, в этом случае нужно отправить ее на другой
				// воркер. Поэтому будем вручную указывать, что сообщение доставлено.
				channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
			}
			catch (Exception e)
			{
				_logger.LogError($"Task {taskId} failed: {e}");
				task.Status = WorkerTaskStatus.Failed;
			}

			db.StringSet(taskId, JsonConvert.SerializeObject(task));
		};

		channel.BasicConsume(queue: WellKnownNames.QueueName, autoAck: false, consumer: consumer);

		_logger.LogInformation("Subscribed to queue");

		await Task.Delay(Timeout.Infinite, cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private async Task<string> ProcessTaskAsync(object workItem, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"WorkItem: {workItem}");
		await Task.Delay(20000, cancellationToken);

		return "result";
	}
}
