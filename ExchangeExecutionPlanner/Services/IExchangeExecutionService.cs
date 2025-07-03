using ExchangeExecutionPlanner.Models;

namespace ExchangeExecutionPlanner.Services;

public interface IExchangeExecutionService
{
    Task<ExecutionPlan> FindBestExecutionAsync(OrderType orderType, decimal amount);
    Task<int> GetExchangeCountAsync();
}