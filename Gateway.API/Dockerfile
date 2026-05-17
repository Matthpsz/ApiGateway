# Estágio de Compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os arquivos de projeto primeiro para cache de camadas
COPY ["Gateway.API/Gateway.API.csproj", "Gateway.API/"]
COPY ["Gateway.Shared/Gateway.Shared.csproj", "Gateway.Shared/"]
RUN dotnet restore "Gateway.API/Gateway.API.csproj"

# Copiar o restante do código e compilar
COPY . .
WORKDIR "/src/Gateway.API"
RUN dotnet build "Gateway.API.csproj" -c Release -o /app/build

# Publicar a aplicação
FROM build AS publish
RUN dotnet publish "Gateway.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final de Execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Gateway.API.dll"]