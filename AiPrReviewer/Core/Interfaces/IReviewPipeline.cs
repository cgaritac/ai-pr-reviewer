namespace AiPrReviewer.Core.Interfaces;

public interface IReviewPipeline
{
    Task RunAsync(long installationId, int prNumber, string repositoryFullName);
}