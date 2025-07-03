using System.Text.Json;
using ExchangeExecutionPlanner.Models;

namespace ExchangeExecutionPlanner.Repositories;

public class JsonExchangeRepository : IExchangeRepository
{
    private const string DefaultFolderPath = "Data/Exchanges";
    private readonly string _folderPath;

    public JsonExchangeRepository(string? folderPath = null)
    {
        _folderPath = folderPath ?? DefaultFolderPath;
    }

    public async Task<List<Exchange>> LoadAllExchangesAsync()
    {
        var result = new List<Exchange>();
        var files = Directory.GetFiles(_folderPath, "*.json");

        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file);
            try
            {
                var exchange = JsonSerializer.Deserialize<Exchange>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (exchange != null)
                    result.Add(exchange);
            }
            catch (JsonException ex)
            {
                // Simple logging;
                Console.WriteLine($"[WARN] Failed to parse exchange file: {file}. Error: {ex.Message}");
            }
        }

        return result;
    }
    
    public async Task<int> GetExchangeCountAsync()
    {
        try
        {
            return await Task.Run(() =>
            {
                var files = Directory.GetFiles(_folderPath, "*.json");
                Console.WriteLine($"[INFO] Found {files.Length} exchange JSON files in: {_folderPath}");
                return files.Length;
            });
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine($"[ERROR] Exchange data directory not found: {_folderPath}. Error: {ex.Message}");
            return -1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Unexpected error while counting exchanges in: {_folderPath}. Error: {ex.Message}");
            return -1;
        }
    }

}