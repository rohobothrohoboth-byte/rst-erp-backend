# Phase 1 Journal Line Persistence Rules

Journal lines use `Direction` + `Amount` as the accounting input and persist matching debit/credit columns:

- `Debit` direction => `Debit = Amount`, `Credit = 0`.
- `Credit` direction => `Debit = 0`, `Credit = Amount`.
- Negative journal-line amounts are rejected.
- The journal header must remain balanced before creation.
- Financial reports remain backend-owned and must not depend on frontend calculations.

Existing historical rows are not rewritten by this change. New and edited journal lines are persisted consistently from the domain model.
