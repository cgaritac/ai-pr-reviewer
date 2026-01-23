namespace AiPrReviewer.Core.Interfaces;

public interface IPrAnalizer
{
    Task<string> AnalyzePRAsync(string prompt);
}