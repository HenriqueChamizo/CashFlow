```mermaid
sequenceDiagram
    participant User as Frontend
    participant CashFlowAuth as Auth Service
    participant CashFlowControl as Transaction Service
    participant CashFlowReport as Report Service

    User->>CashFlowAuth: Login Request
    CashFlowAuth->>User: JWT Token
    User->>CashFlowControl: Request with JWT
    CashFlowControl->>CashFlowAuth: Validate JWT
    CashFlowAuth-->>CashFlowControl: JWT Valid
    CashFlowControl->>User: Response with Data

    User->>CashFlowReport: Request with JWT
    CashFlowReport->>CashFlowAuth: Validate JWT
    CashFlowAuth-->>CashFlowReport: JWT Valid
    CashFlowReport->>User: Response with Report