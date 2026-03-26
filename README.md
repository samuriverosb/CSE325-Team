# CSE325-Team
Repo used for CSE325 Team Group Project

# Members
- Samuel Riveros
- Nuno Ferreira

# Project Artifacts
- [Project Proposal - Week 03](./PROJECT_PROPOSAL.md)
- [Project Checkpoint - Week 04](./W04_PROJECT_CHECKPOINT.md)

# Project Links
- GitHub: https://github.com/samuriverosb/CSE325-Team
- Trello: https://trello.com/b/s9q8d4DE/cse325

# Application Structure (Week 04 Checkpoint)
- Solution: `SelfRelianceFinanceTracker.slnx`
- Main project: `src/FinanceTracker.Web`
- Domain models: `Models/Category.cs`, `Models/Transaction.cs`, `Models/SavingsGoal.cs`
- Data layer: `Data/ApplicationDbContext.cs`, `Data/ApplicationUser.cs`
- Service layer: `Services/*`
- Feature pages:
- `Components/Pages/Dashboard.razor`
- `Components/Pages/Categories.razor`
- `Components/Pages/Transactions.razor`
- `Components/Pages/SavingsGoals.razor`

# Run Locally
1. Open terminal in the repository root.
2. Run: `dotnet restore src/FinanceTracker.Web/FinanceTracker.Web.csproj`
3. Run: `dotnet run --project src/FinanceTracker.Web/FinanceTracker.Web.csproj`
4. If your environment has `.NET CLI` permission issues, run: `.\run.ps1`

# Initialize Database
- One-time setup or reset:
- `.\init-db.ps1`

# Development Test User
- Email: `testuser@finance.local`
- Password: `Test1234!`
- This user is auto-created at startup in Development mode.
