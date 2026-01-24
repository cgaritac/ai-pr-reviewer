using AiPrReviewer.Core.Models;

namespace AiPrReviewer.Application.Diff;

public class GithubDiffParser
{
    public IEnumerable<PRFile> Normalize(IEnumerable<PRFile> files)
    {
        return files
            .Where(f => !string.IsNullOrWhiteSpace(f.Patch))
            .Where(f => f.Filename != null && !f.Filename.EndsWith(".lock"))
            .Select(f => new PRFile
            {
                Filename = f.Filename,
                Patch = f.Patch!.Length > 5000
                    ? f.Patch[..5000]
                    : f.Patch
            });
    }
}