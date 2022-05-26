using System.Text;
using InferenceService.Contracts;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var redisConnectionString = app.Configuration["Redis"];
var rabbitMqHost = app.Configuration["RabbitMQ"];

app.MapGet("/upload", () =>
{
	var taskId = Guid.NewGuid();
	var task = new WorkerTask
	{
		TaskId = taskId,
		WorkItem = "WorkItem",
		Status = WorkerTaskStatus.Created
	};

	var factory = new ConnectionFactory
	{
		HostName = rabbitMqHost,
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

	// Указываем, что сообщения не должны удаляться (они сохраняются на диск или в кеш)
	var properties = channel.CreateBasicProperties();
	properties.Persistent = true;

	// Кладем задачу в Redis
	using var redis = ConnectionMultiplexer.Connect(redisConnectionString);
	var db = redis.GetDatabase(0);
	var taskString = JsonConvert.SerializeObject(task);
	db.StringSet(taskId.ToString(), taskString);

	var body = Encoding.UTF8.GetBytes(taskId.ToString());
	channel.BasicPublish(
		exchange: string.Empty, // exchange распределяет сообщения по очередям
		routingKey: WellKnownNames.QueueName,
		basicProperties: properties,
		body: body);

	return taskId;
});

app.MapGet("/status/{taskId}", (string taskId) =>
{
	using var redis = ConnectionMultiplexer.Connect(redisConnectionString);
	var statusDb = redis.GetDatabase(0);

	var taskString = statusDb.StringGet(taskId);
	if (!taskString.HasValue)
	{
		return Results.NotFound();
	}

	var task = JsonConvert.DeserializeObject<WorkerTask>(taskString);
	if (task == null) throw new ArgumentException(nameof(task));

	return Results.Ok(task.Status.ToString());
});

app.Run();
