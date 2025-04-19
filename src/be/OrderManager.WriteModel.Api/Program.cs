using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OrderManager.WriteModel.Api;
using OrderManager.WriteModel.Api.Extensions;
using OrderManager.WriteModel.Api.Orders.Requests;
using OrderManager.WriteModel.Application;
using OrderManager.WriteModel.Application.Orders;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using OrderManager.WriteModel.Application.Orders.Commands;
using Scalar.AspNetCore;

const string uiOrigins = "UIOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Environment);

builder.Services.AddCors(options =>
{
    options.AddPolicy(uiOrigins, policy => policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Authority = builder.Configuration["Authentication:Authority"];
        o.Audience = builder.Configuration["Authentication:Audience"];
        o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddOpenApi(options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithPreferredScheme("oauth2")
            .WithOAuth2Authentication(oauth =>
            {
                oauth.ClientId = builder.Configuration["Keycloak:ClientId"];
                oauth.Scopes = ["openid", "profile"];
            });
    });
}

app.UseHttpsRedirection();

app.UseCors(uiOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/orders", async (OrderService service, CreateOrderRequest request) =>
{
    var result = await service.CreateOrder(request.ToCommand());

    return result.Match(() => Results.Created("TODO path to RM", result.Value), CustomResults.Problem);
}).RequireAuthorization();

app.MapPatch("/api/orders/{orderId:guid}",
    async (OrderService service, Guid orderId, UpdateOrderRequest request) =>
    {
        var result = await service.UpdateOrder(request.ToCommand(orderId));

        return result.Match(Results.NoContent, CustomResults.Problem);
    }).RequireAuthorization();

app.MapPost("/api/orders/{orderId:guid}/dispatch", async (OrderService service, Guid orderId) =>
{
    var result = await service.Dispatch(new DispatchCommand(orderId));

    return result.Match(Results.NoContent, CustomResults.Problem);
}).RequireAuthorization();

app.MapPost("/api/orders/{orderId:guid}/outfordelivery", async (OrderService service, Guid orderId) =>
{
    var result = await service.OutForDelivery(new OutForDeliveryCommand(orderId));

    return result.Match(Results.NoContent, CustomResults.Problem);
}).RequireAuthorization();

app.MapPost("/api/orders/{orderId:guid}/deliver", async (OrderService service, Guid orderId) =>
{
    var result = await service.Deliver(new DeliverCommand(orderId));

    return result.Match(Results.NoContent, CustomResults.Problem);
}).RequireAuthorization();

app.MapPost("/api/orders/{orderId:guid}/archive", async (OrderService service, Guid orderId) =>
{
    var result = await service.Archive(new ArchiveCommand(orderId));

    return result.Match(Results.NoContent, CustomResults.Problem);
}).RequireAuthorization();

app.MapPost("/api/orders/{orderId:guid}/restore", async (OrderService service, Guid orderId) =>
{
    var result = await service.Restore(new RestoreCommand(orderId));

    return result.Match(Results.NoContent, CustomResults.Problem);
}).RequireAuthorization();

app.Run();