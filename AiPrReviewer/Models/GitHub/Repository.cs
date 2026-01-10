namespace AiPrReviewer.Models.GitHub;

public class Repository
{
    public string FullName { get; set; }

    public Repository(string fullName)
    {
        FullName = fullName;
    }
}