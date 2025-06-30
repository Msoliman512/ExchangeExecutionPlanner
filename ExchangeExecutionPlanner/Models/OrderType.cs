namespace ExchangeExecutionPlanner.Models;

public enum OrderType
{
    Buy,
    Sell
}

public static class OrderTypeHelper
{
    public static OrderType Parse(string type)
    {
        return type.Equals("Buy", StringComparison.OrdinalIgnoreCase)
            ? OrderType.Buy
            : OrderType.Sell;
    }

    public static string ToString(OrderType type)
    {
        return type == OrderType.Buy ? "Buy" : "Sell";
    }
}
