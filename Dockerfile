FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src
COPY Settlement.ExternalApi.sln .
COPY ./src/Settlement.External/Settlement.External.csproj ./Settlement.External/
COPY ./src/Settlement.ExternalApi/Settlement.ExternalApi.csproj ./Settlement.ExternalApi/


WORKDIR /src/Settlement.ExternalApi
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -v n

COPY . .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

ENV COMPlus_EnableDiagnostics=0
WORKDIR /app
COPY --from=builder /app .


# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

ENTRYPOINT ["dotnet", "Settlement.ExternalApi.dll"]