# WindNight.NetFx Task Completion Guidelines

## After Code Changes

### 1. Build Verification
Always build the solution to ensure no compilation errors:
```bash
dotnet build src/WindNight.NetFx.sln
```

### 2. Run Tests
Execute relevant tests based on changes made:
```bash
# For Core changes
dotnet test src/Tests/WindNight.Core.Tests/

# For Extension changes  
dotnet test src/Tests/WindNight.Extension.Tests/

# For Web API changes
dotnet test src/Tests/WebApiTest/

# Run all tests
dotnet test src/WindNight.NetFx.sln
```

### 3. Integration Testing
Test with example applications when making significant changes:
```bash
# Test with API demo
dotnet run --project src/Examples/Net8ApiDemo/

# Test with console examples
dotnet run --project src/Examples/ConsoleExamples_NetCore5/
```

### 4. Package Generation (for releases)
For release-ready changes:
```bash
dotnet build src/WindNight.NetFx.sln --configuration Release
```
This generates NuGet packages in `Output/Release/`

## Code Quality Checks

### Static Analysis
The project has extensive warning suppressions configured, but still validate:
- No new compiler errors
- Consistent with existing code patterns
- Proper XML documentation for public APIs

### Documentation
- Update XML comments for public APIs
- Follow existing documentation patterns
- Ensure IntelliSense information is complete

## Deployment Considerations

### Multi-Framework Support
Ensure changes work across all target frameworks:
- netstandard2.0, netstandard2.1 
- net8.0, net9.0

### Conditional Compilation
Use appropriate compiler directives when needed:
- `__CORE__` for .NET Core
- `NET80`, `NET90` for specific versions
- `STD20`, `STD21` for .NET Standard

## No Lint/Format Commands
The project relies on EditorConfig and Visual Studio for formatting. No specific lint or format commands are required as the build process handles code style validation.