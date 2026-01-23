using AiPrReviewer.Core.Models;

namespace AiPrReviewer.Core.Interfaces;

public interface IPrService
{
    Task<IReadOnlyList<PRFile>> GetPRFilesAsync(
        int prNumber,
        string repositoryFullName,
        string installationToken);
}