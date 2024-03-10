FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8000
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LotusWebApp.Service/LotusWebApp.Service.csproj", "LotusWebApp.Service/"]
RUN dotnet restore "LotusWebApp.Service/LotusWebApp.Service.csproj"
COPY . .
WORKDIR "/src/LotusWebApp.Service"
RUN dotnet build "LotusWebApp.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LotusWebApp.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LotusWebApp.Service.dll"]