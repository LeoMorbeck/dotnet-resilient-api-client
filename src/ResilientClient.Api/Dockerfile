# Dockerfile para a Api

# --- Estágio 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia o .csproj e restaura
COPY ["src/ResilientClient.Api/ResilientClient.Api.csproj", "ResilientClient.Api/"]
RUN dotnet restore "ResilientClient.Api/ResilientClient.Api.csproj"

# Copia o código fonte
COPY ./src/ResilientClient.Api/. ./ResilientClient.Api/
WORKDIR "/src/ResilientClient.Api"

# Publica
RUN dotnet publish -c Release -o /app/publish

# --- Estágio 2: Imagem Final ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ResilientClient.Api.dll"]