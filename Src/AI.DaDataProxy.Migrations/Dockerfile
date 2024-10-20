FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/AI.DaDataProxy.Migrations/AI.DaDataProxy.Migrations.csproj", "Src/AI.DaDataProxy.Migrations/"]
COPY ["Src/AI.DaDataProxy.Entities/AI.DaDataProxy.Entities.csproj", "Src/AI.DaDataProxy.Entities/"]
RUN dotnet restore "Src/AI.DaDataProxy.Migrations/AI.DaDataProxy.Migrations.csproj"
COPY . .
WORKDIR "/src/Src/AI.DaDataProxy.Migrations"
RUN dotnet build "AI.DaDataProxy.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AI.DaDataProxy.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.DaDataProxy.Migrations.dll"]
