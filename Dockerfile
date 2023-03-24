FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN ./build.sh compile

FROM base AS final
WORKDIR /app
COPY --from=build /src/etl/bin/Debug/net7.0 .
ENTRYPOINT ["dotnet", "etl.dll"]
