using Microsoft.AspNetCore.Mvc;
using System.Net;
using ExchangeExecutionPlanner.Api.Utils;
using ExchangeExecutionPlanner.Services;
using ExchangeExecutionPlanner.Models;

namespace ExchangeExecutionPlanner.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExecutionPlanController : ControllerBase
{
    private readonly IExchangeExecutionService _service;

    public ExecutionPlanController(IExchangeExecutionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Returns the best execution plan for the requested order type and amount.
    /// </summary>
    /// <param name="orderType">Order type: 'Buy' or 'Sell'.</param>
    /// <param name="amount">Order amount in BTC (must be positive).</param>
    /// <returns>The best execution plan matching the order type and amount.</returns>
    /// <response code="200">Returns the execution plan.</response>
    /// <response code="400">Invalid order type or amount.</response>
    /// <response code="500">Internal server error.</response>
    /// <remarks>
    /// This endpoint takes an order type (Buy or Sell) and an amount (BTC), and returns the best execution plan across all exchanges.
    /// </remarks>
    [Produces("application/json")]
    [ProducesResponseType(typeof(ExecutionPlan), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetExecutionPlan(
        [FromQuery] string orderType,
        [FromQuery] decimal amount)
    {
        if (amount <= 0)
            return BadRequest("Amount must be a positive number.");

        // Only accept 'Buy' or 'Sell' (case-insensitive), but NOT numbers like '0' or '1'
        if (string.IsNullOrWhiteSpace(orderType) || !OrderTypeValidator.IsAllowedOrderType(orderType))
            return BadRequest("OrderType must be 'Buy' or 'Sell'.");

        Enum.TryParse<OrderType>(orderType, true, out var orderTypeEnum);

        var plan = await _service.FindBestExecutionAsync(orderTypeEnum, amount);
        return Ok(plan);
    }

    /// <summary>
    /// Returns the count of exchange data files loaded (number of exchanges).
    /// </summary>
    /// <returns>The count of exchanges loaded from the data directory.</returns>
    /// <response code="200">Returns the exchange count as a JSON object.</response>
    /// <response code="500">Internal server error.</response>
    /// <remarks>
    /// This endpoint returns the number of exchange JSON files loaded by the API.
    /// </remarks>
    [Produces("application/json")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [HttpGet("exchange-count")]
    public async Task<IActionResult> GetExchangeCount()
    {
        try
        {
            var count = await _service.GetExchangeCountAsync();
            if (count < 0)
                return StatusCode(500, "Could not load exchange count.");
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to get exchange count: {ex.Message}");
        }
    }
}