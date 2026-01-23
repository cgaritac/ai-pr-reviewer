namespace AiPrReviewer.Core.Interfaces;

public interface IInstallationService
{
    Task<string> GetInstallationTokenAsync(long installationId);
}