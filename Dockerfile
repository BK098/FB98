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
COPY ["src/Shared/FB98.Shared.Utils/FB98.Shared.Utils.csproj", "src/Shared/FB98.Shared.Utils/"]

#SystemsModule
COPY ["src/Modules/Systems/FB98.Modules.Systems.Api/FB98.Modules.Systems.Api.csproj", "src/Modules/Systems/FB98.Module.Systems.Api/"]

#IdentityModule
COPY ["src/Modules/Identity/FB98.Modules.Identity.Api/FB98.Modules.Identity.Api.csproj", "src/Modules/Identity/FB98.Modules.Identity.Api/"]
COPY ["src/Modules/Identity/FB98.Modules.Identity.Application/FB98.Modules.Identity.Application.csproj", "src/Modules/Identity/FB98.Modules.Identity.Application/"]
COPY ["src/Modules/Identity/FB98.Modules.Identity.DataAccess/FB98.Modules.Identity.DataAccess.csproj", "src/Modules/Identity/FB98.Modules.Identity.DataAccess/"]
COPY ["src/Modules/Identity/FB98.Modules.Identity.Domain/FB98.Modules.Identity.Domain.csproj", "src/Modules/Identity/FB98.Modules.Identity.Domain/"]

# CatalogModule
COPY ["src/Modules/Catalog/FB98.Modules.Catalog.Api/FB98.Modules.Catalog.Api.csproj", "src/Modules/Catalog/FB98.Modules.Catalog.Api/"]
COPY ["src/Modules/Catalog/FB98.Modules.Catalog.Application/FB98.Modules.Catalog.Application.csproj", "src/Modules/Catalog/FB98.Modules.Catalog.Application/"]
COPY ["src/Modules/Catalog/FB98.Modules.Catalog.DataAccess/FB98.Modules.Catalog.DataAccess.csproj", "src/Modules/Catalog/FB98.Modules.Catalog.DataAccess/"]
COPY ["src/Modules/Catalog/FB98.Modules.Catalog.Domain/FB98.Modules.Catalog.Domain.csproj", "src/Modules/Catalog/FB98.Modules.Catalog.Domain/"]

#WarehouseModule
COPY ["src/Modules/Warehouse/FB98.Modules.Warehouse.Api/FB98.Modules.Warehouse.Api.csproj", "src/Modules/Warehouse/FB98.Modules.Warehouse.Api/"]
COPY ["src/Modules/Warehouse/FB98.Modules.Warehouse.Application/FB98.Modules.Warehouse.Application.csproj", "src/Modules/Warehouse/FB98.Modules.Warehouse.Application/"]
COPY ["src/Modules/Warehouse/FB98.Modules.Warehouse.DataAccess/FB98.Modules.Warehouse.DataAccess.csproj", "src/Modules/Warehouse/FB98.Modules.Warehouse.DataAccess/"]
COPY ["src/Modules/Warehouse/FB98.Modules.Warehouse.Domain/FB98.Modules.Warehouse.Domain.csproj", "src/Modules/Warehouse/FB98.Modules.Warehouse.Domain/"]

#CustomerModule
COPY ["src/Modules/Customers/FB98.Modules.Customers.Api/FB98.Modules.Customers.Api.csproj", "src/Modules/Customers/FB98.Modules.Customers.Api/"]
COPY ["src/Modules/Customers/FB98.Modules.Customers.Application/FB98.Modules.Customers.Application.csproj", "src/Modules/Customers/FB98.Modules.Customers.Application/"]
COPY ["src/Modules/Customers/FB98.Modules.Customers.DataAccess/FB98.Modules.Customers.DataAccess.csproj", "src/Modules/Customers/FB98.Modules.Customers.DataAccess/"]
COPY ["src/Modules/Customers/FB98.Modules.Customers.Domain/FB98.Modules.Customers.Domain.csproj", "src/Modules/Customers/FB98.Modules.Customers.Domain/"]

#OrderModule
COPY ["src/Modules/Orders/FB98.Modules.Orders.Api/FB98.Modules.Orders.Api.csproj", "src/Modules/Orders/FB98.Modules.Orders.Api/"]
COPY ["src/Modules/Orders/FB98.Modules.Orders.Application/FB98.Modules.Orders.Application.csproj", "src/Modules/Orders/FB98.Modules.Orders.Application/"]
COPY ["src/Modules/Orders/FB98.Modules.Orders.DataAccess/FB98.Modules.Orders.DataAccess.csproj", "src/Modules/Orders/FB98.Modules.Orders.DataAccess/"]
COPY ["src/Modules/Orders/FB98.Modules.Orders.Domain/FB98.Modules.Orders.Domain.csproj", "src/Modules/Orders/FB98.Modules.Orders.Domain/"]

#PaymentModule
COPY ["src/Modules/Payments/FB98.Modules.Payments.Api/FB98.Modules.Payments.Api.csproj", "src/Modules/Payments/FB98.Modules.Payments.Api/"]
COPY ["src/Modules/Payments/FB98.Modules.Payments.Application/FB98.Modules.Payments.Application.csproj", "src/Modules/Payments/FB98.Modules.Payments.Application/"]
COPY ["src/Modules/Payments/FB98.Modules.Payments.DataAccess/FB98.Modules.Payments.DataAccess.csproj", "src/Modules/Payments/FB98.Modules.Payments.DataAccess/"]
COPY ["src/Modules/Payments/FB98.Modules.Payments.Domain/FB98.Modules.Payments.Domain.csproj", "src/Modules/Payments/FB98.Modules.Payments.Domain/"]

#ShoppingListModule
COPY ["src/Modules/ShoppingList/FB98.Modules.ShoppingList.Api/FB98.Modules.ShoppingList.Api.csproj", "src/Modules/ShoppingList/FB98.Modules.ShoppingList.Api/"]
COPY ["src/Modules/ShoppingList/FB98.Modules.ShoppingList.Application/FB98.Modules.ShoppingList.Application.csproj", "src/Modules/ShoppingList/FB98.Modules.ShoppingList.Application/"]
COPY ["src/Modules/ShoppingList/FB98.Modules.ShoppingList.DataAccess/FB98.Modules.ShoppingList.DataAccess.csproj", "src/Modules/ShoppingList/FB98.Modules.ShoppingList.DataAccess/"]
COPY ["src/Modules/ShoppingList/FB98.Modules.ShoppingList.Domain/FB98.Modules.ShoppingList.Domain.csproj", "src/Modules/ShoppingList/FB98.Modules.ShoppingList.Domain/"]

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

USER app

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS="http://+:5000"

ENTRYPOINT ["dotnet", "FB98.Bootstrapper.dll"]