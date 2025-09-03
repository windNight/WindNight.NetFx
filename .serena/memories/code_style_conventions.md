# WindNight.NetFx Code Style and Conventions

## EditorConfig Settings
The project uses comprehensive EditorConfig rules defined in `src/.editorconfig`:

### General Formatting
- **Indentation**: 4 spaces (no tabs)
- **Encoding**: UTF-8
- **Line endings**: LF (Unix style)
- **Final newline**: Required
- **Trailing whitespace**: Trimmed

### C# Code Style
- **Braces**: Allman style (new line before open brace)
- **var usage**: Preferred for built-in types
- **Access modifiers**: Required for non-interface members
- **this qualification**: Not required (disabled)

### Naming Conventions
- **Interfaces**: Must start with `I` (IPascalCase)
- **Types**: PascalCase (classes, structs, enums)
- **Members**: PascalCase (properties, methods, events)
- **Namespaces**: Follow `WindNight.[Module].[SubModule]` pattern

## Language Features
- **C# Language Version**: preview
- **Nullable Reference Types**: Enabled
- **ImplicitUsings**: Enabled
- **Unsafe Blocks**: Allowed for performance-critical code

## Project Conventions
- **Multi-targeting**: Support netstandard2.0/2.1, net8.0, net9.0
- **Conditional Compilation**: Uses symbols like `__CORE__`, `NET80`, `STD20`
- **XML Documentation**: Generated for all public APIs
- **Package Generation**: Automatic on Release builds

## Architecture Patterns
- **ProgramBase Pattern**: Applications inherit from base classes for standardized setup
- **Partial Classes**: Used extensively for organizing large classes
- **Internal namespaces**: Use `@internal` suffix for internal components
- **Extension Classes**: Static classes for extending existing types

## Suppressed Warnings
The project suppresses many warnings including:
- CA2007 (ConfigureAwait)
- Nullable reference warnings (CS8600 series)
- Documentation warnings (CS1591, CS1572-1574)
- Obsolete API warnings (CS0618)