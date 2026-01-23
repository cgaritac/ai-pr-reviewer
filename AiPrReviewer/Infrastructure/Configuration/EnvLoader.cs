using DotNetEnv;

namespace AiPrReviewer.Infrastructure.Configuration;

public static class EnvLoader
{
    public static void LoadEnv()
    {
        var projectDir = Directory.GetCurrentDirectory();
        var envPath = Path.Combine(projectDir, ".env");

        if (!File.Exists(envPath))
        {
            var parentDir = Directory.GetParent(projectDir)?.FullName;
            if (parentDir != null)
                envPath = Path.Combine(parentDir, ".env");
        }

        if (File.Exists(envPath))
        {
            try
            {
                Env.Load(envPath);
            }
            catch
            {
                var lines = File.ReadAllLines(envPath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                        continue;

                    var equalIndex = line.IndexOf('=');
                    if (equalIndex <= 0) continue;

                    var key = line.Substring(0, equalIndex).Trim();
                    var value = line.Substring(equalIndex + 1).Trim();

                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }
        else
        {
            Env.Load();
        }
    }
}