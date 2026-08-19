# Finance Phase 1 Baseline

## Accounting reporting authority

Phase 1 establishes the backend as the authoritative source for General Ledger and Trial Balance calculations. The frontend displays backend report results and must not independently recreate accounting balances.

## Period authority

Reports accept an explicit financial period or an explicit date range. When a period is supplied, its StartDate and EndDate are authoritative. Reports return the resolved period context in their DTOs.

## Journal authority

Only non-deleted, posted journal entries and non-deleted journal lines participate in ledger reporting. Unposted, deleted, or reversed history must not silently become accounting activity.

## Balance authority

Account `NormalBalance` determines signed movement and debit/credit presentation. Account opening balances are applied according to `OpeningBalanceDate`; activity before the selected period contributes to opening balance, and activity inside the period contributes to period movement and closing balance.

## Scope

Branch filtering is applied to journal entries. Historical accounts remain reportable when inactive; `IsDeleted` is the accounting-history boundary.
