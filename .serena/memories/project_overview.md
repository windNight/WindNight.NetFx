# WindNight.NetFx Project Overview

## Project Purpose
WindNight.NetFx is a comprehensive .NET library ecosystem that provides foundational components for building scalable applications. It serves as a modular architecture framework with multiple NuGet packages covering different concerns like configuration management, data access, logging, messaging, and web hosting.

## Tech Stack
- **.NET Multi-targeting**: netstandard2.0, netstandard2.1, net8.0, net9.0
- **Web Framework**: ASP.NET Core
- **ORM**: Dapper with custom extensions
- **Databases**: MySQL and MSSQL support
- **Messaging**: RabbitMQ integration
- **Logging**: log4net, Microsoft.Extensions.Logging
- **gRPC**: Complete gRPC hosting with HTTP API bridge
- **Testing**: xunit framework
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Configuration**: Microsoft.Extensions.Configuration
- **HTTP Client**: RestSharp for HTTP communications
- **JSON**: Newtonsoft.Json and System.Text.Json

## Key Technologies Used
- C# with preview language features
- Nullable reference types enabled
- ImplicitUsings enabled 
- Unsafe blocks allowed for performance-critical code
- MSBuild custom targets for build info generation
- NuGet package generation on Release builds