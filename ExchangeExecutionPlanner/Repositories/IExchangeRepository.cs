using ExchangeExecutionPlanner.Models;

namespace ExchangeExecutionPlanner.Repositories;

public interface IExchangeRepository
{
    Task<List<Exchange>> LoadAllExchangesAsync();
}