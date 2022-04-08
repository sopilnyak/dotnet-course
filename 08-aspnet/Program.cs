using Aspnet.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Здесь добавляем сторонние сервисы

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<CountryContext>(opt => opt.UseInMemoryDatabase("Countries"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/add/{a}/{b}", (int a, int b) => Results.Ok(a + b))
    .Produces<int>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);

app.Use(async (context, next) => await next(context));

app.MapControllers();

app.Run();
