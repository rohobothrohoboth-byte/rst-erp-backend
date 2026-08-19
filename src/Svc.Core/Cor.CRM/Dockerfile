FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution-level files
COPY Directory.Packages.props .
COPY Directory.Build.props .

# Copy project files for CRM and its dependencies
COPY src/Svc.Core/Cor.CRM/*.csproj src/Svc.Core/Cor.CRM/
COPY src/Shared/Helpers/*.csproj src/Shared/Helpers/
COPY src/RST_ERP.ServiceDefaults/*.csproj src/RST_ERP.ServiceDefaults/
COPY src/gRPC/Common/*.csproj src/gRPC/Common/
COPY src/gRPC/Contracts/*.csproj src/gRPC/Contracts/

# Restore
RUN dotnet restore "src/Svc.Core/Cor.CRM/Cor.CRM.csproj"

# Copy everything else
COPY . .

# Publish
RUN dotnet publish "src/Svc.Core/Cor.CRM/Cor.CRM.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Cor.CRM.dll"]