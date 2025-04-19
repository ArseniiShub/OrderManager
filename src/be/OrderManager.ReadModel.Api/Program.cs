using System.Text.Json.Serialization;
using JasperFx.Core;
using OrderManager.ReadModel.Api;
using OrderManager.ReadModel.Api.Orders;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

const string uiOrigins = "UIOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!;
builder.Services.AddSingleton<INpgsqlConnectionFactory>(new NpgsqlConnectionFactory(connectionString));
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<DatabaseMigrator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(uiOrigins, policy => policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
});

builder.Host.UseWolverine(options =>
{
    options.Policies.OnAnyException().RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());

    options.UseRabbitMq(new Uri(Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")!))
        .DeclareExchange("eventstream", exchange => { exchange.BindQueue("readmodel"); })
        .AutoProvision();

    options.Policies.UseDurableInboxOnAllListeners();

    options.ListenToRabbitQueue("readmodel");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    var migrator = app.Services.GetRequiredService<DatabaseMigrator>();
    await migrator.Migrate();
}

app.UseHttpsRedirection();

app.UseCors(uiOrigins);

app.MapGet("/api/orders", async (IOrderRepository orderRepository) =>
{
    var orders = await orderRepository.GetAll();
    return Results.Ok(new { orders });
});

app.MapGet("/api/orders/{id:guid}", async (Guid id, IOrderRepository orderRepository) =>
{
    var order = await orderRepository.Get(id);
    return order == null ? Results.NotFound() : Results.Ok(order);
});

app.Run();