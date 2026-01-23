namespace AiPrReviewer.Api.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddGithubHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient("github", client =>
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.Timeout = TimeSpan.FromSeconds(60);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            MaxConnectionsPerServer = 10
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }
}