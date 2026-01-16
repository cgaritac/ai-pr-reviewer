using System.Text.Json;
using AiPrReviewer.Models.GitHub;
using AiPrReviewer.Services.Github;
using DotNetEnv;

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
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<InstallationService>();
builder.Services.AddSingleton<PRService>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/debug/jwt", (JwtService jwtService) =>
{
    var jwt = jwtService.GenerateJwtToken();
    return Results.Ok(jwt);
}).WithName("DebugJwt");

app.MapGet("/debug/installation-token/{id:long}",
    async (long id, InstallationService service) =>
{
    try
    {
        var token = await service.GetInstallationToken(id);
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
    JwtService jwtService,
    IHttpClientFactory httpClientFactory
) =>
{
    var gitHubEvent = request.Headers["X-GitHub-Event"].ToString();

    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    Console.WriteLine($"GitHub Event: {gitHubEvent}");
    if (body.Length > 0)
    {
        var preview = body.Length > 500 ? body.Substring(0, 500) + "..." : body;
        Console.WriteLine($"Payload preview: {preview}");
    }

    if (gitHubEvent == "pull_request")
    {
        var payload = JsonSerializer.Deserialize<PRWebhookPayload>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (payload == null)
        {
            Console.WriteLine("Invalid payload");
            return Results.BadRequest("Invalid payload");
        }

        if (payload?.Installation == null)
        {
            Console.WriteLine("Installation is null in payload");
            return Results.BadRequest("Installation is missing in payload");
        }

        if (payload.PullRequest == null)
        {
            Console.WriteLine("PullRequest is null in payload");
            return Results.BadRequest("PullRequest is missing in payload");
        }

        if (payload.Repository == null)
        {
            Console.WriteLine("Repository is null in payload");
            return Results.BadRequest("Repository is missing in payload");
        }

        //Console.WriteLine($"Pull action: {payload.Action}");
        Console.WriteLine($"Pull request: {payload.PullRequest.Title}");
        Console.WriteLine($"Repository: {payload.Repository.FullName}");
        Console.WriteLine($"Installation: {payload.Installation.Id}");

        if (payload.PullRequest.Merged)
        {
            Console.WriteLine($"Pull request {payload.PullRequest.Id} was merged");
        }
    }

    return Results.Ok();
}).WithName("GitHubWebhook");

app.Run();