using Xunit;
using Moq;
using CashFlowInfra.Services;
using CashFlowInfra.Data;
using CashFlowInfra.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CashFlowInfra.DTOs;

namespace CashFlowControl.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly TransactionService _transactionService;
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<TransactionService>> _loggerMock;

        private readonly DateTime startDate = DateTime.Now.AddDays(-1);
        private readonly DateTime endDate = DateTime.Now;

        public TransactionServiceTests()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CashFlowTestDb")
                .Options;

            _context = new AppDbContext(options);

            _loggerMock = new Mock<ILogger<TransactionService>>();

            // Preenche o banco de dados em memória com dados fictícios
            _context.Transactions.AddRange(new List<Transaction>
            {
                new Transaction { Amount = 100, Type = "credit", Description = "Test Credit", Date = startDate },
                new Transaction { Amount = 50, Type = "debit", Description = "Test Debit", Date = endDate }
            });
            _context.SaveChanges();

            _transactionService = new TransactionService(_context, _loggerMock.Object);
        }

        // Cenário de paginação com múltiplas páginas
        [Fact]
        public async Task GetTransactions_ShouldReturnPagedResult_WithMultiplePages()
        {
            var transactions = await _transactionService.GetReportTransactions(null, null);

            // Arrange
            var queryParams = new TransactionQueryParams() { PageNumber = 2, PageSize = 1 };

            // Act
            var result = await _transactionService.GetTransactions(queryParams);

            var totalPages = (int)Math.Ceiling((double)result.TotalItems / result.PageSize);
            Assert.NotNull(result);
            Assert.Equal(transactions.Count, result.TotalItems); // Total de 2 transações
            Assert.Equal(totalPages, result.TotalPages);
            Assert.Equal(1, result.Items.Count); // Apenas 1 por página
            Assert.Equal(2, result.PageNumber);
            Assert.Equal(1, result.PageSize);
        }

        // Cenário com lista de transações vazia
        [Fact]
        public async Task GetTransactions_ShouldReturnEmptyResult_WhenNoTransactions()
        {
            // Arrange
            _context.Transactions.RemoveRange(_context.Transactions);
            await _context.SaveChangesAsync();

            var queryParams = new TransactionQueryParams() { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _transactionService.GetTransactions(queryParams);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items); // Nenhuma transação
        }

        // Teste de filtro por descrição
        [Fact]
        public async Task GetTransactions_ShouldFilterByDescription()
        {
            // Arrange
            var queryParams = new TransactionQueryParams { Description = "Test Credit" };

            // Act
            var result = await _transactionService.GetTransactions(queryParams);

            // Assert
            Assert.All(result.Items, t => Assert.Equal("Test Credit", t.Description));
        }

        [Fact]
        public async Task GetTransactions_ShouldFilterByType()
        {
            // Arrange
            var queryParams = new TransactionQueryParams { Type = "credit" };

            // Act
            var result = await _transactionService.GetTransactions(queryParams);

            // Assert
            Assert.All(result.Items, t => Assert.Equal("credit", t.Type));
        }


        [Fact]
        public async Task GetTransactions_ShouldFilterByAmount()
        {
            // Arrange
            var queryParams = new TransactionQueryParams { MinAmount = 40, MaxAmount = 60 };

            // Act
            var result = await _transactionService.GetTransactions(queryParams);

            // Assert
            Assert.All(result.Items, t => Assert.InRange(t.Amount, 40, 60));
        }

        [Fact]
        public async Task GetTransactions_ShouldFilterByDate()
        {
            var queryParams = new TransactionQueryParams { StartDate = startDate, EndDate = endDate.AddDays(1) };

            // Act
            var result = await _transactionService.GetTransactions(queryParams);

            // Assert
            Assert.All(result.Items, t => Assert.InRange(t.Date, startDate, endDate.AddDays(1)));
        }

        [Fact]
        public async Task GetReportTransactions_ShouldFilterByDate()
        {
            var result = await _transactionService.GetReportTransactions(startDate, endDate.AddDays(1));

            // Assert
            Assert.All(result, t => Assert.InRange(t.Date, startDate, endDate.AddDays(1)));
        }

        // Teste para verificar se o GetTransactionById retorna null quando não encontrado
        [Fact]
        public async Task GetTransactionById_ShouldReturnNull_WhenNotFound()
        {
            // Act
            var transaction = await _transactionService.GetTransactionById(999);

            // Assert
            Assert.Null(transaction);
        }

        // Teste para garantir que o log foi chamado ao criar uma transação
        [Fact]
        public async Task CreateTransaction_ShouldLogInformation()
        {
            // Arrange
            var newTransaction = new Transaction
            {
                Amount = 300,
                Type = "debit",
                Description = "Logged Transaction",
                Date = DateTime.Now
            };

            // Act
            await _transactionService.CreateTransaction(newTransaction);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Creating")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        // Teste para verificar se ocorre exceção ao tentar criar uma transação inválida
        [Fact]
        public async Task CreateTransaction_ShouldThrowException_WhenTransactionIsInvalid()
        {
            // Arrange
            var invalidTransaction = new Transaction
            {
                Amount = -50, // Valor inválido
                Type = null, // Tipo nulo
                Description = "Invalid Transaction",
                Date = DateTime.Now
            };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _transactionService.CreateTransaction(invalidTransaction));
        }

        // Teste para verificar o Delete quando o ID não é encontrado
        [Fact]
        public async Task DeleteTransaction_ShouldLogWarning_WhenTransactionNotFound()
        {
            // Act
            await _transactionService.DeleteTransaction(9999);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Attempted to delete non-existent transaction")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteTransaction_ShouldSuccess()
        {
            // Arrange
            var newTransaction = new Transaction
            {
                Amount = 1,
                Type = "debit",
                Description = "Transaction to Deleted",
                Date = DateTime.Now
            };

            var transaction = await _transactionService.CreateTransaction(newTransaction);

            await _transactionService.DeleteTransaction(transaction.Id);

            var exists = await _transactionService.GetTransactionById(transaction.Id);

            Assert.Null(exists);
            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transaction with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}