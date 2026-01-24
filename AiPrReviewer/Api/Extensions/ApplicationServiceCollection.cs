using AiPrReviewer.Application.AI;
using AiPrReviewer.Application.Review;
using AiPrReviewer.Application.Diff;
using AiPrReviewer.Application.Rules;
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
        services.AddScoped<GithubDiffParser>();
        services.AddScoped<BasicPrRules>();
        services.AddScoped<IAiReviewer, OpenAiReviewService>();

        return services;
    }
}