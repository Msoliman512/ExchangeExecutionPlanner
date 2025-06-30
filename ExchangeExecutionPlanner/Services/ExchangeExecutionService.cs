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
        var exchanges = await _repo.LoadAllExchangesAsync();
        // TODO: Logic to find the best execution plan
        throw new NotImplementedException();
    }
}
