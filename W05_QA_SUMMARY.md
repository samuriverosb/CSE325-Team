# Week 05 QA Summary

This summary documents the validation and accessibility review completed during Week 05 for the Self-Reliance Finance Tracker project.

## Scope Reviewed

- Categories page
- Transactions page
- Savings Goals page
- Dashboard page
- Domain validation rules in the finance models

## Validation Review

Completed checks:
- Category name is required and must stay within the allowed length.
- Monthly limit must be zero or greater.
- Transaction category is required.
- Transaction amount must be positive.
- Transaction description cannot exceed the allowed length.
- Savings goal target amount must be positive.
- Savings goal current amount can be zero, which supports new goals that have not started yet.
- Savings goal deadline cannot be in the past.
- Savings goal current amount cannot exceed the target amount.

Validation improvements added this week:
- Added explicit English validation messages to the finance models.
- Added field-level validation messages to the add forms.
- Corrected the savings goal validation rule so a new goal can start at `0.00`.

## Accessibility Review

Completed improvements:
- Added `role="alert"` to form validation summaries.
- Added accessible status messages for loading and empty states.
- Added visually hidden table captions for screen reader users.
- Added clearer helper text for goal-related transaction behavior.
- Added a default "Do not apply to a goal" option to reduce form ambiguity.
- Replaced empty transaction descriptions on the dashboard and transaction list with a readable fallback.

## Known Follow-up Items

- Resolve the Git merge conflict on `src/FinanceTracker.Web/Data/app.db`.
- Close any running instance of the app before doing a normal full build, or build with `UseAppHost=false` during validation.
- Finish deployment-related tasks after the merge conflict is resolved.
