# Storage

## Prerequisites

* [Install *azurite*](https://github.com/Azure/Azurite#npm): `npm install -g azurite`
* [Start *azurite*](https://github.com/Azure/Azurite#npm): `azurite --silent --location <data-folder> --debug <log-folder>\debug.log`
* [Install *Azure Storage Explorer*](https://azure.microsoft.com/en-us/features/storage-explorer/)

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

* Create and explore storage accounts in portal
* Create resource group with Bicep Template + CLI
  * [0010-deploy-storage.azcli](0010-deploy-storage.azcli)
  * [0010-deploy-storage.bicep](0010-deploy-storage.bicep)
  * [0010-deploy-container.bicep](0010-deploy-container.bicep)
* Access storage with Storage Explorer
  * Access local *azurite*
  * Access real storage account in Azure
* Simple console demo app in C# that uploads/downloads a blob (prepared, just to get people used to core concepts of API)
  * Used to introduce some fundamentals about .NET 5 (e.g. TFMs, recommended project settings, etc.)