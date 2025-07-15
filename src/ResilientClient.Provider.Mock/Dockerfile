# Dockerfile para o Provider.Mock

# --- Estágio 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o .csproj e restaura
COPY ["src/ResilientClient.Provider.Mock/ResilientClient.Provider.Mock.csproj", "ResilientClient.Provider.Mock/"]
RUN dotnet restore "ResilientClient.Provider.Mock/ResilientClient.Provider.Mock.csproj"

# Copia o código fonte
COPY ./src/ResilientClient.Provider.Mock/. ./ResilientClient.Provider.Mock/
WORKDIR "/src/ResilientClient.Provider.Mock"

# Publica
RUN dotnet publish -c Release -o /app/publish

# --- Estágio 2: Imagem Final ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ResilientClient.Provider.Mock.dll"]