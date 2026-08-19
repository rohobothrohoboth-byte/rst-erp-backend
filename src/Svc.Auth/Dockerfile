FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Packages.props .
COPY Directory.Build.props .

COPY *.sln .
COPY Directory.Build.props .
COPY Directory.Packages.props .

COPY src/Svc.Auth/*.csproj src/Svc.Auth/
COPY src/RST_ERP.ServiceDefaults/*.csproj src/RST_ERP.ServiceDefaults/
COPY src/Shared/Helpers/*.csproj src/Shared/Helpers/
COPY src/gRPC/Common/*.csproj src/gRPC/Common/
COPY src/gRPC/Contracts/*.csproj src/gRPC/Contracts/

RUN dotnet restore src/Svc.Auth/Svc.Auth.csproj

COPY . .

WORKDIR /src/src/Svc.Auth
RUN dotnet build "Svc.Auth.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Svc.Auth.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Svc.Auth.dll"]