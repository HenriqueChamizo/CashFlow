
    namespace CashFlowInfra.DTOs;

    public class DailyBalanceResult
    {
        public DateTime Date { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal DailyBalance { get; set; }
    }