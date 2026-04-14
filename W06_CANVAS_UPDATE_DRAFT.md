# Week 06 Canvas Update Draft

## Week 06 Checkpoint Summary

For Week 06, the main focus was on reporting, budget awareness, and validation review rather than on a new sprint-sized feature set.

Completed work:

- Added a dedicated Reports page with monthly summaries, budget breakdowns, savings goal progress, and recent activity.
- Added Spending Insights and Charts in English inside the Reports page.
- Added a Budget Alerts page for over-budget categories, goal deadlines, stale transaction activity, and monthly cash flow warnings.
- Introduced dedicated report and alert services to keep the page logic cleaner and easier to maintain.
- Updated navigation so the new checkpoint pages are reachable from the main menu.
- Reviewed and documented the existing validation rules across categories, transactions, and savings goals.

## Files Added or Updated

- `src/FinanceTracker.Web/Components/Pages/Reports.razor`
- `src/FinanceTracker.Web/Components/Pages/Alerts.razor`
- `src/FinanceTracker.Web/Components/Layout/NavMenu.razor`
- `src/FinanceTracker.Web/Components/Layout/NavMenu.razor.css`
- `src/FinanceTracker.Web/Services/IReportService.cs`
- `src/FinanceTracker.Web/Services/ReportService.cs`
- `src/FinanceTracker.Web/Services/IAlertService.cs`
- `src/FinanceTracker.Web/Services/AlertService.cs`
- `src/FinanceTracker.Web/Models/MonthlyReport.cs`
- `src/FinanceTracker.Web/Models/ReportCategoryBreakdown.cs`
- `src/FinanceTracker.Web/Models/GoalProgressReportItem.cs`
- `src/FinanceTracker.Web/Models/AlertNotification.cs`
- `src/FinanceTracker.Web/Models/AlertSeverity.cs`
- `src/FinanceTracker.Web/Models/SpendingInsight.cs`
- `src/FinanceTracker.Web/Models/ReportChartPoint.cs`
- `src/FinanceTracker.Web/wwwroot/app.css`
- `W06_VALIDATION_TEST_REPORT.md`

## Verification

- The Week 06 checkpoint changes were compiled successfully using `dotnet msbuild` with the `CoreCompile` target.
- Reporting and alert logic were organized into dedicated services.
- User-facing text for the new pages and documentation is in English.

## Next Recommended Follow-up

- Sync deployment changes from `upstream/main`.
- Continue the backlog item for deeper spending insights or richer charts only if the course scope requires it beyond the checkpoint.
- Coordinate the home screen improvements with the remaining branch work.
