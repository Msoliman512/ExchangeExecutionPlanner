using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ExchangeExecutionPlanner.Models;
using ExchangeExecutionPlanner.Repositories;
using ExchangeExecutionPlanner.Services;

namespace ExchangeExecutionPlanner.Tests.Services;

    public class ExchangeExecutionServiceTests
    {
        // BUY: Single Exchange, Full Fill
        [Fact]
        public async Task FindBestExecutionAsync_Buy_FullFill_SingleExchange()
        {
            // Arrange
            var exchange = new Exchange
            {
                Id = "ex1",
                AvailableFunds = new AvailableFunds { Euro = 12000, Crypto = 0 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "ask-1", Price = 10000, Amount = 1, Type = "sell" } }
                    },
                    Bids = new List<OrderEntry>()
                }
            };
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.LoadAllExchangesAsync()).ReturnsAsync(new List<Exchange> { exchange });
            var service = new ExchangeExecutionService(repoMock.Object);

            // Act
            var plan = await service.FindBestExecutionAsync(OrderType.Buy, 1);

            // Assert
            Assert.True(plan.IsFullyFilled);
            Assert.Equal(1, plan.FilledAmount);
            Assert.Single(plan.ExchangeExecutions);
            Assert.Equal(10000, plan.TotalCostOrProceeds);
            Assert.Equal(10000, plan.AveragePrice);
        }

        // BUY: Partial fill due to insufficient funds
        [Fact]
        public async Task FindBestExecutionAsync_Buy_PartialFill_InsufficientFunds()
        {
            var exchange = new Exchange
            {
                Id = "ex1",
                AvailableFunds = new AvailableFunds { Euro = 4000, Crypto = 0 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "ask-1", Price = 8000, Amount = 1, Type = "sell" } }
                    },
                    Bids = new List<OrderEntry>()
                }
            };
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.LoadAllExchangesAsync()).ReturnsAsync(new List<Exchange> { exchange });
            var service = new ExchangeExecutionService(repoMock.Object);

            // Act
            var plan = await service.FindBestExecutionAsync(OrderType.Buy, 1);

            // Assert
            Assert.False(plan.IsFullyFilled);
            Assert.Equal(0.5m, plan.FilledAmount);
            Assert.Equal(4000, plan.TotalCostOrProceeds);
            Assert.Equal(8000, plan.AveragePrice);
        }

        // SELL: Full Fill, Single Exchange
        [Fact]
        public async Task FindBestExecutionAsync_Sell_FullFill_SingleExchange()
        {
            var exchange = new Exchange
            {
                Id = "ex1",
                AvailableFunds = new AvailableFunds { Euro = 0, Crypto = 2 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>(),
                    Bids = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "bid-1", Price = 12000, Amount = 1, Type = "buy" } }
                    }
                }
            };
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.LoadAllExchangesAsync()).ReturnsAsync(new List<Exchange> { exchange });
            var service = new ExchangeExecutionService(repoMock.Object);

            var plan = await service.FindBestExecutionAsync(OrderType.Sell, 1);

            Assert.True(plan.IsFullyFilled);
            Assert.Equal(1, plan.FilledAmount);
            Assert.Single(plan.ExchangeExecutions);
            Assert.Equal(12000, plan.TotalCostOrProceeds);
            Assert.Equal(12000, plan.AveragePrice);
        }

        // SELL: Partial fill due to not enough crypto
        [Fact]
        public async Task FindBestExecutionAsync_Sell_PartialFill_NotEnoughCrypto()
        {
            var exchange = new Exchange
            {
                Id = "ex1",
                AvailableFunds = new AvailableFunds { Euro = 0, Crypto = 0.3m },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>(),
                    Bids = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "bid-1", Price = 11000, Amount = 1, Type = "buy" } }
                    }
                }
            };
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.LoadAllExchangesAsync()).ReturnsAsync(new List<Exchange> { exchange });
            var service = new ExchangeExecutionService(repoMock.Object);

            var plan = await service.FindBestExecutionAsync(OrderType.Sell, 1);

            Assert.False(plan.IsFullyFilled);
            Assert.Equal(0.3m, plan.FilledAmount);
            Assert.Equal(3300, plan.TotalCostOrProceeds);
            Assert.Equal(11000, plan.AveragePrice);
        }

        // BUY: Best price from multiple exchanges (picks lowest ask)
        [Fact]
        public async Task FindBestExecutionAsync_Buy_ChoosesBestPrice_AcrossExchanges()
        {
            var ex1 = new Exchange
            {
                Id = "ex1",
                AvailableFunds = new AvailableFunds { Euro = 10000, Crypto = 0 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "ask-1", Price = 11000, Amount = 1, Type = "sell" } }
                    },
                    Bids = new List<OrderEntry>()
                }
            };
            var ex2 = new Exchange
            {
                Id = "ex2",
                AvailableFunds = new AvailableFunds { Euro = 10000, Crypto = 0 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "ask-2", Price = 9000, Amount = 1, Type = "sell" } }
                    },
                    Bids = new List<OrderEntry>()
                }
            };
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.LoadAllExchangesAsync()).ReturnsAsync(new List<Exchange> { ex1, ex2 });
            var service = new ExchangeExecutionService(repoMock.Object);

            var plan = await service.FindBestExecutionAsync(OrderType.Buy, 1);

            Assert.True(plan.IsFullyFilled);
            Assert.Equal(1, plan.FilledAmount);
            Assert.Single(plan.ExchangeExecutions);
            Assert.Equal("ex2", plan.ExchangeExecutions[0].ExchangeId);
            Assert.Equal(9000, plan.AveragePrice);
        }

        // SELL: Best price from multiple exchanges (picks highest bid)
        [Fact]
        public async Task FindBestExecutionAsync_Sell_ChoosesBestPrice_AcrossExchanges()
        {
            var ex1 = new Exchange
            {
                Id = "ex1",
                AvailableFunds = new AvailableFunds { Euro = 0, Crypto = 1 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>(),
                    Bids = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "bid-1", Price = 9000, Amount = 1, Type = "buy" } }
                    }
                }
            };
            var ex2 = new Exchange
            {
                Id = "ex2",
                AvailableFunds = new AvailableFunds { Euro = 0, Crypto = 1 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>(),
                    Bids = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "bid-2", Price = 12000, Amount = 1, Type = "buy" } }
                    }
                }
            };
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.LoadAllExchangesAsync()).ReturnsAsync(new List<Exchange> { ex1, ex2 });
            var service = new ExchangeExecutionService(repoMock.Object);

            var plan = await service.FindBestExecutionAsync(OrderType.Sell, 1);

            Assert.True(plan.IsFullyFilled);
            Assert.Equal(1, plan.FilledAmount);
            Assert.Single(plan.ExchangeExecutions);
            Assert.Equal("ex2", plan.ExchangeExecutions[0].ExchangeId);
            Assert.Equal(12000, plan.AveragePrice);
        }

        // EDGE: No orders at all (empty order book)
        [Fact]
        public async Task FindBestExecutionAsync_NoOrders_ReturnsZero()
        {
            var ex1 = new Exchange
            {
                Id = "ex1",
                AvailableFunds = new AvailableFunds { Euro = 10000, Crypto = 1 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>(),
                    Bids = new List<OrderEntry>()
                }
            };
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.LoadAllExchangesAsync()).ReturnsAsync(new List<Exchange> { ex1 });
            var service = new ExchangeExecutionService(repoMock.Object);

            var plan = await service.FindBestExecutionAsync(OrderType.Buy, 1);

            Assert.Equal(0, plan.FilledAmount);
            Assert.Empty(plan.ExchangeExecutions);
            Assert.False(plan.IsFullyFilled);
        }

        // EDGE: No available funds (should not fill)
        [Fact]
        public async Task FindBestExecutionAsync_NoFunds_ReturnsZero()
        {
            var ex1 = new Exchange
            {
                Id = "ex1",
                AvailableFunds = new AvailableFunds { Euro = 0, Crypto = 0 },
                OrderBook = new OrderBook
                {
                    Asks = new List<OrderEntry>
                    {
                        new() { Order = new Order { Id = "ask-1", Price = 10000, Amount = 1, Type = "sell" } }
                    },
                    Bids = new List<OrderEntry>()
                }
            };
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.LoadAllExchangesAsync()).ReturnsAsync(new List<Exchange> { ex1 });
            var service = new ExchangeExecutionService(repoMock.Object);

            var plan = await service.FindBestExecutionAsync(OrderType.Buy, 1);

            Assert.Equal(0, plan.FilledAmount);
            Assert.Empty(plan.ExchangeExecutions);
            Assert.False(plan.IsFullyFilled);
        }
        
        [Fact]
        public async Task GetExchangeCountAsync_Returns_CorrectCount()
        {
            // Arrange
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.GetExchangeCountAsync()).ReturnsAsync(3);
            var service = new ExchangeExecutionService(repoMock.Object);

            // Act
            var count = await service.GetExchangeCountAsync();

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task GetExchangeCountAsync_RepoReturnsNegative_ReturnsNegative()
        {
            // Arrange
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.GetExchangeCountAsync()).ReturnsAsync(-1);
            var service = new ExchangeExecutionService(repoMock.Object);

            // Act
            var count = await service.GetExchangeCountAsync();

            // Assert
            Assert.Equal(-1, count);
        }

        [Fact]
        public async Task GetExchangeCountAsync_RepoThrowsException_ReturnsMinusOneAndLogs()
        {
            // Arrange
            var repoMock = new Mock<IExchangeRepository>();
            repoMock.Setup(r => r.GetExchangeCountAsync()).ThrowsAsync(new Exception("fail"));
            var service = new ExchangeExecutionService(repoMock.Object);

            // Act
            var count = await service.GetExchangeCountAsync();

            // Assert
            Assert.Equal(-1, count);
        }

    }

