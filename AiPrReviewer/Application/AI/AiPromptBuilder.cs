using System.Text;
using AiPrReviewer.Core.Models;

namespace AiPrReviewer.Application.AI;

public class AiPromptBuilder
{
    public string BuildPrPrompt(string repoName, int prNumber, IEnumerable<PRFile> files)
    {
        StringBuilder sb = new();

        sb.AppendLine($"Repository: {repoName}");
        sb.AppendLine($"Pull Request: #{prNumber}");
        sb.AppendLine();
        sb.AppendLine("Changed files:");

        foreach (var file in files)
        {
            if (string.IsNullOrWhiteSpace(file.Patch))
                continue;

            sb.AppendLine($"--- {file.Filename} ---");
            sb.AppendLine(file.Patch);
            sb.AppendLine();
        }

        sb.AppendLine();
        sb.AppendLine("Please review the changes above.");

        string prompt = sb.ToString();

        return prompt;
    }
}