using OpenAI.Chat;

namespace AiPrReviewer.Services.AI;

public class OpenAiReviewService
{
    private readonly string _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        ?? throw new Exception("OPENAI_API_KEY is not set. Configure it in the .env file or as an environment variable.");
    private readonly string _model = Environment.GetEnvironmentVariable("OPENAI_MODEL")
        ?? throw new Exception("OPENAI_MODEL is not set. Configure it in the .env file or as an environment variable.");

    public async Task<string> ReviewPRAsync(string prompt)
    {
        var client = new ChatClient(_model, _apiKey);

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("""
                You are a senior software engineer performing a pull request code review.
                Provide concise, actionable feedback.
                Focus on bugs, security, performance, readability, and best practices.
                Only comment on issues that matter.
                If everything looks good, say so.
            """),

            new UserChatMessage(prompt)
        };

        var response = await client.CompleteChatAsync(messages);

        return response.Value.Content[0].Text;
    }
}