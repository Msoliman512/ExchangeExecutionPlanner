# Exchange Execution Planner

A meta-exchange system that finds the best possible prices across multiple cryptocurrency exchanges for Bitcoin (BTC) trading. The system optimizes buy/sell orders to achieve the lowest cost when buying or highest revenue when selling BTC, while respecting each exchange's balance constraints.

## Overview

This project implements a smart order routing system that:
- Analyzes order books from multiple cryptocurrency exchanges
- Finds optimal execution plans for buying or selling Bitcoin
- Respects individual exchange EUR and BTC balance limits
- Provides both console application and REST API interfaces

The system works by aggregating order books from different exchanges and using a greedy algorithm to select the best prices while ensuring sufficient balances are available on each exchange.

## Architecture

The solution follows a clean architecture pattern with clear separation of concerns:

- **Core Application**: Contains business logic, models, and service interfaces
- **Data Access Layer**: Repository pattern for extensible data access (currently file-based) 
- **Service Layer**: Business logic for order execution planning
- **API Project**: REST endpoints with Swagger documentation
- **Console Application**: Command-line interface for testing
- **Unit Tests**: Comprehensive test coverage for core functionality
- **Shared Data**: Exchange data files stored in `Data/Exchanges` directory

## Project Structure

```
ExchangeExecutionPlanner/
├── ExchangeExecutionPlanner.Core/          # Core business logic
├── ExchangeExecutionPlanner.Api/           # REST API endpoints
├── ExchangeExecutionPlanner.Console/       # Console application
├── ExchangeExecutionPlanner.Tests/         # Unit tests
├── Data/Exchanges/                          # Exchange data files
└── README.md
```

## Features

### Part 1: Console Application
- Reads order books from JSON files
- Processes buy/sell orders with specified amounts
- Outputs optimal execution plans
- Handles balance constraints across exchanges

### Part 2: REST API
- RESTful endpoints for order execution planning
- Swagger documentation for API exploration
- JSON responses with detailed execution plans
- Docker support for easy deployment

## Prerequisites

- .NET 8.0 SDK or later
- Docker (optional, for containerized deployment)

## Installation

### 1. Check .NET Version
```bash
dotnet --version
```

If you don't have .NET 8.0 installed, download it from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download).

### 2. Clone the Repository
```bash
git clone https://github.com/Msoliman512/ExchangeExecutionPlanner.git
cd ExchangeExecutionPlanner
```

### 3. **IMPORTANT**: Add Exchange Data Files
Before running the application, ensure you have the exchange data files in the `Data/Exchanges` directory. These JSON files contain the order book data and balance information for each exchange.

```
Data/Exchanges/
├── exchange1.json
├── exchange2.json
└── ...
```

## Running the Application

### Console Application
```bash
cd ExchangeExecutionPlanner.Console
dotnet run
```

### API (Local Development)
```bash
cd ExchangeExecutionPlanner.Api
dotnet run
```

The API will be available at `https://localhost:7071` with Swagger documentation at `https://localhost:7071/swagger/index.html`. "ports changes according to availability"

## Docker Deployment

### Build the Docker Image
```bash
docker build --no-cache -t exchange-execution-api -f ExchangeExecutionPlanner.Api/Dockerfile .
```

### Run the Container
```bash
docker run -it --rm -p 8080:80 -e ASPNETCORE_HTTP_PORTS=80 -e ExchangeDataFolder=Data/Exchanges -e ASPNETCORE_ENVIRONMENT=Development exchange-execution-api
```

### Access the API
- **Swagger Documentation**: http://localhost:8080/swagger/index.html
- **Direct API Access**: http://localhost:8080/ExecutionPlan?orderType=sell&amount=2

## API Usage Examples

### Browser
```
http://localhost:8080/ExecutionPlan?orderType=sell&amount=2
```

### JavaScript (Browser Console)
```javascript
fetch('http://localhost:8080/ExecutionPlan?orderType=sell&amount=2')
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(err => console.error('Error:', err));
```

### cURL (Terminal)
```bash
curl "http://localhost:8080/ExecutionPlan?orderType=sell&amount=2"
```

## Testing

Run the unit tests:
```bash
dotnet test
```

## Enhancement Roadmap

### Future Enhancements
- **Architecture Refactoring**: Split into Core/Infrastructure/API projects for better separation of concerns
- **Integration Testing**: Add comprehensive API endpoint testing with WebApplicationFactory
- **CI/CD Pipeline**: Implement GitHub Actions for automated building and testing
- **Database Integration**: Add support for SQL/NoSQL databases alongside file-based storage
- **Health Checks & Observability**: Add health endpoints and comprehensive logging
- **Code Quality**: Implement linting, nullable reference checks, and enhanced error handling
- **OpenAPI Enhancements**: Add comprehensive XML documentation and FluentValidation

### Quick Wins
- Move repeated logic into Core/Utils
- Add robust error messages with ProblemDetails responses
- Enhanced request validation and error handling

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Support

For questions or issues, please open an issue on the GitHub repository.
