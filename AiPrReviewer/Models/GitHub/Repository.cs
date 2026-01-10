using System.Text.Json.Serialization;

namespace AiPrReviewer.Models.GitHub;

public class Repository
{
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    public Repository()
    {
    }

    public Repository(string fullName)
    {
        FullName = fullName;
    }
}