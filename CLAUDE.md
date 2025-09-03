# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WindNight.NetFx is a comprehensive .NET library ecosystem that provides foundational components for building scalable applications. The project uses a modular architecture with multiple NuGet packages covering different concerns.

## Core Architecture

### Project Structure
- **Core Libraries**: WindNight.Core (base abstractions), WindNight.Extension (utilities), WindNight.Config (configuration management)
- **ASP.NET Core**: WindNight.AspNetCore.Hosting (web hosting), WindNight.AspNetCore.Mvc.Extensions (MVC extensions)
- **Data Access**: Dapper extensions for MSSQL/MySQL, database abstractions, repository patterns
- **Infrastructure**: RabbitMQ integration, logging extensions, job scheduling with Quartz
- **gRPC**: Complete gRPC hosting, HTTP API bridge, and Swagger integration
- **Tools**: Swagger API hiding, data source testing, ReSharper emoji plugin

### Key Patterns
- Uses Microsoft.Extensions.* ecosystem for DI, configuration, and logging
- Custom LogHelper system with console output and structured logging
- Configuration management through ConfigCenterContext with multiple sources
- Background services pattern for long-running tasks
- Repository pattern with base classes for data access

## Build and Development

### Solution Structure
Main solution file: `src/WindNight.NetFx.sln`

### Build Commands
```
dotnet build src/WindNight.NetFx.sln
dotnet build src/WindNight.NetFx.sln --configuration Release
```

### Test Commands
```
dotnet test src/Tests/WindNight.Core.Tests/
dotnet test src/Tests/WindNight.Extension.Tests/
dotnet test src/Tests/WebApiTest/
```

### Package Building
Packages are automatically generated on Release builds. Output location: `Output/Release/`

### Running Examples
```
dotnet run --project src/Examples/Net8ApiDemo/
dotnet run --project src/Examples/ConsoleExamples_NetCore5/
dotnet run --project src/Examples/GrpcDemo_Net5/
```

## Configuration System

The project uses a sophisticated configuration system:
- **ConfigCenterContext**: Central configuration management
- **ConfigItems**: Strongly-typed configuration classes
- **Multiple Sources**: JSON files, environment variables, remote config centers
- **Hot Reload**: Configuration changes detected and applied at runtime

## Key Components

### ProgramBase Pattern
Most applications inherit from `ProgramBase` which provides:
- Standardized host building with `CreateHostBuilderDefaults`
- Build type detection (Debug/Release)
- Service registration delegates
- Configuration builder setup

### LogHelper System
Custom logging with:
- Multiple log levels and console output formatting
- Integration with Log4net
- Process event registration for custom log handling
- Structured logging support

### Background Services
- `SvrMonitorBackgroundService`: System monitoring
- Job scheduling through Quartz integration
- RabbitMQ consumer services

## Database Integration

### Dapper Extensions
- Base repository classes: `SqlServerBase`, `MySqlTreeBase`
- Entity abstractions with `IEntity` interface
- Pagination support through `IQueryPageBase`
- Connection string management

### Repository Pattern
- `IBaseRepo`, `IReadRepoBase`, `IWriteRepoBase` abstractions
- Entity-based CRUD operations
- Query object patterns with `DefaultQueryBase`

## Development Notes

- Projects target multiple .NET versions (netstandard2.0/2.1, net5.0, net6.0, net8.0)
- Conditional compilation symbols for platform detection
- Nullable reference types enabled
- Unsafe blocks allowed for performance-critical code
- Automatic NuGet package generation on Release builds