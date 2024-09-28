using CashFlowInfra.Interfaces;
using CashFlowInfra.Models;
using CashFlowInfra.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CashFlowReport.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CashFlowReport.Tests.Controllers
{
    public class ReportControllerTests
    {
        private readonly Mock<IReportService> _mockReportService;
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            _mockReportService = new Mock<IReportService>();
            _controller = new ReportController(_mockReportService.Object);
        }

        [Fact]
        public async Task GetTransactionReport_ShouldReturnOk_WhenDatesAreProvided()
        {
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);
            var reportData = new List<Transaction>
            {
                new Transaction { Id = 1, Amount = 100, Type = "credit", Date = startDate, Description = "Test 1" }
            };

            _mockReportService.Setup(s => s.GenerateTransactionReport(startDate, endDate))
                .ReturnsAsync(reportData);

            var result = await _controller.GetTransactionReport(startDate, endDate) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(reportData, result.Value);
        }

        [Fact]
        public async Task GetTransactionReport_ShouldReturnBadRequest_WhenStartDateOrEndDateIsMissing()
        {
            var result = await _controller.GetTransactionReport(null, new DateTime(2024, 1, 31)) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Start date and end date are required.", result.Value);
        }

        [Fact]
        public async Task GetDailyBalanceReport_ShouldReturnOk_WhenDatesAreProvided()
        {
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);
            var reportData = new List<DailyBalanceResult>
            {
                new DailyBalanceResult
                {
                    Date = startDate,
                    TotalCredits = 500,
                    TotalDebits = 100,
                    DailyBalance = 400
                }
            };

            _mockReportService.Setup(s => s.GenerateDailyBalanceReport(startDate, endDate))
                .ReturnsAsync(reportData);

            var result = await _controller.GetDailyBalanceReport(startDate, endDate) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(reportData, result.Value);
        }

        [Fact]
        public async Task GetDailyBalanceReport_ShouldReturnBadRequest_WhenStartDateOrEndDateIsMissing()
        {
            var result = await _controller.GetDailyBalanceReport(null, new DateTime(2024, 1, 31)) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Start date and end date are required.", result.Value);
        }
    }
}