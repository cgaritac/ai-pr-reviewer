using AiPrReviewer.Core.Interfaces;
using AiPrReviewer.Infrastructure.Github;

namespace AiPrReviewer.Api.Extensions;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IJwtService, JwtService>();
        services.AddScoped<IInstallationService, InstallationService>();
        services.AddScoped<IPrService, PrService>();
        services.AddScoped<ICommentService, CommentService>();

        return services;
    }
}