```mermaid
graph TD;
    subgraph "Frontend"
        style User fill:#f9f,stroke:#333,stroke-width:2px;
        User --> HAProxy
    end

    subgraph "Backend"
        style CashFlowControl fill:#b8f,stroke:#333,stroke-width:2px;
        style CashFlowReport fill:#b8f,stroke:#333,stroke-width:2px;
        HAProxy -->|Balanceamento| CashFlowControl
        HAProxy -->|Balanceamento| CashFlowReport
        CashFlowControl --> PostgreSQL
        CashFlowControl --> Redis
        CashFlowReport --> PostgreSQL
        CashFlowReport --> Redis
    end

    subgraph "Database"
        style PostgreSQL fill:#6f9,stroke:#333,stroke-width:2px;
        style Redis fill:#6f9,stroke:#333,stroke-width:2px;
        PostgreSQL --> Patroni
        Patroni --> FailoverSystem
    end