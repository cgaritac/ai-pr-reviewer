# AI PR Reviewer

Automated Pull Request Code Reviews powered by AI and GitHub Apps

AI PR Reviewer is a GitHub App that automatically analyzes Pull Requests using AI and posts concise, actionable code review feedback directly on GitHub.

Designed to demonstrate real-world integrations, secure GitHub App authentication, and practical AI usage in a production-like backend architecture with clean, layered architecture.

## Key Features

- GitHub App authentication (JWT + Installation Tokens)
- GitHub Webhooks for Pull Request events
- Automatic PR file & diff analysis
- Diff normalization (filters, size limits, excludes lock files)
- Basic PR rules evaluation (large PRs, critical file changes)
- AI-powered code reviews using OpenAI
- Automated PR comments with actionable feedback
- Minimal API with clean, modular architecture
- Layered architecture (API, Application, Core, Infrastructure)
- Debug endpoints for development and testing
- Swagger/OpenAPI documentation

## How It Works

1. A Pull Request is opened in a repository
2. GitHub sends a webhook event to the API endpoint `/webhooks/github`
3. The `WebhookHandler` validates and processes the event
4. The `ReviewPipeline` orchestrates the review process:
   - Authenticates as a GitHub App using JWT
   - Retrieves installation token for the repository
   - Fetches PR files and diffs from GitHub
   - Normalizes files using `GithubDiffParser` (filters empty patches, excludes lock files, limits size)
   - Evaluates basic PR rules using `BasicPrRules` (detects large PRs, critical file changes)
   - Builds an AI-friendly prompt with the normalized changes
   - Sends the prompt to OpenAI for analysis
   - Formats the AI response and rule warnings as a GitHub comment
   - Posts the formatted review comment to the Pull Request

## Architecture Overview

```
GitHub PR Opened
      ↓
GitHub Webhook → /webhooks/github
      ↓
WebhookHandler (API Layer)
      ↓
ReviewPipeline (Application Layer)
      ↓
┌─────────────────────────────────────┐
│ 1. JwtService (Infrastructure)      │
│    → Generate JWT Token             │
│                                     │
│ 2. InstallationService (Infra)      │
│    → Get Installation Token         │
│                                     │
│ 3. PrService (Infrastructure)       │
│    → Fetch PR Files & Diffs         │
│                                     │
│ 4. GithubDiffParser (Application)   │
│    → Normalize Files (filter, limit)│
│                                     │
│ 5. BasicPrRules (Application)       │
│    → Evaluate Basic Rules           │
│                                     │
│ 6. AiPromptBuilder (Application)    │
│    → Build AI Prompt                │
│                                     │
│ 7. OpenAiReviewService (Infra)      │
│    → Get AI Review                  │
│                                     │
│ 8. AiCommentFormatter (Application) │
│    → Format Comment + Warnings      │
│                                     │
│ 9. CommentService (Infrastructure)  │
│    → Post Comment to GitHub         │
└─────────────────────────────────────┘
      ↓
GitHub PR Comment Posted
```

## Tech Stack

### Backend
- **.NET 10.0**
- Minimal APIs
- Dependency Injection
- System.IdentityModel.Tokens.Jwt

### GitHub Integration
- GitHub Apps
- Webhooks
- REST API (Installation Tokens)
- HttpClient with named clients

### AI
- OpenAI API (OpenAI NuGet Package v2.8.0)
- GPT-4o-mini / GPT-4.1-mini (configurable)
- Prompt engineering for code reviews

### Dev & Tooling
- **Swagger/OpenAPI** (Swashbuckle.AspNetCore)
- **DotNetEnv** for environment variable loading
- ngrok (local webhook exposure)
- Environment-based configuration
- Debug endpoints for development

## Project Structure

```
AiPrReviewer/
│
├── Api/                          # API Layer - Endpoints & Handlers
│   ├── Controllers/
│   │   └── HealthController.cs
│   ├── Debugs/                   # Debug endpoints for development
│   │   ├── DebugJwtHandler.cs
│   │   ├── DebugInstallationHandler.cs
│   │   └── DebugInstallationsListHandler.cs
│   ├── Extensions/               # Service registration extensions
│   │   ├── ApplicationServiceCollection.cs
│   │   ├── InfrastructureServiceCollection.cs
│   │   └── HttpClientExtensions.cs
│   └── Webhooks/
│       └── WebhookHandler.cs     # GitHub webhook handler
│
├── Application/                  # Application Layer - Business Logic
│   ├── AI/
│   │   ├── AiPromptBuilder.cs    # Builds prompts for AI
│   │   └── AiCommentFormatter.cs # Formats AI responses
│   ├── Diff/
│   │   └── GithubDiffParser.cs   # Normalizes PR diffs (filters, size limits)
│   ├── Rules/
│   │   └── BasicPrRules.cs       # Evaluates basic PR rules
│   └── Review/
│       └── ReviewPipeline.cs      # Orchestrates review process
│
├── Core/                         # Core Layer - Interfaces & Models
│   ├── Interfaces/
│   │   ├── IAiReviewer.cs
│   │   ├── ICommentService.cs
│   │   ├── IInstallationService.cs
│   │   ├── IJwtService.cs
│   │   ├── IPrService.cs
│   │   └── IReviewPipeline.cs
│   └── Models/
│       ├── Installation.cs
│       ├── PR.cs
│       ├── PRFile.cs
│       ├── PRPayload.cs
│       ├── PRWebhookPayload.cs
│       └── Repository.cs
│
├── Infrastructure/               # Infrastructure Layer - External Services
│   ├── Configuration/
│   │   └── EnvLoader.cs          # Environment variable loader
│   ├── Github/                   # GitHub API implementations
│   │   ├── CommentService.cs
│   │   ├── InstallationService.cs
│   │   ├── JwtService.cs
│   │   └── PrService.cs
│   └── OpeAI/
│       └── OpenAiReviewService.cs # OpenAI integration
│
├── Program.cs                    # Application entry point
├── appsettings.json
└── appsettings.Development.json
```

