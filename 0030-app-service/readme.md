# Azure App Service Web Apps

## Prerequisites

* Install [.NET 5](https://dotnet.microsoft.com/) and latest version of [Visual Studio](https://visualstudio.microsoft.com/vs/)
* [*Azure App Service* extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azureappservice) for Visual Studio Code

## Theory and Concepts

* General introduction into different ways of hosting web apps in Azure
  * PaaS vs. IaaS vs. Serverless
  * Container vs. Code
  * Windows, Linux
  * [Pricing](https://azure.microsoft.com/en-us/pricing/details/app-service/windows/)
* [Introduction to App Service](https://docs.microsoft.com/en-us/azure/app-service/overview)
  * Supported platforms (special focus: ASP.NET Core)
  * Container support
  * [Relationship with Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/)
  * [Relationship with Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
* [App configuration](https://docs.microsoft.com/en-us/azure/app-service/configure-common)
  * [Relationship with Azure Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/general/basic-concepts)
  * [Relationship with Azure App Configuration](https://docs.microsoft.com/en-us/azure/azure-app-configuration/overview)
* [App Service Plans](https://docs.microsoft.com/en-us/azure/app-service/overview-hosting-plans)
  * Shared vs. dedicated vs. isolated
  * Scaling, cost optimization
  * Free, Basic, Standard, Premium, Consumption
* Networking
  * [VNet integration](https://docs.microsoft.com/en-us/azure/app-service/web-sites-integrate-with-vnet); only brief introduction
  * [Hybrid connections](https://docs.microsoft.com/en-us/azure/app-service/app-service-hybrid-connections)
* Security
  * [Easy Auth](https://docs.microsoft.com/en-us/azure/app-service/overview-authentication-authorization)
  * [Managed Identity](https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity)

## Out of scope

* Introduction to container technology (Docker, K8s etc.)
* Details about app monitoring and troubleshooting

## Exercise

* Implement a simple "Hello World" ASP.NET Core 5 Web API
  * Used to introduce some fundamentals about ASP.NET Core 5 (e.g. DI, middlewares, startup, API controllers, routing basics)
* Manually work with web apps
  * Create App Service Web App in Portal (Windows)
  * Deploy web API from Visual Studio
* Create Web App with ARM Template + Azure CLI
* CI/CD
  * [Add source code to Azure DevOps Repo](https://docs.microsoft.com/en-us/azure/devops/repos/git/creatingrepo)
  * [Configure CI/CD from Deployment Center](https://docs.microsoft.com/en-us/azure/app-service/deploy-continuous-deployment)
  * [Explore what's going on in Azure Pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/get-started/what-is-azure-pipelines)
* Extend web app: Access blob storage using Managed Identity
  * Extend code
  * Extend ARM Template (configures Managed Identity)
  * Use CI/CD to deploy new version
