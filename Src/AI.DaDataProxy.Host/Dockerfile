FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/AI.DaDataProxy.Host/AI.DaDataProxy.Host.csproj", "Src/AI.DaDataProxy.Host/"]
COPY ["Src/AI.DaDataProxy/AI.DaDataProxy.csproj", "Src/AI.DaDataProxy/"]
COPY ["Src/AI.DaDataProxy.Http.Contracts/AI.DaDataProxy.Http.Contracts.csproj", "Src/AI.DaDataProxy.Http.Contracts/"]
COPY ["Src/AI.DaDataProxy.Entities/AI.DaDataProxy.Entities.csproj", "Src/AI.DaDataProxy.Entities/"]
RUN dotnet restore "Src/AI.DaDataProxy.Host/AI.DaDataProxy.Host.csproj"
COPY . .
WORKDIR "/src/Src/AI.DaDataProxy.Host"
RUN dotnet build "AI.DaDataProxy.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AI.DaDataProxy.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.DaDataProxy.Host.dll"]
