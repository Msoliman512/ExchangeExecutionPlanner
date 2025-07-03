using ExchangeExecutionPlanner.Repositories;

namespace TestProject1.Repositories
{
    public class JsonExchangeRepositoryTests
    {
        private string PrepareTestData(params (string fileName, string json)[] files)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            foreach (var (fileName, json) in files)
                File.WriteAllText(Path.Combine(tempDir, fileName), json);

            return tempDir;
        }

        [Fact]
        public async Task LoadAllExchangesAsync_Returns_Exchanges_From_ValidJson()
        {
            // Arrange
            var json = @"{
                ""Id"": ""exchange-1"",
                ""AvailableFunds"": { ""Crypto"": 2.0, ""Euro"": 5000 },
                ""OrderBook"": { ""Bids"": [], ""Asks"": [] }
            }";
            var folder = PrepareTestData(("ex1.json", json));

            var repo = new JsonExchangeRepository(folder);

            // Act
            var exchanges = await repo.LoadAllExchangesAsync();

            // Assert
            Assert.Single(exchanges);
            Assert.Equal("exchange-1", exchanges[0].Id);

            // Cleanup
            Directory.Delete(folder, true);
        }

        [Fact]
        public async Task LoadAllExchangesAsync_Returns_Empty_For_EmptyFolder()
        {
            // Arrange
            var folder = PrepareTestData(); // no files
            var repo = new JsonExchangeRepository(folder);

            // Act
            var exchanges = await repo.LoadAllExchangesAsync();

            // Assert
            Assert.Empty(exchanges);

            // Cleanup
            Directory.Delete(folder, true);
        }

        [Fact]
        public async Task LoadAllExchangesAsync_Skips_InvalidJson()
        {
            // Arrange
            var validJson = @"{
        ""Id"": ""exchange-1"",
        ""AvailableFunds"": { ""Crypto"": 2.0, ""Euro"": 5000 },
        ""OrderBook"": { ""Bids"": [], ""Asks"": [] }
    }";
            var invalidJson = @"{ not a valid json";
            var folder = PrepareTestData(
                ("ex1.json", validJson),
                ("bad.json", invalidJson)
            );
            var repo = new JsonExchangeRepository(folder);

            // Act
            var exchanges = await repo.LoadAllExchangesAsync();

            // Assert
            Assert.Single(exchanges);
            Assert.Equal("exchange-1", exchanges[0].Id);

            // Cleanup
            Directory.Delete(folder, true);
        }
        
        [Fact]
        public async Task GetExchangeCountAsync_Returns_CorrectCount()
        {
            // Arrange
            var folder = PrepareTestData(
                ("ex1.json", "{}"),
                ("ex2.json", "{}"),
                ("ex3.json", "{}")
            );
            var repo = new JsonExchangeRepository(folder);

            // Act
            var count = await repo.GetExchangeCountAsync();

            // Assert
            Assert.Equal(3, count);

            // Cleanup
            Directory.Delete(folder, true);
        }

        [Fact]
        public async Task GetExchangeCountAsync_Returns_Zero_For_EmptyFolder()
        {
            // Arrange
            var folder = PrepareTestData(); // no files
            var repo = new JsonExchangeRepository(folder);

            // Act
            var count = await repo.GetExchangeCountAsync();

            // Assert
            Assert.Equal(0, count);

            // Cleanup
            Directory.Delete(folder, true);
        }

        [Fact]
        public async Task GetExchangeCountAsync_Returns_MinusOne_For_MissingFolder()
        {
            // Arrange
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()); // does not exist!
            var repo = new JsonExchangeRepository(folder);

            // Act
            var count = await repo.GetExchangeCountAsync();

            // Assert
            Assert.Equal(-1, count);
        }
    }
}