FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Central package management
COPY Directory.Packages.props .
COPY Directory.Build.props .

# Project files

COPY src/Svc.Finance/Cor.Finance/*.csproj src/Svc.Finance/Cor.Finance/
COPY src/gRPC/Common/*.csproj src/gRPC/Common/
COPY src/gRPC/Contracts/*.csproj src/gRPC/Contracts/
COPY src/RST_ERP.ServiceDefaults/*.csproj src/RST_ERP.ServiceDefaults/
COPY src/Shared/Helpers/*.csproj src/Shared/Helpers/

# Restore
RUN dotnet restore "src/Svc.Finance/Cor.Finance/Cor.Finance.csproj"

# Source
COPY . .

# Build/publish using the already-restored packages
RUN dotnet publish "src/Svc.Finance/Cor.Finance/Cor.Finance.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Cor.Finance.dll"]
