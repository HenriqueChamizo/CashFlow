using CashFlowInfra.DTOs;
using CashFlowInfra.Interfaces;
using CashFlowInfra.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowControl.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Transaction>>> Get([FromQuery] TransactionQueryParams queryParams)
        {
            var transactions = await _transactionService.GetTransactions(queryParams);
            return Ok(transactions);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> Get(int id)
        {
            var transaction = await _transactionService.GetTransactionById(id);
            if (transaction == null)
                return NotFound();

            return transaction;
        }
        
        [HttpPost]
        public async Task<ActionResult<Transaction>> Create(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var createdTransaction = await _transactionService.CreateTransaction(transaction);
            return CreatedAtAction(nameof(Get), new { id = createdTransaction.Id }, createdTransaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _transactionService.GetTransactionById(id);
            if (transaction == null)
                return NotFound();
            await _transactionService.DeleteTransaction(id);
            return Ok();
        }
    }
}
