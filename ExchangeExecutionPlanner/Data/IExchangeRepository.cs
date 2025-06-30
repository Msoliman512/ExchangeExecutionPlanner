using ExchangeExecutionPlanner.Models;

namespace ExchangeExecutionPlanner.Data;

public interface IExchangeRepository
{
    Task<List<Exchange>> LoadAllExchangesAsync();
}