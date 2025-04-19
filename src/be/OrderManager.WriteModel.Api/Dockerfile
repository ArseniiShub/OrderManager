FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderManager.WriteModel.Api/OrderManager.WriteModel.Api.csproj", "OrderManager.WriteModel.Api/"]
RUN dotnet restore "OrderManager.WriteModel.Api/OrderManager.WriteModel.Api.csproj"
COPY . .
WORKDIR "/src/OrderManager.WriteModel.Api"
RUN dotnet build "./OrderManager.WriteModel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderManager.WriteModel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManager.WriteModel.Api.dll"]
