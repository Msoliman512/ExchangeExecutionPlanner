﻿FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExchangeExecutionPlanner/ExchangeExecutionPlanner.csproj", "ExchangeExecutionPlanner/"]
RUN dotnet restore "ExchangeExecutionPlanner/ExchangeExecutionPlanner.csproj"
COPY . .
WORKDIR "/src/ExchangeExecutionPlanner"
RUN dotnet build "ExchangeExecutionPlanner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExchangeExecutionPlanner.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExchangeExecutionPlanner.dll"]
