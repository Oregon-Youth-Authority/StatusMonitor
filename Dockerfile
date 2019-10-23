FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["StatusMonitor.csproj", ""]
RUN dotnet restore "./StatusMonitor.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "StatusMonitor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StatusMonitor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet StatusMonitor.dll