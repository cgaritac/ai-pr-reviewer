namespace AiPrReviewer.Models.GitHub;

public class PRPayload
{
    public string Action { get; set; }
    public PullRequest PullRequest { get; set; }
    public Repository Repository { get; set; }

    public PRPayload(string action, PullRequest pullRequest, Repository repository)
    {
        Action = action;
        PullRequest = pullRequest;
        Repository = repository;
    }
}