namespace ExchangeExecutionPlanner.Api.Utils;

public static class OrderTypeValidator
{
    public static bool IsAllowedOrderType(string value)
    {
        return value.Equals("Buy", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("Sell", StringComparison.OrdinalIgnoreCase);
    }
}