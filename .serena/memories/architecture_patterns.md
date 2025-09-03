# WindNight.NetFx Architecture Patterns

## Core Design Patterns

### ProgramBase Pattern
Applications use standardized host building through `ProgramBase.CreateHostBuilderDefaults()`:
- Centralized configuration setup
- Build type detection (Debug/Release)
- Service registration delegates
- Exception handling setup
- Configuration builder customization

### Configuration System
- **ConfigCenterContext**: Central configuration management
- **ConfigItems**: Strongly-typed configuration classes
- **Multiple Sources**: JSON files, environment variables, remote config centers
- **Hot Reload**: Configuration changes detected and applied at runtime

### Logging System
- **LogHelper**: Custom logging facade with console output
- **DefaultLogHelperBase**: Base class for logging implementations
- **ILogService**: Abstraction for logging services
- **Multiple Providers**: Console, log4net, database, file storage

### Repository Pattern
Base repository classes for data access:
- **IBaseRepo**, **IReadRepoBase**, **IWriteRepoBase**: Core abstractions
- **MySqlBase**, **SqlServerBase**: Database-specific implementations
- **Entity abstractions**: `IEntity`, `EntityBase` for domain objects
- **Pagination**: Built-in support through `IQueryPageBase`

### Dependency Injection
- **Ioc**: Custom IoC wrapper around Microsoft.Extensions.DI
- **Service Registration**: Through extension methods and delegates
- **Scoped Services**: Support for request-scoped services

### Background Services
- **BaseConsumerBackgroundService**: For message consumers
- **BaseProducerBackgroundService**: For message producers  
- **SvrMonitorBackgroundService**: System monitoring services

## Key Abstractions

### Entity Framework
- **IEntity**: Base entity interface with ID support
- **ITreeObject**: Support for hierarchical data structures
- **DataStatusEnums**: Standard data lifecycle states

### Query Framework  
- **IQueryBase**: Base query interfaces
- **DefaultQueryBase**: Standard query implementations
- **IQueryPageBase**: Pagination support

### Monitoring System
- **ISvrMonitorInfo**: Server monitoring abstractions
- **SvrBuildInfo**: Build information tracking
- **ISvrRegisterInfo**: Service registration information

## Extension Patterns
- Static extension classes for existing types
- Partial classes for organizing large implementations
- Fluent API patterns for configuration
- Builder patterns for complex object creation

## Error Handling
- **BusinessException**: Domain-specific exceptions
- **Global Exception Handlers**: Centralized error handling
- **Retry Mechanisms**: Built-in retry functionality