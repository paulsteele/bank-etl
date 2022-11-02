FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN ./build.sh compile

FROM base AS final
WORKDIR /app
COPY --from=build /etl/bin/Debug/net6.0 .
ENTRYPOINT ["dotnet", "etl.dll"]
