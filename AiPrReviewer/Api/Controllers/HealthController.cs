using Microsoft.AspNetCore.Mvc;

namespace AiPrReviewer.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new {
            status = "healthy",
            service = "AiPrReviewer",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            uptime = TimeSpan.FromMilliseconds(Environment.TickCount64),
            memory = new {
                total = GC.GetTotalMemory(false),
                available = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes
            },
            cpu = new {
                usage = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds
            },
            timestamp = DateTime.UtcNow });
    }
}