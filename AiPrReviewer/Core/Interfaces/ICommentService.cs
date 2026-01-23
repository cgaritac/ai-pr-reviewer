namespace AiPrReviewer.Core.Interfaces;

public interface ICommentService
{
    Task PostCommentAsync(string repoFullName, int prNumber, string comment, string installationToken);
}