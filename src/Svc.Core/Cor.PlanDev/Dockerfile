FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Packages.props .
COPY Directory.Build.props .

COPY . .

RUN dotnet restore "src/Svc.Core/Cor.PlanDev/Cor.PlanDev.csproj"

RUN dotnet publish "src/Svc.Core/Cor.PlanDev/Cor.PlanDev.csproj" -c Release -o /app/publish --no-restore 
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Cor.PlanDev.dll"]



