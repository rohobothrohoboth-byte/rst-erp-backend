FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.soln .
COPY src/Svc.HRM/Profile.API/*.csproj src/Svc.HRM/Profile.API/
COPY src/Shared/Helpers/*.csproj src/Shared/Helpers/
COPY src/gRPC/Common/*.csproj src/gRPC/Common/
COPY src/gRPC/Contracts/*.csproj src/gRPC/Contracts/

RUN dotnet restore src/Svc.HRM/Profile.API/Profile.API.csproj

COPY . .
WORKDIR /src/src/Svc.HRM/Profile.API
RUN dotnet build "Profile.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Profile.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Profile.API.dll"]