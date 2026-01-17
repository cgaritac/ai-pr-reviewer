using System.Text.Json.Serialization;

namespace AiPrReviewer.Models.GitHub;

public class PRFile
{
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("path")]
    public string? Patch { get; set; }

    public PRFile() { }

    public PRFile(string filename, string status, string patch)
    {
        Filename = filename;
        Status = status;
        Patch = patch;
    }
}