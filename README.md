# 🤖 AI PR Reviewer

Automated Pull Request Code Reviews powered by AI and GitHub Apps

AI PR Reviewer is a GitHub App that automatically analyzes Pull Requests using AI and posts concise, actionable code review feedback directly on GitHub.

Designed to demonstrate real-world integrations, secure GitHub App authentication, and practical AI usage in a production-like backend architecture.

## 🚀 Key Features

- 🔐 GitHub App authentication (JWT + Installation Tokens)
- 🪝 GitHub Webhooks for Pull Request events
- 📂 Automatic PR file & diff analysis
- 🧠 AI-powered code reviews using OpenAI
- 💬 Automated PR comments with actionable feedback
- ⚙️ Minimal API with clean, modular architecture

## 🧩 How It Works

1. A Pull Request is opened in a repository
2. GitHub sends a webhook event to the API
3. The app authenticates as a GitHub App
4. PR files and diffs are fetched from GitHub
5. Relevant changes are converted into an AI-friendly prompt
6. OpenAI analyzes the code changes
7. A formatted review comment is posted back to the Pull Request

## 🏗️ Architecture Overview

```
GitHub PR Opened
      ↓
GitHub Webhook
      ↓
.NET Minimal API
      ↓
GitHub App Authentication (JWT)
      ↓
PR Files & Diffs
      ↓
Prompt Builder
      ↓
OpenAI Review
      ↓
GitHub PR Comment
```

## 🛠️ Tech Stack

### Backend
- .NET 8
- Minimal APIs
- System.IdentityModel.Tokens.Jwt

### GitHub Integration
- GitHub Apps
- Webhooks
- REST API (Installation Tokens)

### AI
- OpenAI API
- GPT-4o-mini / GPT-4.1-mini
- Prompt engineering for code reviews

### Dev & Tooling
- ngrok (local webhook exposure)
- Environment-based configuration
- Dependency Injection

## 📁 Project Structure

```
AiPrReviewer
│
├── Models
│   └── GitHub
│       ├── PRWebhookPayload.cs
│       ├── PRFile.cs
│       ├── PR.cs
│       ├── Installation.cs
│       ├── Repository.cs
│       └── PRPayload.cs
│
├── Services
│   ├── Github
│   │   ├── JwtService.cs
│   │   ├── InstallationService.cs
│   │   ├── PRService.cs
│   │   └── CommentService.cs
│   │
│   └── AI
│       ├── AiPromptBuilder.cs
│       ├── OpenAiReviewService.cs
│       └── AiCommentFormatter.cs
│
└── Program.cs
```

## 🔐 Environment Variables

```bash
# GitHub App
APP_ID=123456
PRIVATE_KEY=-----BEGIN RSA PRIVATE KEY-----
...

# OpenAI
OPENAI_API_KEY=sk-xxxxxxxx
OPENAI_MODEL=gpt-4o-mini
```

⚠️ **Never commit secrets. Use environment variables or secret managers.**

## 🧪 Local Development

```bash
dotnet restore
dotnet run
```

Expose the API using ngrok:

```bash
ngrok http 5288
```

Configure the ngrok URL as the webhook endpoint in your GitHub App.

## 🧠 Design Decisions

- **Minimal API** for simplicity and clarity
- **GitHub App auth** instead of personal tokens (enterprise-ready)
- **Diff-based prompts** to reduce token usage and noise
- **Separated services** for GitHub, AI, and formatting
- **AI as an assistant**, not a replacement for developers

## 🧩 Example PR Comment

```
🤖 AI Code Review

- Consider validating null inputs in UserService.cs
- Potential performance issue inside the authentication loop
- Naming could be improved for better readability

---
_This review was automatically generated._
```

## 🚧 Possible Improvements

- Avoid duplicate comments on the same PR
- Inline comments per file / line
- Repository-level configuration (.ai-pr-reviewer.yml)
- Azure OpenAI support
- Azure DevOps Pipelines integration
- Metrics dashboard (number of PRs reviewed, issues detected)

## 📌 Why This Project?

This project was built to demonstrate:

- Real-world GitHub App integrations
- Secure authentication flows
- Practical AI usage in software engineering
- Clean backend architecture
- Automation that solves a real developer pain point

## 👤 Author

**Carlos Garita**  
Full Stack Developer  
Passionate about scalable systems, cloud architectures, and AI-assisted development.

---

> ⚠️ **Note**: I'm currently working on improving the system architecture. The project is under active development and may experience significant changes.