## Environment Variables

Create a `.env` file in the project root or configure environment variables:

```bash
# GitHub App Configuration
APP_ID=123456
PRIVATE_KEY=-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEA...
-----END RSA PRIVATE KEY-----

# OpenAI Configuration
OPENAI_API_KEY=sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
OPENAI_MODEL=gpt-4o-mini
```

The `EnvLoader` class automatically loads variables from:
1. `.env` file in the project directory
2. `.env` file in the parent directory (if not found in project)
3. System environment variables

⚠️ **Never commit secrets. Use environment variables or secret managers.**

## Local Development

### Prerequisites
- .NET 10.0 SDK
- A GitHub App (create one at https://github.com/settings/apps)
- OpenAI API key
- ngrok (for local webhook testing)

### Setup

1. **Clone and restore dependencies:**
```bash
dotnet restore
```

2. **Configure environment variables:**
Create a `.env` file with your GitHub App credentials and OpenAI API key (see above).

3. **Run the application:**
```bash
dotnet run
```

The API will start on `http://localhost:5288` (or the port configured in `launchSettings.json`).

4. **Expose the API using ngrok:**
```bash
ngrok http 5288
```

5. **Configure GitHub App webhook:**
- Go to your GitHub App settings
- Set the webhook URL to your ngrok URL: `https://your-ngrok-url.ngrok.io/webhooks/github`
- Subscribe to "Pull request" events

6. **Access Swagger UI:**
In development mode, visit `http://localhost:5288/swagger` to explore the API endpoints.

## API Endpoints

### Production Endpoints

- **POST** `/webhooks/github` - GitHub webhook endpoint for PR events

### Debug Endpoints (Development)

- **GET** `/debug/jwt` - Generate and return a JWT token for testing
- **GET** `/debug/installations` - List all GitHub App installations
- **GET** `/debug/installation-token/{id}` - Get installation token for a specific installation ID

## Design Decisions

- **Layered Architecture** (API, Application, Core, Infrastructure) for separation of concerns and testability
- **Minimal API** for simplicity and clarity
- **GitHub App auth** instead of personal tokens (enterprise-ready, scalable)
- **Dependency Injection** with extension methods for clean service registration
- **Diff-based prompts** to reduce token usage and focus on actual changes
- **Diff normalization** to filter irrelevant files and limit token usage
- **Basic PR rules** to catch common issues before AI analysis
- **ReviewPipeline** pattern to orchestrate complex workflows
- **Interface-based design** for easy testing and swapping implementations
- **HttpClient with named clients** for better HTTP client management
- **Environment variable loading** with fallback support for flexible configuration
- **AI as an assistant**, not a replacement for developers

## Example PR Comment

```
**Automated Review Summary**

- Consider validating null inputs in UserService.cs
- Potential performance issue inside the authentication loop
- Naming could be improved for better readability
- Missing error handling in the exception path

**Rule Warnings:**
- Large PR: consider splitting into smaller changes.
- Changes in Program.cs detected. Ensure startup logic is correct.

---
_This review was generated by AiPrReviewer, a tool that uses AI to review pull requests._
```

## Possible Improvements

- [ ] Avoid duplicate comments on the same PR
- [ ] Inline comments per file / line
- [ ] Repository-level configuration (.ai-pr-reviewer.yml)
- [ ] Azure OpenAI support
- [ ] Azure DevOps Pipelines integration
- [ ] Metrics dashboard (number of PRs reviewed, issues detected)
- [ ] Webhook signature verification for security
- [ ] Retry logic for failed API calls
- [ ] Caching of installation tokens
- [ ] Support for different AI providers (configurable)

## Why This Project?

This project was built to demonstrate:

- Real-world GitHub App integrations
- Secure authentication flows (JWT + Installation Tokens)
- Practical AI usage in software engineering
- Clean, layered backend architecture
- Dependency Injection and SOLID principles
- Automation that solves a real developer pain point
- Production-ready patterns and practices

## Author

**Carlos Garita**  
Full Stack Developer  
Passionate about scalable systems, cloud architectures, and AI-assisted development.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
