namespace ExchangeExecutionPlanner.Models;

public class Order
{
    public string Id { get; set; }
    public DateTime Time { get; set; }
    public string Type { get; set; } // "Buy" or "Sell"
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
}