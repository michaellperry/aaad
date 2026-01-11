# Dockerfile (API)

Multi-stage build for .NET API with non-root runtime and health check.

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/GloboTicket.API/GloboTicket.API.csproj", "GloboTicket.API/"]
COPY ["src/GloboTicket.Application/GloboTicket.Application.csproj", "GloboTicket.Application/"]
COPY ["src/GloboTicket.Domain/GloboTicket.Domain.csproj", "GloboTicket.Domain/"]
COPY ["src/GloboTicket.Infrastructure/GloboTicket.Infrastructure.csproj", "GloboTicket.Infrastructure/"]

RUN dotnet restore "GloboTicket.API/GloboTicket.API.csproj"

# Copy source code and build
COPY src/ .
RUN dotnet build "GloboTicket.API/GloboTicket.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GloboTicket.API/GloboTicket.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN groupadd -r globoticket && useradd -r -g globoticket globoticket
USER globoticket

COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080
ENTRYPOINT ["dotnet", "GloboTicket.API.dll"]
```
