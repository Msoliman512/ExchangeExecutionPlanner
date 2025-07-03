# Exchange Execution Planner

A meta-exchange system that finds the best possible prices across multiple cryptocurrency exchanges for Bitcoin (BTC) trading. The system optimizes buy/sell orders to achieve the lowest cost when buying or highest revenue when selling BTC, while respecting each exchange's balance constraints.

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)](https://www.docker.com/)

## Overview

This project implements a smart order routing system that:
- Analyzes order books from multiple cryptocurrency exchanges
- Finds optimal execution plans for buying or selling Bitcoin
- Respects individual exchange EUR and BTC balance limits
- Provides both console application and REST API interfaces

The system works by aggregating order books from different exchanges and using a greedy algorithm to select the best prices while ensuring sufficient balances are available on each exchange.

## Technologies Used

**Backend**
- **.NET 8** – Main framework for both the console and API applications
- **C#** – Core language for business logic, data models, and services
- **ASP.NET Core Web API** – For RESTful endpoint implementation
- **xUnit** – Unit testing framework for .NET
- **Moq** – Mocking library for writing effective unit tests
- **Swagger (Swashbuckle)** – Auto-generated, interactive API documentation
- **Docker** – Containerization for easy deployment and consistent environments
- **JSON** – Data exchange format for exchange orderbooks and configuration files

**Architecture & Design Patterns**
- **Clean Architecture** – Separation of concerns with Core/Infrastructure/API layers
- **Repository Pattern** – Abstracted data access for future extensibility
- **Service Layer Pattern** – Business logic separation and organization
- **SOLID Principles** – Maintainable and testable code structure
- **Dependency Injection** – Built-in .NET DI container for loose coupling

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
├── Data/Exchanges/                        # Exchange data files (JSON)
├── ExchangeExecutionPlanner/              # Core business logic + Console application
├── ExchangeExecutionPlanner.Api/          # REST API endpoints
└── ExchangeExecutionPlanner.Tests/        # Unit tests
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

If you don't have .NET 8.0 installed, download it from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet/8.0).

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
cd ExchangeExecutionPlanner
dotnet run
```

### API (Local Development)
```bash
cd ExchangeExecutionPlanner.Api
dotnet run
```

The API will be available at `https://localhost:7071` with Swagger documentation at `https://localhost:7071/swagger/index.html`. 

*Note: Port numbers may vary based on availability.*

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

The test suite includes:
- **Core Logic Testing**: Validates order execution algorithms
- **Edge Case Coverage**: Tests boundary conditions and error scenarios
- **Repository Testing**: Ensures data access layer reliability
- **Service Layer Testing**: Verifies business logic correctness

## Enhancement Roadmap

### Future Enhancements
- **Architecture Refactoring**: Split into Core/Infrastructure/API projects for better separation of concerns. *We can split the core project to only have the service layer and repository layer and have another project for the console app so we have better separation of concerns. This way, even if we use the core app as a package, we don't have coupling issues.*
- **Integration Testing**: Add comprehensive API endpoint testing with WebApplicationFactory
- **CI/CD Pipeline**: Implement GitHub Actions for automated building and testing
- **Database Integration**: Add support for SQL/NoSQL databases alongside file-based storage
- **Health Checks & Observability**: Add health endpoints and comprehensive logging
- **Code Quality**: Implement linting, nullable reference checks, and enhanced error handling
- **OpenAPI Enhancements**: Add comprehensive XML documentation and FluentValidation

## Contributing

Contributions are always welcome!

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Add tests for new functionality
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

Please ensure your code follows the existing code style and includes appropriate tests.

## License

This project is licensed under the MIT License.

## Support

For questions or issues, please open an issue on the GitHub repository.
