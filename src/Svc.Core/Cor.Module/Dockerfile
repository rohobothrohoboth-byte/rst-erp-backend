FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY src/Svc.Core/Cor.Module/*.csproj src/Svc.Core/Cor.Module/
COPY src/Shared/Helpers/*.csproj src/Shared/Helpers/
COPY src/gRPC/Common/*.csproj src/gRPC/Common/
COPY src/gRPC/Contracts/*.csproj src/gRPC/Contracts/

RUN dotnet restore src/Svc.Core/Cor.Module/Cor.Module.csproj

COPY . .
WORKDIR /src/src/Svc.Core/Cor.Module
RUN dotnet build "Cor.Module.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cor.Module.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Cor.Module.dll"]