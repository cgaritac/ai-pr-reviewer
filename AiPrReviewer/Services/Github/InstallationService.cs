using System.Net.Http.Headers;
using System.Text.Json;

namespace AiPrReviewer.Services.Github;

public class InstallationService(JwtService jwtService, IHttpClientFactory httpClientFactory)
{
    private readonly JwtService _jwtService = jwtService;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<string> GetInstallationToken(long installationId)
    {
        var token = _jwtService.GenerateJwtToken();

        var client = _httpClientFactory.CreateClient("github");
        
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github+json")
        );

        client.DefaultRequestHeaders.UserAgent.ParseAdd("AiPrReviewer/1.0");

        var response = await client.PostAsync(
            $"https://api.github.com/app/installations/{installationId}/access_tokens",
            null
        );

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            var errorMessage = $"GitHub API error ({statusCode}): {errorContent}";
            
            Console.WriteLine($"Error getting installation token:");
            Console.WriteLine($"Status Code: {statusCode}");
            Console.WriteLine($"Response: {errorContent}");
            
            throw new HttpRequestException(errorMessage);
        }

        var jsonContent = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonContent);

        return jsonDoc.RootElement.GetProperty("token").GetString() ?? throw new Exception("Failed to get installation token");
    }
}