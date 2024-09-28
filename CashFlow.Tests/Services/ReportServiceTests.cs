using Xunit;
using Moq;
using CashFlowInfra.Interfaces;
using CashFlowInfra.Models;
using CashFlowInfra.DTOs;
using CashFlowInfra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashFlowReport.Tests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _reportService = new ReportService(_mockTransactionService.Object);
        }

        [Fact]
        public async Task GenerateTransactionReport_ShouldReturnTransactions()
        {
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);
            var mockTransactions = new List<Transaction>
            {
                new Transaction { Id = 1, Amount = 100, Type = "credit", Date = new DateTime(2024, 1, 10), Description = "Test Credit" },
                new Transaction { Id = 2, Amount = 50, Type = "debit", Date = new DateTime(2024, 1, 15), Description = "Test Debit" }
            };

            _mockTransactionService.Setup(s => s.GetReportTransactions(startDate, endDate))
                                   .ReturnsAsync(mockTransactions);

            var result = await _reportService.GenerateTransactionReport(startDate, endDate);

            Assert.Equal(2, result.Count);
            Assert.Equal(mockTransactions, result);
        }

        [Fact]
        public async Task GenerateDailyBalanceReport_ShouldReturnCorrectBalances()
        {
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);
            var mockTransactions = new List<Transaction>
            {
                new Transaction { Id = 1, Amount = 100, Type = "credit", Date = new DateTime(2024, 1, 10), Description = "Test Credit" },
                new Transaction { Id = 2, Amount = 50, Type = "debit", Date = new DateTime(2024, 1, 10), Description = "Test Debit" },
                new Transaction { Id = 3, Amount = 200, Type = "credit", Date = new DateTime(2024, 1, 11), Description = "Test Credit" },
                new Transaction { Id = 4, Amount = 100, Type = "debit", Date = new DateTime(2024, 1, 11), Description = "Test Debit" }
            };

            _mockTransactionService.Setup(s => s.GetReportTransactions(startDate, endDate))
                                   .ReturnsAsync(mockTransactions);

            var result = await _reportService.GenerateDailyBalanceReport(startDate, endDate);

            Assert.Equal(2, result.Count);
            Assert.Equal(new DateTime(2024, 1, 10), result[0].Date);
            Assert.Equal(100, result[0].TotalCredits);
            Assert.Equal(50, result[0].TotalDebits);
            Assert.Equal(50, result[0].DailyBalance);

            Assert.Equal(new DateTime(2024, 1, 11), result[1].Date);
            Assert.Equal(200, result[1].TotalCredits);
            Assert.Equal(100, result[1].TotalDebits);
            Assert.Equal(100, result[1].DailyBalance);
        }

        [Fact]
        public async Task GenerateTransactionReport_ShouldReturnEmpty_WhenNoTransactionsFound()
        {
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);
            _mockTransactionService.Setup(s => s.GetReportTransactions(startDate, endDate))
                                   .ReturnsAsync(new List<Transaction>());

            var result = await _reportService.GenerateTransactionReport(startDate, endDate);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GenerateDailyBalanceReport_ShouldReturnEmpty_WhenNoTransactionsFound()
        {
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);
            _mockTransactionService.Setup(s => s.GetReportTransactions(startDate, endDate))
                                   .ReturnsAsync(new List<Transaction>());

            var result = await _reportService.GenerateDailyBalanceReport(startDate, endDate);

            Assert.Empty(result);
        }
    }
}