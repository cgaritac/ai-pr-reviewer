using System.Net.Http.Headers;
using System.Text.Json;
using AiPrReviewer.Core.Interfaces;

namespace AiPrReviewer.Api.Debugs;

public class DebugInstallationsListHandler(IJwtService jwtService, IHttpClientFactory httpClientFactory, ILogger<DebugInstallationsListHandler> logger)
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<DebugInstallationsListHandler> _logger = logger;

    public async Task<IResult> HandleAsync()
    {
        try
        {
            var jwt = _jwtService.GenerateJwtToken();
            var client = _httpClientFactory.CreateClient("github");
            
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("AiPrReviewer/1.0");

            var response = await client.GetAsync("app/installations");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to get installations ({StatusCode}): {Content}", response.StatusCode, content);
                return Results.Problem(
                    detail: $"Failed to get installations ({response.StatusCode}): {content}",
                    statusCode: (int)response.StatusCode,
                    title: "Error getting installations"
                );
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var installations = new List<object>();

            foreach (var installation in doc.RootElement.EnumerateArray())
            {
                installations.Add(new
                {
                    id = installation.TryGetProperty("id", out var id) ? id.GetInt64() : 0,
                    account = installation.TryGetProperty("account", out var account) 
                        ? account.TryGetProperty("login", out var login) ? login.GetString() : null 
                        : null,
                    repositorySelection = installation.TryGetProperty("repository_selection", out var repoSel) 
                        ? repoSel.GetString() 
                        : null,
                    targetType = installation.TryGetProperty("target_type", out var targetType) 
                        ? targetType.GetString() 
                        : null
                });
            }

            return Results.Ok(new { installations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting installations");
            return Results.Problem(
                detail: $"Unexpected error: {ex.Message}\nStack trace: {ex.StackTrace}",
                statusCode: 500,
                title: "Unexpected error"
            );
        }
    }
}
