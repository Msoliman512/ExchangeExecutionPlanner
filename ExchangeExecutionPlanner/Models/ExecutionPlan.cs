namespace ExchangeExecutionPlanner.Models;

public class ExecutionPlan
{
    public string ExchangeId { get; set; }
    public string OrderType { get; set; } // "Buy" or "Sell"
    public decimal RequestedAmount { get; set; }
    public decimal FilledAmount { get; set; }
    public List<OrderMatch> Matches { get; set; } = new();
    public decimal TotalCostOrProceeds { get; set; } // EUR spent or received
    public decimal AveragePrice { get; set; }
    public bool IsFullyFilled { get; set; }
}

public class OrderMatch
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
}