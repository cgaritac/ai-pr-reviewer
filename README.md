# 🤖 AI-Powered Pull Request Reviewer

An automated AI-assisted Pull Request reviewer for GitHub that helps teams detect code quality issues, potential bugs, and architectural risks directly from PR diffs — combining deterministic static rules with thoughtful LLM reasoning.

⚠️ **Important**: This tool is **not** meant to replace human reviewers.  
✅ It is designed to **augment** code reviews, reduce noise, and surface risks early.

## 🚀 Why This Project Exists

Code reviewers are essential, but:

- Large PRs are hard to review thoroughly
- Reviewers miss subtle risks under time pressure
- Static linters lack context and real reasoning

This project addresses those pain points by:

- Analyzing **only changed code** (diff-based analysis)  
- Applying deterministic rules first  
- Using AI **only** where human-like reasoning adds real value

## ✨ Key Features

- 🔍 Diff-based analysis (no full repo scanning)  
- 📏 Pre-AI static rules  
  - PR size warnings  
  - Too many files changed  
  - Sensitive folders detection  
- 🧠 AI-powered reasoning  
  - Potential bugs  
  - Risky patterns  
  - Code consistency issues  
- 💬 Automated PR comments  
- 🔐 Secure GitHub App integration  
- 🧩 Clean, extensible architecture

## 🧠 How It Works
GitHub Pull Request Event
↓
GitHub Webhook
↓
PR Diff Fetcher
↓
Static Rule Engine
↓
AI Reviewer (LLM)
↓
PR Comment Publisher

## 🏗️ Architecture Overview

The project follows a clean, modular architecture:
src/
├─ Api/                # Webhooks & HTTP endpoints
├─ Core/               # Domain models & interfaces
├─ Application/        # Business logic & review pipeline
└─ Infrastructure/     # GitHub & AI provider integrations

This design enables:

- Future extension to Azure DevOps  
- Easy swapping of AI providers (OpenAI → Azure OpenAI → Local LLMs)  
- Clear separation of concerns  

## 🧪 What the AI Reviews (and What It Doesn't)

**The AI DOES:**

- Analyze only modified code  
- Identify potential bugs and risks  
- Detect inconsistencies and bad practices  
- Flag unclear or confusing logic  

**The AI DOES NOT:**

- Rewrite or refactor large portions of code  
- Replace human approval  
- Execute builds or tests  
- Analyze dependencies or external libraries  

## 🤖 AI Strategy (Important Design Choice)

This project **does not** blindly send code to an LLM. Instead it:

1. Uses static rules to filter and reduce noise first  
2. Keeps context tightly controlled and minimal  
3. Forces structured, constrained LLM output (JSON schema)  

This approach delivers:

- Much lower token usage  
- More consistent feedback  
- Significantly better signal-to-noise ratio  

## 🛠️ Tech Stack

**Backend**  
- .NET 8  
- ASP.NET Core (Minimal APIs)

**AI**  
- OpenAI API  
- Designed for easy migration to Azure OpenAI  
- Structured responses using JSON schema

**GitHub Integration**  
- GitHub App (secure — no personal tokens)  
- Webhooks  
- Pull Request Comments API

**Tooling**  
- ngrok / smee.io (for local webhook testing)  
- Docker (optional)

## 🔐 Security Considerations

- GitHub webhook signature verification  
- Least-privilege GitHub App permissions  
- Secrets managed via environment variables  
- No permanent storage of source code  

## 🧑‍💻 Example PR Comment
🧠 Automated Review Summary
⚠️ Potential Issues Detected:
• Logic in UserService.cs may cause NullReferenceException when user is not found
• Authentication-related files modified without corresponding test updates
📏 PR Size:
• 12 files changed, ~480 lines — consider splitting for easier review
✅ Recommendation:
• Add null checks and unit tests for the updated authentication logic
text## 🗺️ Roadmap

**MVP**  
- GitHub webhook integration  
- PR comment publishing  
- Diff parser  
- Static rule engine  
- AI reviewer integration  

**Future / Next steps**  
- File-level comments  
- Azure DevOps support  
- Language-specific rules  
- Configurable review policies  
- Local LLM support (Ollama, etc.)  

## 🚧 Current Status

🟡 **Actively in development**  
Currently focused on solid GitHub integration and core review logic.

## 📄 License

MIT License

## 📬 Feedback & Contributions

Ideas, feedback, bug reports, and pull requests are very welcome!  

Thanks for checking it out! 🚀
