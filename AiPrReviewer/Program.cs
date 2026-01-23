using AiPrReviewer.Core.Interfaces;
using AiPrReviewer.Application.AI;
using AiPrReviewer.Infrastructure.OpeAI;
using AiPrReviewer.Infrastructure.Github;
using AiPrReviewer.Application.Review;
using AiPrReviewer.Api.Webhooks;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;

// .env file loading and validation
var projectDir = Directory.GetCurrentDirectory();
var envPath = Path.Combine(projectDir, ".env");

if (!File.Exists(envPath))
{
    var parentDir = Directory.GetParent(projectDir)?.FullName;
    if (parentDir != null)
    {
        envPath = Path.Combine(parentDir, ".env");
    }
}

if (File.Exists(envPath))
{
    try
    {
        Env.Load(envPath);
    }
    catch
    {
        var lines = File.ReadAllLines(envPath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;
            
            var equalIndex = line.IndexOf('=');
            if (equalIndex <= 0) continue;
            
            var key = line.Substring(0, equalIndex).Trim();
            var value = line.Substring(equalIndex + 1).Trim();
            
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
else
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<WebhookHandler>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IInstallationService, InstallationService>();
builder.Services.AddScoped<IPrService, PrService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<AiPromptBuilder>();
builder.Services.AddScoped<IAiReviewer, OpenAiReviewService>();
builder.Services.AddScoped<AiCommentFormatter>();

builder.Services.AddScoped<IReviewPipeline, ReviewPipeline>();

builder.Services.AddHttpClient("github", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        MaxConnectionsPerServer = 10
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/debug/jwt", ([FromServices] IJwtService jwtService) =>
{
    var jwt = jwtService.GenerateJwtToken();
    return Results.Ok(jwt);
}).WithName("DebugJwt");

app.MapGet("/debug/installation-token/{id:long}",
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