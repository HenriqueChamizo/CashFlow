using CashFlowInfra.Data; 
using System.Text;
using CashFlowInfra.DTOs;
using CashFlowInfra.Interfaces;
using CashFlowInfra.Models;

namespace CashFlowInfra.Services
{
    public class ReportService : IReportService
    {
        private readonly ITransactionService _transactionService;

        public ReportService(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        
        public async Task<IList<Transaction>> GenerateTransactionReport(DateTime startDate, DateTime endDate)
        {
            return await _transactionService.GetReportTransactions(startDate, endDate);
        }
    
        public async Task<List<DailyBalanceResult>> GenerateDailyBalanceReport(DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionService.GetReportTransactions(startDate, endDate);

            var dailyBalances = transactions
                .GroupBy(t => t.Date.Date)
                .Select(g => new DailyBalanceResult
                {
                    Date = g.Key,
                    TotalCredits = g.Where(t => t.Type == "credit").Sum(t => t.Amount),
                    TotalDebits = g.Where(t => t.Type == "debit").Sum(t => t.Amount),
                    DailyBalance = g.Where(t => t.Type == "credit").Sum(t => t.Amount)
                                - g.Where(t => t.Type == "debit").Sum(t => t.Amount)
                })
                .ToList();

            return dailyBalances;
        }
    }
}