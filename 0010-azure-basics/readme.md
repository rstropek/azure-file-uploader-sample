# Azure Basics

## Prerequisites

* Make sure to have the latest version of the Azure CLI installed.
  * Find out [what's the current version](https://docs.microsoft.com/en-us/cli/azure/release-notes-azure-cli)
  * Check version: `az version`
  * [Install guide](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
* Make sure to have access to an Azure subscription

## Theory and Concepts

* General introduction to Azure
* Ways to interact with Azure
  * [Portal](https://portal.azure.com)
  * [Shell](https://shell.azure.com)
  * [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/)
  * [Azure PowerShell](https://docs.microsoft.com/en-us/powershell/azure/)
  * [Azure REST API](https://docs.microsoft.com/en-us/rest/api/azure/)
  * [Azure ARM Templates](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/overview)
  * [Azure Bicep](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/bicep-decompile)
* Azure AAD Fundamentals
  * [Terminology](https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-whatis#terminology)
  * Azure tenants basics
* [Azure Scope Levels](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/overview#understand-scope)
  * Management Groups (not covered in detail)
  * Subscriptions
    * [Note: Mention limits](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits)
  * Resource groups
  * Resources
* Managing resources
  * [Tags](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/tag-resources)
  * [RBAC](https://docs.microsoft.com/en-us/azure/role-based-access-control/)
  * [Resource locks](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/lock-resources)

## Out of scope

* Azure security (e.g. policies, Azure Security Center, Azure Sentinal, etc.)

## Exercises

* Create and explore resource groups in portal
* Create resource group with Azure CLI
  * [0010-basics.azcli](0010-basics.azcli)
  * [0020-create-rg.azcli](0020-create-rg.azcli)
* Create resource group with ARM Template + CLI
  * [0030-deploy-template.azcli](0030-deploy-template.azcli)
  * [0030-deploy-rg.json](0030-deploy-rg.json)
* Create resource group with Bicep Template + CLI
  * [0040-deploy-bicep.azcli](0040-deploy-bicep.azcli)
  * [0040-deploy-rg.bicep](0040-deploy-rg.bicep)
