FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution-level files
COPY Directory.Packages.props .
COPY Directory.Build.props .

# Copy all project files
COPY src/Svc.HRM/Svc.HRM.Attendance/*.csproj src/Svc.HRM/Svc.HRM.Attendance/
COPY src/Shared/Helpers/*.csproj src/Shared/Helpers/
COPY src/RST_ERP.ServiceDefaults/*.csproj src/RST_ERP.ServiceDefaults/
COPY src/gRPC/Common/*.csproj src/gRPC/Common/
COPY src/gRPC/Contracts/*.csproj src/gRPC/Contracts/

# Restore with cache mount
RUN dotnet restore "src/Svc.HRM/Svc.HRM.Attendance/Svc.HRM.Attendance.csproj"

# Copy everything else
COPY . .

# Publish
RUN dotnet publish "src/Svc.HRM/Svc.HRM.Attendance/Svc.HRM.Attendance.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Svc.HRM.Attendance.dll"]
