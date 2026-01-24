using AiPrReviewer.Core.Models;

namespace AiPrReviewer.Application.Rules;

public class BasicPrRules
{
    public IEnumerable<string> Evaluate(IEnumerable<PRFile> files)
    {
        var warnings = new List<string>();

        if (files.Sum(f => f.Patch?.Length ?? 0) > 10_000)
            warnings.Add("Large PR: consider splitting into smaller changes.");

        if (files.Any(f => f.Filename != null && f.Filename.Contains("Program.cs")))
            warnings.Add("Changes in Program.cs detected. Ensure startup logic is correct.");

        return warnings;
    }
}