using AiPrReviewer.Core.Interfaces;

namespace AiPrReviewer.Api.Debugs;

public class DebugInstallationHandler(IInstallationService installationService, ILogger<DebugInstallationHandler> logger)
{
    private readonly IInstallationService _installationService = installationService;
    private readonly ILogger<DebugInstallationHandler> _logger = logger;

    public async Task<IResult> HandleAsync(long installationId)
    {
        try
        {
            var token = await _installationService.GetInstallationTokenAsync(installationId);
            return Results.Ok(new { token });
        }
        catch (HttpRequestException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Error getting installation token"
            );
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: $"Unexpected error: {ex.Message}\nStack trace: {ex.StackTrace}",
                statusCode: 500,
                title: "Unexpected error"
            );
        }
    }
}