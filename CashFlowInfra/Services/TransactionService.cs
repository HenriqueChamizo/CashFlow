using CashFlowInfra.Data;
using CashFlowInfra.Interfaces;
using CashFlowInfra.Models;
using CashFlowInfra.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CashFlowInfra.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(AppDbContext context, ILogger<TransactionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<Transaction>> GetTransactions(TransactionQueryParams queryParams)
        {
            var query = _context.Transactions.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Type))
            {
                query = query.Where(t => t.Type == queryParams.Type);
            }
            if (queryParams.MinAmount.HasValue)
            {
                query = query.Where(t => t.Amount >= queryParams.MinAmount);
            }
            if (queryParams.MaxAmount.HasValue)
            {
                query = query.Where(t => t.Amount <= queryParams.MaxAmount);
            }
            if (!string.IsNullOrEmpty(queryParams.Description))
            {
                query = query.Where(t => t.Description != null && t.Description.Contains(queryParams.Description));
            }
            if (queryParams.StartDate.HasValue)
            {
                query = query.Where(t => t.Date >= queryParams.StartDate.Value);
            }
            if (queryParams.EndDate.HasValue)
            {
                query = query.Where(t => t.Date <= queryParams.EndDate.Value);
            }
            var totalItems = await query.CountAsync();
            var transactions = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return new PagedResult<Transaction>
            {
                Items = transactions,
                TotalItems = totalItems,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }


        public async Task<IList<Transaction>> GetReportTransactions(DateTime? StartDate, DateTime? EndDate)
        {
            var query = _context.Transactions.AsQueryable();

            if (StartDate.HasValue)
                query = query.Where(t => t.Date.Date >= StartDate.Value.Date);
    
            if (EndDate.HasValue)
                query = query.Where(t => t.Date.Date <= EndDate.Value.Date);
    
            return await query.ToListAsync();
        }


        public async Task<Transaction> GetTransactionById(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            _logger.LogInformation("Creating: {Transaction}", transaction);
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Transaction created with ID: {TransactionId}", transaction.Id);
            return transaction;
        }

        public async Task DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) {
                _logger.LogWarning("Attempted to delete non-existent transaction with ID: {TransactionId}", id);
                return;
            }
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Transaction with ID {TransactionId} deleted", id);
        }
    }
}
