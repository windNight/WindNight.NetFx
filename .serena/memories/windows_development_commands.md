# Windows Development Commands for WindNight.NetFx

## File System Operations
```cmd
# List files and directories
dir
dir /s *.cs          # Find all .cs files recursively
dir /s *.csproj      # Find all project files

# Navigate directories
cd src\Examples\Net8ApiDemo
cd ..\..             # Go up directories
cd /d D:\Github\WindNight\WindNight.NetFx  # Change drive and directory

# Create directories
mkdir new-folder

# Copy files
copy source.txt destination.txt
xcopy /s source-folder destination-folder  # Recursive copy
```

## Text Search
```cmd
# Find text in files (Windows equivalent of grep)
findstr /s /i "ProgramBase" *.cs
findstr /s "namespace WindNight" *.cs
findstr /r /s "class.*Base" *.cs     # Regex search
```

## Git Commands
```bash
git status
git add .
git commit -m "commit message"
git push
git pull
git branch
git checkout -b new-branch
git merge branch-name
```

## .NET CLI Commands  
```bash
# Build and test
dotnet build
dotnet test
dotnet run
dotnet clean
dotnet restore

# Package management
dotnet add package PackageName
dotnet remove package PackageName
dotnet list package

# Project management
dotnet new console -n ProjectName
dotnet add reference ../OtherProject/OtherProject.csproj
```

## PowerShell Alternatives
```powershell
# More powerful alternatives to cmd
Get-ChildItem -Recurse -Include "*.cs"     # Find files
Select-String -Pattern "ProgramBase" -Path "*.cs" -Recurse   # Text search
Test-Path "file.txt"                       # Check if file exists
```

## Process Management
```cmd
# List running processes
tasklist
tasklist | findstr "dotnet"

# Kill process
taskkill /PID processid
taskkill /IM "dotnet.exe" /F
```