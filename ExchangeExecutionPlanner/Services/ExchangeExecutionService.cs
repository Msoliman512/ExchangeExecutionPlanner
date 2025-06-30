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

    public async Task<ExecutionPlan> FindBestExecutionAsync(string orderType, decimal amount)
    {
        var exchanges = await _repo.LoadAllExchangesAsync(); // Get data from repo (adjust args as needed)
        // TODO: Logic to find the best execution plan
        throw new NotImplementedException();
    }

    public async Task<List<ExecutionPlan>> GetExecutionPlansAsync(string orderType, decimal amount)
    {
        var exchanges = await _repo.LoadAllExchangesAsync();
        // TODO: Logic to get all execution plans
        throw new NotImplementedException();
    }
}
