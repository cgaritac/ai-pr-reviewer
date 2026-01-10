using System.Text.Json;
using AiPrReviewer.Models.GitHub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/webhooks/github", async (HttpRequest request) =>
{
    var gitHubEvent = request.Headers["X-GitHub-Event"].ToString();

    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    Console.WriteLine($"GitHub Event: {gitHubEvent}");

    if (gitHubEvent == "pull_request")
    {
        var payload = JsonSerializer.Deserialize<PRPayload>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (payload == null)
        {
            Console.WriteLine("Invalid payload");
            return Results.BadRequest("Invalid payload");
        }

        Console.WriteLine($"Pull action: {payload.Action}");
        Console.WriteLine($"Pull request: {payload.PullRequest.Title}");
        Console.WriteLine($"Repository: {payload.Repository.FullName}");

        if (payload.PullRequest.Merged)
        {
            Console.WriteLine($"Pull request {payload.PullRequest.Id} was merged");
        }
    }

    return Results.Ok();
}).WithName("GitHubWebhook");

app.Run();