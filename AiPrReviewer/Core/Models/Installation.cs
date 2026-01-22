using System.Text.Json.Serialization;

namespace AiPrReviewer.Core.Models.GitHub;

public class Installation
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    public Installation(){}

    public Installation(long id)
    {
        Id = id;
    }
}