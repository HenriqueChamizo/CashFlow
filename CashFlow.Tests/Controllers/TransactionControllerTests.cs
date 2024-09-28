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

        [Fact]
        public async Task Get_ShouldReturnAllTransactions()
        {
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

            var result = await _controller.Get(new TransactionQueryParams());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PagedResult<Transaction>>(okResult.Value);
            Assert.Equal(2, returnValue.TotalItems);
        }

        [Fact]
        public async Task GetById_ShouldReturnTransaction_WhenTransactionExists()
        {
            var transaction = new Transaction { Id = 1, Amount = 100, Type = "credit" };

            _transactionServiceMock.Setup(s => s.GetTransactionById(1)).ReturnsAsync(transaction);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<ActionResult<Transaction>>(result);
            var returnValue = Assert.IsType<Transaction>(result.Value);
            Assert.Equal(100, returnValue.Amount);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTransactionDoesNotExist()
        {
            _transactionServiceMock.Setup(s => s.GetTransactionById(1)).ReturnsAsync((Transaction)null);

            var result = await _controller.Get(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedTransaction_WhenModelIsValid()
        {
            var newTransaction = new Transaction { Amount = 200, Type = "credit", Description = "New Transaction" };

            _transactionServiceMock.Setup(s => s.CreateTransaction(newTransaction))
                .ReturnsAsync(newTransaction);

            var result = await _controller.Create(newTransaction);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Transaction>(createdResult.Value);
            Assert.Equal(200, returnValue.Amount);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            _controller.ModelState.AddModelError("Amount", "Required");

            var result = await _controller.Create(new Transaction());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenTransactionExists()
        {
            var transaction = new Transaction { Id = 1, Amount = 100, Type = "credit" };
            _transactionServiceMock.Setup(s => s.GetTransactionById(1)).ReturnsAsync(transaction);

            var result = await _controller.Delete(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenTransactionDoesNotExist()
        {
            _transactionServiceMock.Setup(s => s.GetTransactionById(1)).ReturnsAsync((Transaction)null);

            var result = await _controller.Delete(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
