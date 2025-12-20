# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore MarketplaceSolution.sln
RUN dotnet publish Marketplace.API/Marketplace.API.csproj -c Release -o /app/publish

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 🔥 INSTALAR DEPENDENCIAS NECESARIAS PARA NPGSQL
RUN apt-get update && apt-get install -y \
    libkrb5-3 \
    libssl3 \
    libicu-dev \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Marketplace.API.dll"]
