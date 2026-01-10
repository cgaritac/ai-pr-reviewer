using System.Text.Json;

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
        Console.WriteLine("Webhook received: from GitHub");
        Console.WriteLine(body);
    }

    return Results.Ok();
}).WithName("GitHubWebhook");

app.Run();