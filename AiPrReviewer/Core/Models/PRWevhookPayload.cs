using System.Text.Json.Serialization;

namespace AiPrReviewer.Core.Models.GitHub;

public class PRWebhookPayload
{
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("installation")]
    public Installation? Installation { get; set; }

    [JsonPropertyName("pull_request")]
    public PullRequest? PullRequest { get; set; }

    public Repository? Repository { get; set; }

    public PRWebhookPayload()
    {
    }

    public PRWebhookPayload(Installation installation, PullRequest pullRequest, Repository repository)
    {
        Installation = installation;
        PullRequest = pullRequest;
        Repository = repository;
    }
}