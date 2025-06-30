using ExchangeExecutionPlanner.Data;
using ExchangeExecutionPlanner.Models;

namespace ExchangeExecutionPlanner.Services;

public class ExchangeExecutionService : IExchangeExecutionService
{
    private readonly IExchangeRepository _repo;

    public ExchangeExecutionService(IExchangeRepository repo)
    {
        _repo = repo;
    }

    public async Task<ExecutionPlan> FindBestExecutionAsync(OrderType orderType, decimal amount)
    {
        var exchanges = await _repo.LoadAllExchangesAsync();

        // 1. Build a flat list of eligible orders from all exchanges
        var flatOrders = new List<(Order order, string exchangeId)>();

        foreach (var ex in exchanges)
        {
            if (orderType == OrderType.Buy && ex.AvailableFunds.Euro > 0 && ex.OrderBook.Asks.Any())
            {
                // Only include asks where exchange has EUR
                flatOrders.AddRange(ex.OrderBook.Asks.Select(a => a.Order)
                    .Where(o => OrderTypeHelper.Parse(o.Type) == OrderType.Sell)
                    .Select(ask => (ask, ex.Id)));
            }
            else if (orderType == OrderType.Sell && ex.AvailableFunds.Crypto > 0 && ex.OrderBook.Bids.Any())
            {
                // Only include bids where exchange has Crypto
                flatOrders.AddRange(ex.OrderBook.Bids.Select(b => b.Order)
                    .Where(o => OrderTypeHelper.Parse(o.Type) == OrderType.Buy)
                    .Select(bid => (bid, ex.Id)));
            }
        }

        // 2. Sort the global list for best price
        var sortedOrders = (orderType == OrderType.Buy)
            ? flatOrders.OrderBy(x => x.order.Price).ToList()
            : flatOrders.OrderByDescending(x => x.order.Price).ToList();

        // 3. Greedily fill orders
        decimal remainingAmount = amount;
        decimal totalFilled = 0;
        decimal totalCostOrProceeds = 0;

        // To group by exchange for reporting
        var exchangeExecs = new Dictionary<string, ExchangeExecution>();

        // Track available funds for each exchange (as they change during filling)
        var exchangeFunds = exchanges.ToDictionary(
            ex => ex.Id,
            ex => orderType == OrderType.Buy ? ex.AvailableFunds.Euro : ex.AvailableFunds.Crypto
        );

        foreach (var (order, exchangeId) in sortedOrders)
        {
            if (remainingAmount <= 0)
                break;

            decimal available = exchangeFunds[exchangeId]; // funds available for this exchange
            decimal maxOrderAmount = order.Amount; // How much the order can offer.
            /*
                Calculate the maximum fillable amount from this order, limited by:
               - the order's available quantity, and
               - our available funds on this exchange (either EUR if buying, or crypto if selling).
                 For buys: We can afford at most (available EUR / order price) units at this price.
                 For sells: We can only sell as much crypto as we have.
                In both cases, we never take more than the order actually offers.
            */
            decimal maxByFunds = orderType == OrderType.Buy
                ? Math.Min(available / order.Price, maxOrderAmount)
                : Math.Min(available, maxOrderAmount);

            ;

            decimal fill = Math.Min(maxByFunds, remainingAmount);
            if (fill <= 0) continue;

            if (!exchangeExecs.TryGetValue(exchangeId, out var exchExec))
            {
                exchExec = new ExchangeExecution
                {
                    ExchangeId = exchangeId,
                    Matches = new List<OrderMatch>(),
                    FilledAmount = 0,
                    ExchangeCostOrProceeds = 0
                };
                exchangeExecs[exchangeId] = exchExec;
            }

            exchExec.Matches.Add(new OrderMatch
            {
                OrderId = order.Id,
                Amount = fill,
                Price = order.Price
            });

            exchExec.FilledAmount += fill;
            exchExec.ExchangeCostOrProceeds += fill * order.Price;

            // Deduct funds used from the exchange
            if (orderType == OrderType.Buy)
                exchangeFunds[exchangeId] -= fill * order.Price;
            else
                exchangeFunds[exchangeId] -= fill;

            totalFilled += fill;
            totalCostOrProceeds += fill * order.Price;
            remainingAmount -= fill;
        }

        decimal avgPrice = totalFilled > 0 ? totalCostOrProceeds / totalFilled : 0;

        return new ExecutionPlan
        {
            OrderType = OrderTypeHelper.ToString(orderType),
            RequestedAmount = amount,
            FilledAmount = totalFilled,
            ExchangeExecutions = exchangeExecs.Values.ToList(),
            TotalCostOrProceeds = totalCostOrProceeds,
            AveragePrice = avgPrice,
            IsFullyFilled = totalFilled == amount
        };
    }
}