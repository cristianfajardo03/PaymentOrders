# Migraciones EF Core

La API usa SQLite en el archivo local `payment-orders.local.db` y aplica sus migraciones EF Core al iniciar. También puedes ejecutarlas manualmente desde la raíz del repositorio:

```powershell
dotnet tool restore
dotnet ef database update --project src/PaymentOrders.Infrastructure/PaymentOrders.Infrastructure.csproj --startup-project src/PaymentOrders.Api/PaymentOrders.Api.csproj
```

Para MySQL, configura `Database__Provider=MySql` y `ConnectionStrings__PaymentOrders`, y usa el proyecto de migraciones MySQL:

```powershell
dotnet ef database update --project src/PaymentOrders.Infrastructure.MySqlMigrations/PaymentOrders.Infrastructure.MySqlMigrations.csproj --startup-project src/PaymentOrders.Api/PaymentOrders.Api.csproj
```
