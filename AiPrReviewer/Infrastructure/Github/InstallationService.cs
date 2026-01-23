using System.Net.Http.Headers;
using System.Text.Json;
using AiPrReviewer.Core.Interfaces;

namespace AiPrReviewer.Infrastructure.Github;

public class InstallationService(IJwtService jwtService, IHttpClientFactory httpClientFactory) : IInstallationService
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<string> GetInstallationTokenAsync(long installationId)
    {
        var jwt = _jwtService.GenerateJwtToken();
        var client = _httpClientFactory.CreateClient("github");
        
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);

        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github+json")
        );

        client.DefaultRequestHeaders.UserAgent.ParseAdd("AiPrReviewer/1.0");

        var response = await client.PostAsync(
            $"app/installations/{installationId}/access_tokens",
            null
        );

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get installation token ({response.StatusCode}): {content}");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("token")
            .GetString()
            ?? throw new InvalidOperationException("GitHub did not return an installation token");
    }
}