FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["StatusMonitor.Server/StatusMonitor.Server.csproj", "StatusMonitor.Server/"]
COPY ["StatusMonitor.ApiKey.Providers/StatusMonitor.ApiKey.Providers.csproj", "StatusMonitor.ApiKey.Providers/"]
RUN dotnet restore "StatusMonitor.ApiKey.Providers/StatusMonitor.ApiKey.Providers.csproj"
RUN dotnet restore "StatusMonitor.Server/StatusMonitor.Server.csproj"
COPY . .
WORKDIR "/src/StatusMonitor.Server"
RUN dotnet build "StatusMonitor.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StatusMonitor.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet StatusMonitor.Server.dll