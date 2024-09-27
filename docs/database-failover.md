```mermaid
graph TD;
    subgraph "Backend"
        style CashFlowControl fill:#b8f,stroke:#333,stroke-width:2px;
        CashFlowControl -->|Escrita/Leitura| PostgreSQLPrimary
        CashFlowReport -->|Leitura| PostgreSQLReplica
    end

    subgraph "Database"
        style PostgreSQLPrimary fill:#6f9,stroke:#333,stroke-width:2px;
        style PostgreSQLReplica fill:#6f9,stroke:#333,stroke-width:2px;
        PostgreSQLPrimary --> Patroni
        PostgreSQLReplica --> Patroni
        Patroni --> FailoverSystem
    end