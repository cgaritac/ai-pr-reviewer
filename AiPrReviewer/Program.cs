using AiPrReviewer.Core.Interfaces;
using AiPrReviewer.Api.Extensions;
using AiPrReviewer.Infrastructure.Configuration;
using AiPrReviewer.Api.Webhooks;
using Microsoft.AspNetCore.Mvc;

EnvLoader.LoadEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices().AddInfrastructureServices().AddGithubHttpClient();

builder.Services.AddScoped<WebhookHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var debug = app.MapGroup("/debug");

debug.MapGet("/jwt", ([FromServices] IJwtService jwtService) =>
{
    var jwt = jwtService.GenerateJwtToken();
    return Results.Ok(jwt);
}).WithName("DebugJwt");

debug.MapGet("/installation-token/{id:long}",
    async (long id, [FromServices] IInstallationService service) =>
{
    try
    {
        var token = await service.GetInstallationTokenAsync(id);
        return Results.Ok(new { token });
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Error getting installation token"
        );
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: $"Unexpected error: {ex.Message}\nStack trace: {ex.StackTrace}",
            statusCode: 500,
            title: "Unexpected error"
        );
    }
}).WithName("DebugInstallationToken");

app.MapPost("/webhooks/github", async (
    HttpRequest request,
    WebhookHandler handler) =>
{
    return await handler.HandleAsync(request);
})
.WithName("GitHubWebhook");

app.Run();