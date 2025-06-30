using ExchangeExecutionPlanner.Models;

namespace ExchangeExecutionPlanner.Services;

public interface IExchangeExecutionService
{
    Task<ExecutionPlan> FindBestExecutionAsync(string orderType, decimal amount);
}