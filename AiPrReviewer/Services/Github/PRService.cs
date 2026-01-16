using AiPrReviewer.Models.GitHub;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AiPrReviewer.Services.Github;

public class PRService(IHttpClientFactory httpClientFactory)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<List<PRFile>> GetPRFilesAsync(int prNumber, string installationToken, string repositoryName)
    {
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", installationToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        var response = await client.GetAsync($"https://api.github.com/repos/{repositoryName}/pulls/{prNumber}/files");

        response.EnsureSuccessStatusCode();

        var jsonContent = await response.Content.ReadAsByteArrayAsync();

        return JsonSerializer.Deserialize<List<PRFile>>(
            jsonContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? new List<PRFile>();
    }
}