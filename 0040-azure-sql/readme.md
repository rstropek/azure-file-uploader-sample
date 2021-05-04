# Azure SQL Database

## Theory and Concepts

* General introduction into different SQL offerings on Azure
  * PaaS vs. IaaS vs. Serverless
  * Azure SQL Managed Instances
  * Azure SQL DB
  * Other SQL offerings (e.g. MySQL, PostgreSQL)
  * [Elastic DB pools introduction](https://docs.microsoft.com/en-us/azure/azure-sql/database/elastic-pool-overview)
  * [Pricing](https://azure.microsoft.com/en-us/pricing/details/sql-database/single/)
  * [Differences to regular SQL Server](https://docs.microsoft.com/en-us/azure/azure-sql/database/transact-sql-tsql-differences-sql-server)
* [Azure Data Studio introduction](https://docs.microsoft.com/en-us/sql/azure-data-studio/?view=sql-server-ver15)
* Security
  * AAD integration

## Out of scope

* General introduction in SQL (assumption: people are familiar with it)

## Exercise

* Create SQL Server and SQL DB with ARM Template + Azure CLI
* Access DB with Azure Data Studio and T-SQL
* Implement a simple "Hello Database" console app with Entity Framework Core 5
  * Used to introduce some fundamentals about EF Core 5 (e.g. model, data context, app settings, migrations)

## Helper scripts

### Connection Strings

* LocalDB: `Server=(localdb)\\MSSQLLocalDB;Database=CsvImport;Trusted_Connection=True`
* Azure Managed Identity: `Server=sql-67zwxeawqbvie.database.windows.net; Authentication=Active Directory MSI; Initial Catalog=sqldb-67zwxeawqbvie;`

### Migrations Script

Generated with `dotnet ef migrations script`

```sql
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Customers] (
    [ID] int NOT NULL IDENTITY,
    [FirstName] nvarchar(100) NULL,
    [LastName] nvarchar(100) NULL,
    [Email] nvarchar(150) NOT NULL,
    [Gender] nvarchar(50) NULL,
    [IpAddress] nvarchar(15) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([ID])
);
GO

CREATE UNIQUE INDEX [IX_Customers_Email] ON [Customers] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210503083850_Customers', N'5.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [CustomersStaging] (
    [ID] int NOT NULL IDENTITY,
    [FirstName] nvarchar(100) NULL,
    [LastName] nvarchar(100) NULL,
    [Email] nvarchar(150) NOT NULL,
    [Gender] nvarchar(50) NULL,
    [IpAddress] nvarchar(15) NULL,
    CONSTRAINT [PK_CustomersStaging] PRIMARY KEY ([ID])
);
GO

CREATE UNIQUE INDEX [IX_CustomersStaging_Email] ON [CustomersStaging] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210503092054_CustomersStaging', N'5.0.5');
GO

COMMIT;
GO
```

### Create external user (app) in SQL

```sql
CREATE USER [web-test-67zwxeawqbvie] FROM EXTERNAL PROVIDER;
ALTER ROLE db_owner ADD MEMBER [web-test-67zwxeawqbvie];
```
