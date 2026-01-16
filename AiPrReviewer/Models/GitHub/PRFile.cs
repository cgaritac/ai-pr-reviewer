using System.Text.Json.Serialization;

public class PRFile
{
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    public PRFile() { }

    public PRFile(string filename, string status, string path)
    {
        Filename = filename;
        Status = status;
        Path = path;
    }
}