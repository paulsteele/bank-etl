FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["firefly-iii-auto-connector.csproj", "./"]
RUN dotnet restore "firefly-iii-auto-connector.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "firefly-iii-auto-connector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "firefly-iii-auto-connector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "firefly-iii-auto-connector.dll"]
