namespace AiPrReviewer.Core.Interfaces;

public interface IPrCommenter
{
    Task<string> CommentPRAsync(string prompt);
}