using AiPrReviewer.Core.Interfaces;
using AiPrReviewer.Application.AI;

namespace AiPrReviewer.Application.Review;

public class ReviewPipeline(
    IJwtService jwtService,
    IInstallationService installationService,
    IPrService prService,
    IAiReviewer aiReviewer,
    ICommentService commentService,
    AiPromptBuilder promptBuilder,
    AiCommentFormatter aiCommentFormatter,
    ILogger<ReviewPipeline> logger) : IReviewPipeline
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly IInstallationService _installationService = installationService;
    private readonly IPrService _prService = prService;
    private readonly IAiReviewer _aiReviewer = aiReviewer;
    private readonly ICommentService _commentService = commentService;
    private readonly AiPromptBuilder _promptBuilder = promptBuilder;
    private readonly AiCommentFormatter _aiCommentFormatter = aiCommentFormatter;
    private readonly ILogger<ReviewPipeline> _logger = logger;

    public async Task RunAsync(long installationId, int prNumber, string repositoryFullName)
    {
        _logger.LogInformation(
            "Starting review pipeline for installation {InstallationId}, PR {PrNumber}, Repository {RepositoryFullName}",
            installationId,
            prNumber,
            repositoryFullName);

        // 1. Get installation token
        var installationToken = await _installationService.GetInstallationTokenAsync(installationId);

        // 2. Get PR files
        var prFiles = await _prService.GetPRFilesAsync(prNumber, repositoryFullName, installationToken);

        if (!prFiles.Any())
        {
            _logger.LogInformation(
                "No files found in PR {PrNumber} in repository {RepositoryFullName}. Skipping review.",
                prNumber,
                repositoryFullName);
            return;
        }

        // 3. Build AI prompt
        var prompt = _promptBuilder.BuildPrPrompt(repositoryFullName, prNumber, prFiles);

        // 4. Call OpenAI to get review
        var aiFeedback = await _aiReviewer.ReviewPRAsync(prompt);

        if (string.IsNullOrWhiteSpace(aiFeedback))
        {
            _logger.LogWarning(
                "No feedback from OpenAI for PR {PrNumber} in repository {RepositoryFullName}.",
                prNumber,
                repositoryFullName);
            return;
        }

        // 5. Format comment
        var formattedComment = _aiCommentFormatter.FormatComment(aiFeedback);

        // 6. Post comment to GitHub
        await _commentService.PostCommentAsync(repositoryFullName, prNumber, formattedComment, installationToken);

        _logger.LogInformation(
            "Review completed for PR {PrNumber} in repository {RepositoryFullName}. Comment posted successfully.",
            prNumber,
            repositoryFullName);
    }
}