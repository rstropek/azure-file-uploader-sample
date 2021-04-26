# Storage

## Prerequisites

* [Install *azurite*](https://github.com/Azure/Azurite#npm): `npm install -g azurite`
* [Start *azurite*](https://github.com/Azure/Azurite#npm): `azurite --silent --location <data-folder> --debug <log-folder>\debug.log`
* [Install *Azure Storage Explorer*](https://azure.microsoft.com/en-us/features/storage-explorer/)
* Install [.NET 5](https://dotnet.microsoft.com/) and latest version of [Visual Studio](https://visualstudio.microsoft.com/vs/)

## Theory and Concepts

* [Storage services on Azure](https://docs.microsoft.com/en-us/azure/?product=storage)
  * Blob Storage
  * Disk Storage
  * Queue Storage
  * Table Storage
  * Files
  * Relationship/differences to SQL and NoSQL offerings
  * Relationship with Azure CDN
  * When to use what?
  * [Storage explorer](https://docs.microsoft.com/en-us/azure/vs-azure-tools-storage-manage-with-storage-explorer)
* Storage emulator
  * [Azurite](https://github.com/azure/azurite)
* [Blob storage](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-overview)
  * [Accounts, containers, and blobs](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction)
  * [Pricing](https://azure.microsoft.com/en-us/pricing/details/storage/)
  * HTTP access
  * [Access levels](https://docs.microsoft.com/en-us/azure/storage/blobs/anonymous-read-access-configure)
  * Authorization (AAD, SAS)
  * [Data protection mechanisms](https://docs.microsoft.com/en-us/azure/storage/blobs/data-protection-overview)
  * [Access tiers](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-storage-tiers)
  * [Blob Storage event handling](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-event-overview)

## Out of scope

* Storage services other than Blob Storage
* NoSQL offerings in Azure (e.g. CosmosDB)
* Full-text search offerings in Azure
* Azure Data Lakes

## Exercise

### Overview

* Create and explore storage accounts in portal
* Create resource group with Bicep Template + CLI
  * [0010-deploy-storage.azcli](0010-deploy-storage.azcli)
  * [0010-deploy-storage.bicep](0010-deploy-storage.bicep)
  * [0010-deploy-container.bicep](0010-deploy-container.bicep)
* Access storage with Storage Explorer
  * Access local *azurite*
  * Access real storage account in Azure
* Simple console demo app in C# that uploads/downloads a blob
  * Tip: [Useful API Snippets](https://www.craftedforeveryone.com/beginners-guide-and-reference-to-azure-blob-storage-sdk-v12-dot-net-csharp/)
  * [0020-csv-client.azcli](0020-csv-client.azcli)
  * [data.csv](data.csv) (created with [https://mockaroo.com/](https://mockaroo.com/))

## Storyboard Console App

* Goal: Implement a simple CLI for interacting with *Azure Blob Storage* with .NET 5
  * List blobs
  * Upload blobs
  * Parse uploaded CSV files (with lease)
  * Share uploaded files (SAS)
* Create console app with .NET 5
  * *net5.0* TFM
  * Nullable reference types
  * Code analysis
* Add necessary NuGet packages:
  * *System.CommandLine* to build CLI
  * *Serilog.Sinks.Console* for logging
  * *Azure.Storage.Blobs* for Azure Blob Storage
  * *CsvHelper* for CSV parsing
* Add *list* feature
  * `ConnectionParameters`: discuss records, discuss nullable attributes
  * Add root command
  * Add list command
  * Add building of connection strings
  * Add list handler
* Add *upload* feature
  * Add upload command
  * Add upload handler
* Add *parse* feature
  * Add parse command
  * Add parse handler
  * Discuss leases
* Add *share* feature
  * Add share command
  * Add share handler
  * Discuss *Shared Access Signatures*
* Further exercises (if time)
  * What should we change to make code ready for automated unit tests?
  * Snapshots
  * Try with versioning
  * Try with soft delete
