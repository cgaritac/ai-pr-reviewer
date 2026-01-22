using System.Text.Json.Serialization;

namespace AiPrReviewer.Core.Models.GitHub;

public class PullRequest
{
    public long Id { get; set; }
    
    [JsonPropertyName("number")]
    public int Number { get; set; }
    
    public bool Merged { get; set; }
    public string? Title { get; set; }

    public PullRequest()
    {
    }

    public PullRequest(long id, bool merged, string title)
    {
        Id = id;
        Merged = merged;
        Title = title;
    }
}