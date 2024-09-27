using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CashFlowInfra.Validates;

public class TransactionTypeAttribute : ValidationAttribute
{
    [ExcludeFromCodeCoverage]
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var transactionType = value as string;
        if (transactionType == "credit" || transactionType == "debit")
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("The transaction type must be either 'credit' or 'debit'.");
    }
}
