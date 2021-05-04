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
