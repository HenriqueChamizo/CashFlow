```mermaid
graph TD;
    subgraph "Data"
        style PostgreSQL fill:#6f9,stroke:#333,stroke-width:2px;
        style Redis fill:#6f9,stroke:#333,stroke-width:2px;
        PostgreSQL
        Redis
    end

    subgraph "Backend"
        style CashFlowAuth fill:#b8f,stroke:#333,stroke-width:2px;
        style CashFlowControl fill:#b8f,stroke:#333,stroke-width:2px;
        style CashFlowReport fill:#b8f,stroke:#333,stroke-width:2px;
        CashFlowAuth -->|JWT| CashFlowControl
        CashFlowControl --> PostgreSQL
        CashFlowControl --> CashFlowReport
        CashFlowReport --> Redis
    end
    
    subgraph "Frontend"
        style FrontEnd fill:#f9f,stroke:#333,stroke-width:2px;
        FrontEnd --> CashFlowAuth
    end

    subgraph "Monitoring"
        style Prometheus fill:#f96,stroke:#333,stroke-width:2px;
        style Grafana fill:#f96,stroke:#333,stroke-width:2px;
        style Loki fill:#f96,stroke:#333,stroke-width:2px;
        Prometheus --> Grafana
        Prometheus --> CashFlowControl
        Prometheus --> CashFlowReport
        Grafana -->|Métricas| Prometheus
        Loki --> Grafana
    end