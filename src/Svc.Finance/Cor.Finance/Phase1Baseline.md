# Finance Phase 1

## Step 4 — Accounting/reporting foundation

This checkpoint hardens the existing General Ledger and Trial Balance reporting paths and establishes the backend as the authoritative calculation layer.

### Rules

- Posted journal entries are the only journal entries included in accounting reports.
- Reports are period-driven. A `periodId` resolves its authoritative start/end dates from `FinancialPeriods`; explicit dates are accepted for controlled/custom reporting.
- Opening balances are calculated from the account opening balance plus posted movements before the selected period.
- Period debit/credit activity is calculated from posted journal lines inside the selected period.
- Closing balances are calculated by the backend from the opening balance and period movement.
- Account `NormalBalance` is authoritative; report code must not infer normal balance solely from `AccountType`.
- Branch filtering is applied to posted journal entries when supplied.
- Trial Balance returns opening, activity, closing, difference, balanced status, and period metadata.
- General Ledger returns period metadata, opening/closing balances, transactions, and running balances.
- The frontend must display backend report results and must not recreate accounting calculations.
- Database data is not modified by this phase.

### Validation target

For a selected financial period, Trial Balance and General Ledger must agree on posted journal activity and use the same period boundaries, normal-balance rules, and branch filter.
