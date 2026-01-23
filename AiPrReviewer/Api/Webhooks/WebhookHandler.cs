using System.Text.Json;
using AiPrReviewer.Core.Interfaces;
using AiPrReviewer.Core.Models;

namespace AiPrReviewer.Api.Webhooks;

public class WebhookHandler(IReviewPipeline reviewPipeline, ILogger<WebhookHandler> logger)
{
    private readonly IReviewPipeline _reviewPipeline = reviewPipeline;
    private readonly ILogger<WebhookHandler> _logger = logger;

    public async Task<IResult> HandleAsync(HttpRequest request)
    {
        var githubEvent = request.Headers["X-GitHub-Event"].ToString();
        
        // Only process pull_request events
        if (githubEvent != "pull_request")
        {
            _logger.LogInformation("Ignoring event type: {EventType}", githubEvent);
            return Results.Ok($"Event type '{githubEvent}' ignored");
        }

        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();

        var payload = JsonSerializer.Deserialize<PRWebhookPayload>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (payload?.PullRequest == null || payload?.Installation == null || payload?.Repository == null || string.IsNullOrWhiteSpace(payload.Repository.FullName)) {
            _logger.LogError("Invalid payload: {Payload}", payload);
            return Results.BadRequest("Invalid payload");
        }

        if (payload.Action != "opened") {
            _logger.LogInformation("Pull request action '{Action}' not 'opened', skipping review", payload.Action);
            return Results.Ok($"PR action '{payload.Action}' not 'opened', skipping review");
        }

        try
        {
            var repositoryFullName = payload.Repository.FullName!;
            await _reviewPipeline.RunAsync(
                payload.Installation.Id,
                payload.PullRequest.Number,
                repositoryFullName);

            _logger.LogInformation(
                "Review completed successfully for PR {PrNumber} in repository {RepositoryFullName}",
                payload.PullRequest.Number,
                repositoryFullName);

            return Results.Ok($"Review completed for PR {payload.PullRequest.Number} in repository {repositoryFullName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error processing review for PR {PrNumber} in repository {RepositoryFullName}",
                payload.PullRequest.Number,
                payload.Repository.FullName);
            
            return Results.Problem(
                detail: $"Error processing review: {ex.Message}",
                statusCode: 500,
                title: "Review Processing Error");
        }
    }
}