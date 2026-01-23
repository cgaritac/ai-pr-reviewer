using AiPrReviewer.Core.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AiPrReviewer.Infrastructure.Github;

public class PrService(IHttpClientFactory httpClientFactory)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<List<PRFile>> GetPRFilesAsync(int prNumber, string installationToken, string repositoryName)
    {
        var client = _httpClientFactory.CreateClient("github");

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", installationToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd("AiPrReviewer/1.0");

        var response = await client.GetAsync($"repos/{repositoryName}/pulls/{prNumber}/files");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            var errorMessage = $"GitHub API error ({statusCode}): {errorContent}";
            
            Console.WriteLine($"Error getting PR files:");
            Console.WriteLine($"Status Code: {statusCode}");
            Console.WriteLine($"Response: {errorContent}");
            Console.WriteLine($"Repository: {repositoryName}");
            Console.WriteLine($"PR Number: {prNumber}");
            
            throw new HttpRequestException(errorMessage);
        }

        var jsonContent = await response.Content.ReadAsByteArrayAsync();

        return JsonSerializer.Deserialize<List<PRFile>>(
            jsonContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? new List<PRFile>();
    }
}