using CashFlowInfra.DTOs;
using CashFlowInfra.Models;

namespace CashFlowInfra.Interfaces;

public interface IReportService
{
    Task<IList<Transaction>> GenerateTransactionReport(DateTime startDate, DateTime endDate);
    Task<List<DailyBalanceResult>> GenerateDailyBalanceReport(DateTime startDate, DateTime endDate);
}