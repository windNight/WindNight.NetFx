#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
ENV TZ Asia/Shanghai
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Examples/MvcDemo_Net5/MvcDemo_Net5.csproj", "Examples/MvcDemo_Net5/"]
RUN dotnet restore "Examples/MvcDemo_Net5/MvcDemo_Net5.csproj"
COPY . .
WORKDIR "/src/Examples/MvcDemo_Net5"
RUN dotnet build "MvcDemo_Net5.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MvcDemo_Net5.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MvcDemo_Net5.dll"]