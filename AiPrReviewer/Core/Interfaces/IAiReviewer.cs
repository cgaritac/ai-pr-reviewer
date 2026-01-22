namespace AiPrReviewer.Core.Interfaces;

public interface IAiReviewer
{
    Task<string> ReviewPRAsync(string prompt);
}