using AiPrReviewer.Api.Extensions;
using AiPrReviewer.Api.Debugs;
using AiPrReviewer.Api.Webhooks;
using AiPrReviewer.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;

EnvLoader.LoadEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices().AddInfrastructureServices().AddGithubHttpClient();

builder.Services.AddScoped<WebhookHandler>();
builder.Services.AddScoped<DebugJwtHandler>();
builder.Services.AddScoped<DebugInstallationHandler>();
builder.Services.AddScoped<DebugInstallationsListHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var debug = app.MapGroup("/debug");

debug.MapGet("/jwt", async ([FromServices] DebugJwtHandler handler) =>
{
    return await handler.HandleAsync();
}).WithName("DebugJwt");

debug.MapGet("/installations", async ([FromServices] DebugInstallationsListHandler handler) =>
{
    return await handler.HandleAsync();
}).WithName("DebugInstallations");

debug.MapGet("/installation-token/{id:long}", async (
    long id,
    [FromServices] DebugInstallationHandler handler) =>
{
    return await handler.HandleAsync(id);
}).WithName("DebugInstallationToken");

app.MapPost("/webhooks/github", async (
    HttpRequest request,
    [FromServices] WebhookHandler handler) =>
{
    return await handler.HandleAsync(request);
})
.WithName("GitHubWebhook");

app.Run();