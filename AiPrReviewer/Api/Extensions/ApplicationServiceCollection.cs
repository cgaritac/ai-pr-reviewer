using AiPrReviewer.Application.AI;
using AiPrReviewer.Application.Review;
using AiPrReviewer.Core.Interfaces;
using AiPrReviewer.Infrastructure.OpeAI;

namespace AiPrReviewer.Api.Extensions;

public static class ApplicationServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IReviewPipeline, ReviewPipeline>();
        services.AddScoped<AiPromptBuilder>();
        services.AddScoped<AiCommentFormatter>();
        services.AddScoped<IAiReviewer, OpenAiReviewService>();

        return services;
    }
}