# User Stories

This document captures the main user stories for the Self-Reliance Finance Tracker project.

## Core Stories

### US-01: Register and sign in
As a new user, I want to register and sign in securely so that I can access my personal finance data.

Acceptance criteria:
- A user can create an account with email and password.
- A user can sign in and sign out successfully.
- Unauthenticated users cannot access protected finance pages.

### US-02: Manage budget categories
As a user, I want to create, edit, and delete categories so that I can organize my spending and income.

Acceptance criteria:
- A user can add a category with a name and an optional monthly limit.
- A user can update a category name or monthly limit.
- A user can remove a category that is no longer needed.

### US-03: Record transactions
As a user, I want to record income and expense transactions so that I can keep an accurate history of my finances.

Acceptance criteria:
- A transaction must have an amount, category, type, and date.
- A user can create, edit, and delete transactions.
- The transaction list shows recent activity in reverse chronological order.

### US-04: Track savings goals
As a user, I want to create and manage savings goals so that I can measure progress toward future purchases or emergencies.

Acceptance criteria:
- A goal must include a name, target amount, current amount, and deadline.
- A user can create, edit, and delete savings goals.
- The system displays each goal's progress percentage and remaining amount.

### US-05: Apply income to a savings goal
As a user, I want to associate an income transaction with a savings goal so that my saved amount updates as I receive money.

Acceptance criteria:
- Income transactions can optionally be linked to a savings goal.
- Expense transactions cannot be applied to a savings goal.
- When linked income is added, the selected goal balance increases.

### US-06: Review dashboard summary
As a user, I want to see a dashboard summary so that I can quickly understand my current financial position.

Acceptance criteria:
- The dashboard shows current month income, expenses, and remaining budget.
- The dashboard shows active savings goals.
- The dashboard includes recent transactions and category budget usage.

### US-07: Monitor category budget usage
As a user, I want to compare category spending against monthly limits so that I can identify overspending early.

Acceptance criteria:
- The dashboard shows spent amount versus monthly limit for each category.
- Progress indicators show visual budget usage.
- Categories with high or exceeded usage are visually distinguishable.

### US-08: Receive clear validation feedback
As a user, I want form validation messages to be clear and consistent so that I can correct errors quickly.

Acceptance criteria:
- Required fields display validation feedback in English.
- Numeric fields reject invalid ranges.
- Savings goal validation prevents past deadlines and current amounts greater than target amounts.
