using AiPrReviewer.Core.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using AiPrReviewer.Core.Interfaces;

namespace AiPrReviewer.Infrastructure.Github;

public class PrService(IHttpClientFactory httpClientFactory, ILogger<PrService> logger) : IPrService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<PrService> _logger = logger;

    public async Task<IReadOnlyList<PRFile>> GetPRFilesAsync(int prNumber, string repositoryFullName, string installationToken)
    {
        var client = _httpClientFactory.CreateClient("github");

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", installationToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd("AiPrReviewer/1.0");

        var response = await client.GetAsync($"repos/{repositoryFullName}/pulls/{prNumber}/files");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            _logger.LogError(
                "Error fetching PR files. Repo={Repo}, PR={Pr}, Status={Status}, Body={Body}",
                repositoryFullName,
                prNumber,
                response.StatusCode,
                error);

            throw new HttpRequestException(
                $"GitHub API error {response.StatusCode}: {error}");
        }

        var jsonContent = await response.Content.ReadAsByteArrayAsync();

        return JsonSerializer.Deserialize<List<PRFile>>(
            jsonContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? new List<PRFile>();
    }
}