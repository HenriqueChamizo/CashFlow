```mermaid
graph TD;
    subgraph "Backend"
        style CashFlowControl fill:#b8f,stroke:#333,stroke-width:2px;
        style CashFlowReport fill:#b8f,stroke:#333,stroke-width:2px;
        CashFlowControl -->|Métricas| Prometheus
        CashFlowReport -->|Métricas| Prometheus
    end

    subgraph "Monitoring"
        style Prometheus fill:#f96,stroke:#333,stroke-width:2px;
        style Grafana fill:#f96,stroke:#333,stroke-width:2px;
        style Loki fill:#f96,stroke:#333,stroke-width:2px;
        Prometheus --> Grafana
        Loki --> Grafana
        Grafana -->|Dashboard| User
        ApplicationInsights --> Grafana
        ApplicationInsights --> Logs
    end