# WindNight.NetFx Codebase Structure

## Solution Structure
Main solution: `src/WindNight.NetFx.sln`

## Core Libraries
- **WindNight.Core**: Base abstractions, core extensions, business exceptions
- **WindNight.Extension**: Utilities, HTTP helpers, logging helpers
- **WindNight.Config**: Configuration management system with ConfigCenterContext
- **WindNight.Config.Extensions**: Configuration extensions and API controllers

## ASP.NET Core Components
- **WindNight.AspNetCore.Hosting**: Web hosting with ProgramBase pattern
- **WindNight.AspNetCore.Mvc.Extensions**: MVC extensions, filters, attributes
- **WindNight.AspNetCore.GRpc.Hosting**: gRPC hosting infrastructure

## Data Access Layer
- **WindNight.Extension.Dapper**: Base Dapper extensions
- **WindNight.Extension.Dapper.Mssql**: SQL Server implementations
- **WindNight.Extension.Dapper.Mysql**: MySQL implementations  
- **WindNight.Extension.Db.Abstractions**: Database abstractions and interfaces

## Infrastructure
- **WindNight.RabbitMq**: RabbitMQ producer/consumer implementations
- **WindNight.Extension.DbLog.Mysql**: Database logging provider
- **WindNight.Extension.LogStore**: Log storage abstractions
- **Schedule**: Job scheduling with Quartz integration

## Extensions and Tools
- **extensions/WindNight.AspNetCore.GRpc.HttpApi**: gRPC HTTP API bridge
- **extensions/WindNight.AspNetCore.GRpc.Swagger**: gRPC Swagger integration
- **extensions/Swashbuckle.AspNetCore.HideApi**: Swagger API hiding functionality
- **WindNight.DataSourceTestTool**: WinForms testing tool for data sources

## Examples and Demos
Multiple example projects in `src/Examples/`:
- Net8ApiDemo, GrpcDemo_Net5, MvcDemo_Net5
- RabbitMqDemos, JobDemos, Logger.Demo
- ConsoleExamples for different .NET versions

## Test Projects
- WindNight.Core.Tests
- WindNight.Extension.Tests  
- WebApiTest (integration tests)