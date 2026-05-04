# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WifiCdmx.sln .
COPY WifiCdmx.API/WifiCdmx.API.csproj WifiCdmx.API/
COPY WifiCdmx.Application/WifiCdmx.Application.csproj WifiCdmx.Application/
COPY WifiCdmx.Domain/WifiCdmx.Domain.csproj WifiCdmx.Domain/
COPY WifiCdmx.Infrastructure/WifiCdmx.Infrastructure.csproj WifiCdmx.Infrastructure/

RUN dotnet restore WifiCdmx.API/WifiCdmx.API.csproj

COPY . .

RUN dotnet publish WifiCdmx.API/WifiCdmx.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Use non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

RUN mkdir -p /app/Data

EXPOSE 8080

ENTRYPOINT ["dotnet", "WifiCdmx.API.dll"]
