# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/CoffeeCodex.API/CoffeeCodex.API.csproj", "src/CoffeeCodex.API/"]
COPY ["src/CoffeeCodex.Application/CoffeeCodex.Application.csproj", "src/CoffeeCodex.Application/"]
COPY ["src/CoffeeCodex.Infrastructure/CoffeeCodex.Infrastructure.csproj", "src/CoffeeCodex.Infrastructure/"]
COPY ["src/CoffeeCodex.Domain/CoffeeCodex.Domain.csproj", "src/CoffeeCodex.Domain/"]
COPY ["src/CoffeeCodex.Shared/CoffeeCodex.Shared.csproj", "src/CoffeeCodex.Shared/"]
RUN dotnet restore "src/CoffeeCodex.API/CoffeeCodex.API.csproj"

COPY . .
RUN dotnet publish "src/CoffeeCodex.API/CoffeeCodex.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CoffeeCodex.API.dll"]
