using AiPrReviewer.Core.Interfaces;

namespace AiPrReviewer.Api.Debugs;

public class DebugJwtHandler(IJwtService jwtService, ILogger<DebugJwtHandler> logger)
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<DebugJwtHandler> _logger = logger;

    public Task<IResult> HandleAsync()
    {
        try
        {
            var jwt = _jwtService.GenerateJwtToken();
            return Task.FromResult<IResult>(Results.Ok(jwt));
        }
        catch (Exception ex)
        {
            return Task.FromResult<IResult>(Results.Problem(
                detail: $"Unexpected error: {ex.Message}\nStack trace: {ex.StackTrace}",
                statusCode: 500,
                title: "Unexpected error"
            ));
        }
    }
}