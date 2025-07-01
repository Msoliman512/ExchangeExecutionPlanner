FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExchangeExecutionPlanner.Api/ExchangeExecutionPlanner.Api.csproj", "ExchangeExecutionPlanner.Api/"]
RUN dotnet restore "ExchangeExecutionPlanner.Api/ExchangeExecutionPlanner.Api.csproj"
COPY . .
WORKDIR "/src/ExchangeExecutionPlanner.Api"
RUN dotnet build "ExchangeExecutionPlanner.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExchangeExecutionPlanner.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Data ./Data    
ENTRYPOINT ["dotnet", "ExchangeExecutionPlanner.Api.dll"]
