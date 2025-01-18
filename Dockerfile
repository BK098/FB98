FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Bootstrapper/FB98.Bootstrapper/FB98.Bootstrapper.csproj", "src/Bootstrapper/FB98.Bootstrapper/"]
COPY ["src/Shared/FB98.Shared.Abstractions/FB98.Shared.Abstractions.csproj", "src/Shared/FB98.Shared.Abstractions/"]
COPY ["src/Shared/FB98.Shared.Infrastructure/FB98.Shared.Infrastructure.csproj", "src/Shared/FB98.Shared.Infrastructure/"]
COPY ["src/Modules/Identity/FB98.Modules.Identity.Api/FB98.Modules.Identity.Api.csproj", "src/Modules/Identity/FB98.Modules.Identity.Api/"]
COPY ["src/Modules/Identity/FB98.Modules.Identity.Application/FB98.Modules.Identity.Application.csproj", "src/Modules/Identity/FB98.Modules.Identity.Application/"]

RUN dotnet restore "src/Bootstrapper/FB98.Bootstrapper/FB98.Bootstrapper.csproj"

COPY . .
WORKDIR "/src/src/Bootstrapper/FB98.Bootstrapper"
RUN dotnet build "FB98.Bootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FB98.Bootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

USER root

COPY --from=publish /app/publish .

RUN mkdir -p /app/Certificates
COPY ["src/Bootstrapper/FB98.Bootstrapper/Certificates/aspnetapp.pfx", "/app/Certificates/aspnetapp.pfx"]
RUN chmod 644 /app/Certificates/aspnetapp.pfx

USER app

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/Certificates/aspnetapp.pfx

#ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourPassword

ENTRYPOINT ["dotnet", "FB98.Bootstrapper.dll"]