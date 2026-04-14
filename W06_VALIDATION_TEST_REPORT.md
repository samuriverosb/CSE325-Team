# Week 06 Validation Test Report

## Purpose

This report documents the Week 06 validation review completed for the Self-Reliance Finance Tracker checkpoint.

## Scope Covered

- Category validation rules
- Transaction validation rules
- Savings goal validation rules
- Form-level validation feedback on the main finance pages

## Validation Rules Reviewed

### Categories

- Category name is required.
- Category name must stay between 2 and 80 characters.
- Monthly limit must be between 0 and 1,000,000.

### Transactions

- Category selection is required.
- Amount must be between 0.01 and 1,000,000,000.
- Transaction type is required.
- Transaction date is required.
- Description cannot exceed 250 characters.

### Savings Goals

- Goal name is required.
- Goal name must stay between 2 and 120 characters.
- Target amount must be between 1 and 10,000,000.
- Current amount must be between 0 and 10,000,000.
- Deadline is required.
- Deadline cannot be in the past.
- Current amount cannot exceed the target amount.

## Test Approach

- Reviewed the model annotations in the finance domain models.
- Confirmed that validation messages are written in English.
- Checked that the Razor forms display validation feedback through summaries and field-level messages.
- Recompiled the application after the Week 06 reporting and alert changes to confirm no validation-related compile regressions were introduced.

## Results

- Category validation rules are present and mapped to the UI.
- Transaction validation rules are present and mapped to the UI.
- Savings goal validation rules are present, including the custom cross-field checks.
- Validation messaging remains consistent with the Week 05 QA improvements.
- No new compile issues were introduced by the Week 06 checkpoint work.

## Notes

- This checkpoint focused on validating the existing rules and their user-facing feedback, not on adding a separate automated test project.
- Further automated coverage can be added later if the course requires dedicated unit or integration tests.
