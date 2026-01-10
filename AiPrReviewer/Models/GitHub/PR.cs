namespace AiPrReviewer.Models.GitHub;

public class PullRequest
{
    public int Id { get; set; }
    public bool Merged { get; set; }
    public string Title { get; set; }

    public PullRequest(int id, bool merged, string title)
    {
        Id = id;
        Merged = merged;
        Title = title;
    }
}