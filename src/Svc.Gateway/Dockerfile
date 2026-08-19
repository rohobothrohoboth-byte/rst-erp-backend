FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY Directory.Packages.props .
COPY Directory.Build.props .

COPY src/Svc.Gateway/*.csproj src/Svc.Gateway/
COPY src/RST_ERP.ServiceDefaults/*.csproj src/RST_ERP.ServiceDefaults/
COPY src/Svc.Notification/*.csproj src/Svc.Notification/
COPY src/Svc.Task/*.csproj src/Svc.Task/
COPY src/Shared/Helpers/*.csproj src/Shared/Helpers/

# Restore all projects with cache mount
RUN dotnet restore src/Svc.Gateway/Svc.Gateway.csproj

# Copy everything else
COPY . .

# Build
WORKDIR /src/src/Svc.Gateway
RUN dotnet build "Svc.Gateway.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Svc.Gateway.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Svc.Gateway.dll"]
