# WindNight.NetFx

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-blue)
![Platform](https://img.shields.io/badge/platform-.NET%20Standard%202.0%2B-lightgrey)

## Overview

WindNight.NetFx is a comprehensive .NET library ecosystem that provides foundational components for building scalable applications. It offers a modular architecture with multiple NuGet packages covering configuration management, data access, logging, messaging, and web hosting.

The library leverages modern .NET practices, including ASP.NET Core, dependency injection, and multi-targeting support, making it suitable for both legacy and modern applications.

## 🚀 Key Features

- **🏗️ Modular Architecture**: 15+ specialized NuGet packages for different concerns
- **🌐 Multi-targeting**: Support for .NET Standard 2.0/2.1, .NET 8.0/9.0
- **⚙️ Configuration Management**: Centralized configuration with hot reload capabilities
- **📊 Data Access**: Repository patterns with Dapper for MySQL and SQL Server
- **📝 Comprehensive Logging**: Multi-provider logging with console, file, and database outputs
- **🔄 Background Services**: Built-in support for hosted services and job scheduling
- **🌍 Web Hosting**: ASP.NET Core hosting with standardized patterns
- **📡 gRPC Support**: Complete gRPC hosting with HTTP API bridge
- **🐰 Messaging**: RabbitMQ integration with producer/consumer patterns
- **📋 API Documentation**: Swagger integration with API visibility controls

## 📦 Package Structure

### Core Libraries
- **WindNight.Core** - Base abstractions and utilities
- **WindNight.Extension** - Extended utilities and HTTP helpers
- **WindNight.Config** - Configuration management system

### ASP.NET Core
- **WindNight.AspNetCore.Hosting** - Web hosting infrastructure
- **WindNight.AspNetCore.Mvc.Extensions** - MVC extensions and filters
- **WindNight.AspNetCore.GRpc.Hosting** - gRPC hosting support

### Data Access
- **WindNight.Extension.Dapper** - Base Dapper extensions
- **WindNight.Extension.Dapper.Mssql** - SQL Server implementation
- **WindNight.Extension.Dapper.Mysql** - MySQL implementation
- **WindNight.Extension.Db.Abstractions** - Database abstractions

### Infrastructure
- **WindNight.RabbitMq** - RabbitMQ messaging
- **WindNight.Extension.DbLog.Mysql** - Database logging
- **WindNight.Extension.LogStore** - Log storage abstractions

## 🛠️ Development Setup

### Prerequisites

- .NET SDK 8.0 or 9.0
- Visual Studio 2022 or VS Code
- Git for version control

### Clone and Build

```bash
git clone https://github.com/windNight/WindNight.NetFx.git
cd WindNight.NetFx
dotnet build src/WindNight.NetFx.sln
```

### Run Tests

```bash
# Run all tests
dotnet test src/WindNight.NetFx.sln

# Run specific test projects
dotnet test src/Tests/WindNight.Core.Tests/
dotnet test src/Tests/WindNight.Extension.Tests/
```

### Try Examples

```bash
# ASP.NET Core API Demo
dotnet run --project src/Examples/Net8ApiDemo/

# Console Examples
dotnet run --project src/Examples/ConsoleExamples_NetCore5/

# gRPC Demo
dotnet run --project src/Examples/GrpcDemo_Net5/

# RabbitMQ Demo
dotnet run --project src/Examples/RabbitMqDemos/
```

## 📚 Documentation & Examples

### 📁 Example Projects

The repository includes comprehensive examples:

- **Net8ApiDemo** - Modern ASP.NET Core 8.0 Web API
- **GrpcDemo_Net5** - gRPC service implementation
- **RabbitMqDemos** - Message producer/consumer examples
- **Logger.Demo** - Logging system demonstrations
- **ConsoleExamples** - Console application patterns

### 🔧 Architecture Patterns

#### ProgramBase Pattern
Standardized application startup with exception handling:

```csharp
public class Program : DefaultProgramBase
{
    public static async Task Main(string[] args)
    {
        var buildType = GetBuildType();
        await InitAsync(CreateHostBuilder, buildType, () => Task.CompletedTask, args);
    }
}
```

#### Repository Pattern
Data access with base repository classes:

```csharp
public class UserRepository : MySqlBase<User, int>
{
    public UserRepository(string connectionString) : base(connectionString) { }
    public override string DefaultTableName => "users";
}
```

#### Configuration System
Centralized configuration management:

```csharp
// Strongly-typed configuration
public class AppSettings : DefaultConfigItemBase
{
    public string ApiKey { get; set; }
    public int TimeoutMs { get; set; } = 30000;
}

// Usage
var settings = ConfigCenterContext.Configuration
    .GetSection("AppSettings").Get<AppSettings>();
```

## 🏗️ Project Structure

```
src/
├── WindNight.Core/                 # Core abstractions
├── WindNight.Extension/            # Utilities and helpers  
├── WindNight.Config/               # Configuration system
├── WindNight.AspNetCore.Hosting/   # Web hosting
├── WindNight.AspNetCore.Mvc.Extensions/ # MVC extensions
├── Dapper/                         # Data access layer
│   ├── WindNight.Extension.Dapper/
│   ├── WindNight.Extension.Dapper.Mssql/
│   └── WindNight.Extension.Dapper.Mysql/
├── WindNight.RabbitMq/             # Messaging
├── Extensions/                     # Additional extensions
├── Examples/                       # Demo applications
└── Tests/                          # Unit and integration tests
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Build and Test

```bash
# Build entire solution
dotnet build src/WindNight.NetFx.sln

# Run tests before submitting
dotnet test src/WindNight.NetFx.sln

# Generate packages (Release mode)
dotnet build src/WindNight.NetFx.sln --configuration Release
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- **Repository**: https://github.com/windNight/WindNight.NetFx.git
- **NuGet Packages**: Search "WindNight" on [nuget.org](https://www.nuget.org/)
- **Issues**: [GitHub Issues](https://github.com/windNight/WindNight.NetFx/issues)

## 💡 Support

If you find this project helpful, please consider:
- ⭐ Starring the repository
- 🐛 Reporting issues
- 🛠️ Contributing improvements
- 📖 Improving documentation

---

Built with ❤️ by windnight
