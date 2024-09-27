using System;
using System.ComponentModel.DataAnnotations;
using CashFlowInfra.Validates;

namespace CashFlowInfra.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "The transaction type is required.")]
        [TransactionType]
        public string? Type { get; set; }

        [Required(ErrorMessage = "The transaction date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "The description is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The description must be between 3 and 100 characters.")]
        public string? Description { get; set; }
    }

}
