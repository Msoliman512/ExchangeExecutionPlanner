using System.Text.Json;
using ExchangeExecutionPlanner.Models;

namespace ExchangeExecutionPlanner.Data;

public class JsonExchangeRepository : IExchangeRepository
{
    
    private const string DefaultFolderPath = "Data/Exchanges";  
    public async Task<List<Exchange>> LoadAllExchangesAsync()
    {
        var result = new List<Exchange>();
        var files = Directory.GetFiles(DefaultFolderPath, "*.json");

        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file);
            var exchange = JsonSerializer.Deserialize<Exchange>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (exchange != null)
                result.Add(exchange);
        }
        return result;
    }
}