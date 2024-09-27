using CashFlowInfra.Models;
using CashFlowInfra.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashFlowInfra.Interfaces
{
    public interface ITransactionService
    {
        Task<PagedResult<Transaction>> GetTransactions(TransactionQueryParams queryParams);
        Task<IList<Transaction>> GetReportTransactions(DateTime? StartDate, DateTime? EndDate);
        Task<Transaction> GetTransactionById(int id);
        Task<Transaction> CreateTransaction(Transaction transaction);
        Task DeleteTransaction(int id);
    }
}
