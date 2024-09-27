using Xunit;
using Moq;
using CashFlowControl.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CashFlowInfra.DTOs;
using CashFlowInfra.Interfaces;
using CashFlowInfra.Models;

namespace CashFlowControl.Tests.Controllers
{
    public class TransactionControllerTests
    {
        private readonly TransactionController _controller;
        private readonly Mock<ITransactionService> _transactionServiceMock;

        public TransactionControllerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _controller = new TransactionController(_transactionServiceMock.Object);
        }

        // Test for Get with pagination and filters
        [Fact]
        public async Task Get_ShouldReturnAllTransactions()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, Amount = 100, Type = "credit", Description = "Test 1" },
                new Transaction { Id = 2, Amount = 50, Type = "debit", Description = "Test 2" }
            };
            
            var pagedResult = new PagedResult<Transaction>() 
            {
                Items = transactions,
                PageNumber = 1,
                PageSize = transactions.Count,
                TotalItems = transactions.Count
            };
            _transactionServiceMock.Setup(s => s.GetTransactions(It.IsAny<TransactionQueryParams>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.Get(new TransactionQueryParams());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PagedResult<Transaction>>(okResult.Value);
            Assert.Equal(2, returnValue.TotalItems);
        }

        // Test for Get by id when transaction exists
        [Fact]
        public async Task GetById_ShouldReturnTransaction_WhenTransactionExists()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, Amount = 100, Type = "credit" };

            _transactionServiceMock.Setup(s => s.GetTransactionById(1)).ReturnsAsync(transaction);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<ActionResult<Transaction>>(result);
            var returnValue = Assert.IsType<Transaction>(result.Value);
            Assert.Equal(100, returnValue.Amount);
        }

        // Test for Get by id when transaction does not exist
        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTransactionDoesNotExist()
        {
            // Arrange
            _transactionServiceMock.Setup(s => s.GetTransactionById(1)).ReturnsAsync((Transaction)null);

            // Act
            var result = await _controller.Get(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Test for Create transaction when model is valid
        [Fact]
        public async Task Create_ShouldReturnCreatedTransaction_WhenModelIsValid()
        {
            // Arrange
            var newTransaction = new Transaction { Amount = 200, Type = "credit", Description = "New Transaction" };

            _transactionServiceMock.Setup(s => s.CreateTransaction(newTransaction))
                .ReturnsAsync(newTransaction);

            // Act
            var result = await _controller.Create(newTransaction);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Transaction>(createdResult.Value);
            Assert.Equal(200, returnValue.Amount);
        }

        // Test for Create transaction when model is invalid
        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Amount", "Required");

            // Act
            var result = await _controller.Create(new Transaction());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // Test for Delete when transaction exists
        [Fact]
        public async Task Delete_ShouldReturnOk_WhenTransactionExists()
        {
            // Arrange
            var transaction = new Transaction { Id = 1, Amount = 100, Type = "credit" };
            _transactionServiceMock.Setup(s => s.GetTransactionById(1)).ReturnsAsync(transaction);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        // Test for Delete when transaction does not exist
        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenTransactionDoesNotExist()
        {
            // Arrange
            _transactionServiceMock.Setup(s => s.GetTransactionById(1)).ReturnsAsync((Transaction)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
