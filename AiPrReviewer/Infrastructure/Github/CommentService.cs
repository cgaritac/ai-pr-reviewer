using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AiPrReviewer.Infrastructure.Github;

public class CommentService(IHttpClientFactory httpClientFactory)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task PostCommentAsync(string repoFullName, int prNumber, string comment, string installationToken)
    {
        var client = _httpClientFactory.CreateClient("github");

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", installationToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd("AiPrReviewer/1.0");

        var payload = new
        {
            body = comment
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync(
            $"https://api.github.com/repos/{repoFullName}/issues/{prNumber}/comments",
            content
        );

        response.EnsureSuccessStatusCode();
    }
}