using Microsoft.Extensions.DependencyInjection;
using ExchangeExecutionPlanner.Services;
using ExchangeExecutionPlanner.Models;
using ExchangeExecutionPlanner.Repositories;

Console.WriteLine("=== Exchange Execution Planner ===");

// 1. Configure DI
var services = new ServiceCollection();

// Register the repository and the service
services.AddScoped<IExchangeRepository, JsonExchangeRepository>();
services.AddScoped<IExchangeExecutionService, ExchangeExecutionService>();

// 2. Build service provider
var provider = services.BuildServiceProvider();

// 3. Get the service or repo from DI
var executionService = provider.GetRequiredService<IExchangeExecutionService>();

while (true)
{
    Console.WriteLine("\n----------------------------");
    Console.WriteLine("Please select order type:");
    Console.WriteLine("1 - Buy");
    Console.WriteLine("2 - Sell");
    Console.Write("Enter choice (1/2): ");
    var typeInput = Console.ReadLine();
    if (typeInput == null || (typeInput != "1" && typeInput != "2"))
    {
        Console.WriteLine("Invalid choice. Try again.");
        continue;
    }

    var orderType = typeInput == "1" ? OrderType.Buy : OrderType.Sell;

    Console.Write("\nEnter desired amount (in BTC): ");
    var amountInput = Console.ReadLine();
    if (!decimal.TryParse(amountInput, out var amount) || amount <= 0)
    {
        Console.WriteLine("Invalid amount. Please enter a positive number.");
        continue;
    }

    Console.WriteLine("\nAmount is in BTC.");
    Console.WriteLine("Finding best execution plan...");

    var plan = await executionService.FindBestExecutionAsync(orderType, amount);

    // Print all properties of the ExecutionPlan object, with clear formatting
    Console.WriteLine("\n=== Execution Plan Result ===");
    Console.WriteLine($"Order Type            : {plan.OrderType}");
    Console.WriteLine($"Requested Amount      : {plan.RequestedAmount} BTC");
    Console.WriteLine($"Filled Amount         : {plan.FilledAmount} BTC");
    Console.WriteLine($"Total Cost/Proceeds   : {plan.TotalCostOrProceeds:F2} EUR");
    Console.WriteLine($"Average Price         : {plan.AveragePrice:F2} EUR/BTC");
    Console.WriteLine($"Is Fully Filled?      : {(plan.IsFullyFilled ? "Yes" : "No")}");
    Console.WriteLine($"# of Exchange Executions: {plan.ExchangeExecutions.Count}");

    if (plan.ExchangeExecutions.Count > 0)
    {
        Console.WriteLine("\n=== Per Exchange Breakdown ===");
        foreach (var exec in plan.ExchangeExecutions)
        {
            Console.WriteLine($"\n[Exchange: {exec.ExchangeId}]");
            Console.WriteLine($"  Filled Amount          : {exec.FilledAmount} BTC");
            Console.WriteLine($"  Exchange Cost/Proceeds : {exec.ExchangeCostOrProceeds:F2} EUR");
            Console.WriteLine($"  Number of Matches      : {exec.Matches.Count}");

            if (exec.Matches.Count > 0)
            {
                Console.WriteLine("  Order Matches:");
                foreach (var match in exec.Matches)
                {
                    Console.WriteLine($"    - OrderId : {match.OrderId}");
                    Console.WriteLine($"      Amount  : {match.Amount} BTC");
                    Console.WriteLine($"      Price   : {match.Price} EUR/BTC");
                }
            }
            else
            {
                Console.WriteLine("  No order matches.");
            }
        }
    }
    else
    {
        Console.WriteLine("No exchanges were able to fill this order.");
    }

    Console.WriteLine("\n----------------------------");
    Console.Write("Would you like to perform another operation? (y/n): ");
    var again = Console.ReadLine();
    if (again == null || again.Trim().ToLower() != "y")
        break;
}

Console.WriteLine("\nThank you for using Exchange Execution Planner! Goodbye.");