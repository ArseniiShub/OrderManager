FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5002
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderManager.ReadModel.Api/OrderManager.ReadModel.Api.csproj", "OrderManager.ReadModel.Api/"]
COPY ["OrderManager.Shared/OrderManager.Shared.csproj", "OrderManager.Shared/"]
RUN dotnet restore "OrderManager.ReadModel.Api/OrderManager.ReadModel.Api.csproj"
COPY . .
WORKDIR "/src/OrderManager.ReadModel.Api"
RUN dotnet build "./OrderManager.ReadModel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderManager.ReadModel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManager.ReadModel.Api.dll"]