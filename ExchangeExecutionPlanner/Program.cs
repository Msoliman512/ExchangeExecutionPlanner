using Microsoft.Extensions.DependencyInjection;
using ExchangeExecutionPlanner.Data;
using ExchangeExecutionPlanner.Services;

Console.WriteLine("ExchangeExecutionPlanner");

// 1. Configure DI
var services = new ServiceCollection();

// Register the repository and the service
services.AddScoped<IExchangeRepository, JsonExchangeRepository>();
services.AddScoped<IExchangeExecutionService, ExchangeExecutionService>();

// 2. Build service provider
var provider = services.BuildServiceProvider();

// 3. Get the service or repo from DI
var repo = provider.GetRequiredService<IExchangeRepository>();

// (OPTIONAL) Or get your execution service:
var executionService = provider.GetRequiredService<IExchangeExecutionService>();

// Use it!
var exchanges = await repo.LoadAllExchangesAsync();
Console.WriteLine($"Loaded {exchanges.Count} exchanges");

// Example usage of service (you need to implement the logic first!)
// var bestPlan = await executionService.FindBestExecutionAsync("Buy", 1.2m);
// Console.WriteLine($"Best plan: {bestPlan}");