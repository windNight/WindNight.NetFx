# WindNight.NetFx NuGet Packages

[TOC]

## Overview

WindNight.NetFx is a comprehensive .NET library ecosystem that provides foundational components for building scalable applications. It offers a modular architecture with multiple NuGet packages covering different concerns including configuration management, data access, logging, messaging, and web hosting.

The library is open-source, hosted on GitHub under the MIT License, and supports multi-targeting across .NET Standard 2.0/2.1, .NET 8.0, and .NET 9.0.

## Package Catalog

### Core Libraries

#### WindNight.Core
**Base abstractions and core utilities**
```powershell
dotnet add package WindNight.Core
```
- Entity abstractions (`IEntity`, `EntityBase`)
- Business exceptions and error handling
- Extension methods for common types
- JSON and configuration utilities
- Hardware and system information helpers

#### WindNight.Extension  
**Extended utilities and helpers**
```powershell
dotnet add package WindNight.Extension
```
- HTTP client helpers with RestSharp integration
- IP address utilities and detection
- Comprehensive logging system (`LogHelper`)
- Call context management for .NET Core

#### WindNight.Config
**Configuration management system**
```powershell
dotnet add package WindNight.Config
```
- `ConfigCenterContext` for centralized configuration
- Multiple configuration sources support
- Hot reload capabilities
- Strongly-typed configuration classes

#### WindNight.Config.Extensions
**Configuration API extensions**
```powershell
dotnet add package WindNight.Config.Extensions
```
- RESTful configuration APIs
- Configuration controller implementations
- Authentication for configuration endpoints

### ASP.NET Core Components

#### WindNight.AspNetCore.Hosting
**Web hosting infrastructure**
```powershell
dotnet add package WindNight.AspNetCore.Hosting
```
- `ProgramBase` pattern for standardized host building
- Exception handling and logging setup
- Service registration helpers
- Background service support

#### WindNight.AspNetCore.Mvc.Extensions
**MVC extensions and filters**
```powershell
dotnet add package WindNight.AspNetCore.Mvc.Extensions
```
- Custom action filters and attributes
- API authorization helpers
- HTTP request extensions
- Swagger integration helpers

#### WindNight.AspNetCore.GRpc.Hosting
**gRPC hosting infrastructure**
```powershell
dotnet add package WindNight.AspNetCore.GRpc.Hosting
```
- gRPC service hosting
- Swagger integration for gRPC
- HTTP API bridge for gRPC services

### Data Access

#### WindNight.Extension.Dapper
**Base Dapper extensions**
```powershell
dotnet add package WindNight.Extension.Dapper
```
- Common Dapper utilities and extensions
- Base repository patterns

#### WindNight.Extension.Dapper.Mssql
**SQL Server implementation**
```powershell
dotnet add package WindNight.Extension.Dapper.Mssql
```
- `SqlServerBase` repository implementations
- SQL Server specific extensions
- Connection management

#### WindNight.Extension.Dapper.Mysql
**MySQL implementation**
```powershell
dotnet add package WindNight.Extension.Dapper.Mysql
```
- `MySqlBase` repository implementations
- MySQL specific extensions and helpers
- Tree structure support (`MySqlTreeBase`)

#### WindNight.Extension.Db.Abstractions
**Database abstractions**
```powershell
dotnet add package WindNight.Extension.Db.Abstractions
```
- Repository interface definitions
- Entity abstractions
- Query and pagination interfaces

### Infrastructure & Messaging

#### WindNight.RabbitMq
**RabbitMQ integration**
```powershell
dotnet add package WindNight.RabbitMq
```
- Producer and consumer implementations
- Background service support
- Message wrapper abstractions
- Configuration management

### Logging & Monitoring

#### WindNight.Extension.DbLog.Mysql
**Database logging provider**
```powershell
dotnet add package WindNight.Extension.DbLog.Mysql
```
- MySQL database logging provider
- Structured log storage
- Log processing and management

#### WindNight.Extension.LogStore
**Log storage abstractions**
```powershell
dotnet add package WindNight.Extension.LogStore
```
- Abstract log storage interfaces
- Multiple storage backend support
- Log processing pipeline

### Extensions

#### Swashbuckle.AspNetCore.HideApi
**Swagger API management**
```powershell
dotnet add package Swashbuckle.AspNetCore.HideApi
```
- Hide APIs from Swagger documentation
- Debug and system API filtering
- Configuration-based API visibility

## Quick Start

### Basic ASP.NET Core Application

```csharp
using WindNight.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MyApp
{
    public class Program : DefaultProgramBase
    {
        public static async Task Main(string[] args)
        {
            var buildType = GetBuildType();
            await InitAsync(CreateHostBuilder, buildType, () => Task.CompletedTask, args);
        }

        private static string GetBuildType()
        {
#if DEBUG
            return "Debug";
#else
            return "Release";
#endif
        }

        private static IHostBuilder CreateHostBuilder(string buildType, string[] args)
        {
            return CreateHostBuilderDefaults(buildType, args,
                configureAppConfiguration: (context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddEnvironmentVariables();
                },
                configureServicesDelegate: (context, services) =>
                {
                    // Register your services here
                    services.AddDefaultLogService(context.Configuration);
                });
        }
    }
}
```

### Configuration Management

```csharp
using WindNight.ConfigCenter.Extension;
using Microsoft.Extensions.Configuration;

// Access configuration
var config = ConfigCenterContext.Configuration;
var connectionString = config.GetConnectionString("DefaultConnection");

// Strongly-typed configuration
public class AppSettings : DefaultConfigItemBase
{
    public string ApiKey { get; set; }
    public int Timeout { get; set; }
}

var appSettings = config.GetSection("AppSettings").Get<AppSettings>();
```

### Data Access with Repository Pattern

```csharp
using WindNight.Extension.Dapper.Mysql;

public class UserRepository : MySqlBase<User, int>
{
    public UserRepository(string connectionString) : base(connectionString) { }
    
    public override string DefaultTableName => "users";
    
    // Custom methods inherit base CRUD operations
    public async Task<User> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM users WHERE email = @email";
        return await QueryFirstOrDefaultAsync<User>(sql, new { email });
    }
}
```

### Logging

```csharp
using WindNight.LogExtension;

// Simple logging
LogHelper.Info("Application started");
LogHelper.Error("An error occurred", exception);

// Structured logging
LogHelper.Debug($"Processing user {userId}", new { UserId = userId, Action = "Process" });
```

## Target Frameworks

All packages support:
- **.NET Standard 2.0** - Compatible with .NET Framework 4.6.1+
- **.NET Standard 2.1** - Compatible with .NET Core 3.0+
- **.NET 8.0** - Latest LTS support
- **.NET 9.0** - Current version support

## Repository

**GitHub**: https://github.com/windNight/WindNight.NetFx.git

## License

MIT License - see LICENSE file for details