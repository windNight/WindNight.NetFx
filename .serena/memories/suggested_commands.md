# WindNight.NetFx Development Commands

## Build Commands
```bash
# Build entire solution
dotnet build src/WindNight.NetFx.sln

# Build in Release mode (generates NuGet packages)
dotnet build src/WindNight.NetFx.sln --configuration Release

# Clean build
dotnet clean src/WindNight.NetFx.sln
```

## Test Commands
```bash
# Run Core tests
dotnet test src/Tests/WindNight.Core.Tests/

# Run Extension tests  
dotnet test src/Tests/WindNight.Extension.Tests/

# Run Web API integration tests
dotnet test src/Tests/WebApiTest/

# Run all tests
dotnet test src/WindNight.NetFx.sln
```

## Running Examples
```bash
# Run Net8 API Demo
dotnet run --project src/Examples/Net8ApiDemo/

# Run Console Examples (NetCore 5)
dotnet run --project src/Examples/ConsoleExamples_NetCore5/

# Run gRPC Demo
dotnet run --project src/Examples/GrpcDemo_Net5/

# Run RabbitMQ Demo
dotnet run --project src/Examples/RabbitMqDemos/

# Run MVC Demo
dotnet run --project src/Examples/MvcDemo_Net5/
```

## Package Management
```bash
# Restore packages
dotnet restore src/WindNight.NetFx.sln

# List package references
dotnet list src/WindNight.Core/WindNight.Core.csproj package
```

## Windows Utility Commands
```cmd
# List directories
dir

# Find files
dir /s *.cs

# Navigate directories
cd src\Examples\Net8ApiDemo

# Git operations
git status
git add .
git commit -m "message"
git push
```

## Development Workflow
1. Make changes to code
2. Build: `dotnet build src/WindNight.NetFx.sln`
3. Test: `dotnet test src/WindNight.NetFx.sln`
4. For releases: Build with `--configuration Release` to generate packages