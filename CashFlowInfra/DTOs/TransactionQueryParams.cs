namespace CashFlowInfra.DTOs;

public class TransactionQueryParams : PaginationParams
{
    public string? Type { get; set; } 
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
