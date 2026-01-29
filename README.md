## Database Schema (ERD)

```mermaid
erDiagram
    CUSTOMER ||--|| SUBSCRIPTION : "has"
    SUBSCRIPTION ||--o| TIER : "belongs to"
    CUSTOMER ||--o{ USAGE_LOG : "generates"
    CUSTOMER ||--o{ MONTHLY_SUMMARY : "is billed via"
    API_ENDPOINT ||--o{ USAGE_LOG : "is targeted by"

    CUSTOMER {
        int CustomerId PK
        string Name
        string ApiKey
    }

    TIER {
        int TierId PK
        string Name
        int MonthlyQuota
        int RateLimit
        decimal Price
    }

    USAGE_LOG {
        int LogId PK
        int CustomerId FK
        int ApiId FK
        datetime Timestamp
        int Year
        int Month
    }

    MONTHLY_SUMMARY {
        int SummaryId PK
        int CustomerId FK
        string Month
        int TotalRequests
        decimal BillAmount
    }
