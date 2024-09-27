```mermaid
graph TD;
    subgraph "Backend"
        style CashFlowControl fill:#b8f,stroke:#333,stroke-width:2px;
        style CashFlowReport fill:#b8f,stroke:#333,stroke-width:2px;
        CashFlowControl --> RabbitMQ
        RabbitMQ --> CashFlowReport
        RabbitMQ --> CashFlowAuth
        RabbitMQ --> QueueManager
        QueueManager --> CashFlowReport
        QueueManager --> CashFlowControl
    end