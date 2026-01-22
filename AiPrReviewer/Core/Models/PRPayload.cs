using System.Text.Json.Serialization;

namespace AiPrReviewer.Core.Models.GitHub;

public class PRPayload
{
    public string? Action { get; set; }
    
    [JsonPropertyName("pull_request")]
    public PullRequest? PullRequest { get; set; }
    
    public Repository? Repository { get; set; }

    public PRPayload()
    {
    }

    public PRPayload(string action, PullRequest pullRequest, Repository repository)
    {
        Action = action;
        PullRequest = pullRequest;
        Repository = repository;
    }
}