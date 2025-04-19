using JasperFx.Core;
using Marten;
using Marten.Events.Projections;
using OrderManager.WriteModel.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderManager.WriteModel.Application.Orders;
using Weasel.Core;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Marten;
using Wolverine.RabbitMQ;

namespace OrderManager.WriteModel.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddMarten(options =>
            {
                options.DisableNpgsqlLogging = true;

                options.Connection(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!);
                options.UseSystemTextJsonForSerialization();
                options.Projections.Snapshot<Order>(SnapshotLifecycle.Inline);

                if (environment.IsDevelopment())
                {
                    options.AutoCreateSchemaObjects = AutoCreate.All;
                }
            })
            .UseLightweightSessions()
            .IntegrateWithWolverine();

        services.AddWolverine(options =>
        {
            options.Policies.OnAnyException().RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());

            options.UseRabbitMq(new Uri(Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")!))
                .AutoProvision();

            options.PublishAllMessages()
                .ToRabbitExchange("eventstream")
                .UseDurableOutbox();
        });

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<OrderService>();
    }
}