FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY src/Svc.Auth/*.csproj src/Svc.Auth/
COPY src/Shared/Helpers/*.csproj src/Shared/Helpers/
COPY src/gRPC/Common/*.csproj src/gRPC/Common/
COPY src/gRPC/Contracts/*.csproj src/gRPC/Contracts/

# Restore dependencies
RUN dotnet restore src/Svc.Auth/Svc.Auth.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/src/Svc.Auth
RUN dotnet build "Svc.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Svc.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Svc.Auth.dll"]