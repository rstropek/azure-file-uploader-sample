# Azure Functions

## Theory and Concepts

* General discussions
  * What is "event-driven computing"?
  * What is "serverless"?
* General introduction into Azure Functions
  * Relationship with App Service (already covered before)
  * Serverless vs. PaaS
  * [Relationship with other Serverless functions in Azure](https://docs.microsoft.com/en-us/azure/azure-functions/functions-compare-logic-apps-ms-flow-webjobs)
  * [Consumption vs. premium plan](https://docs.microsoft.com/en-us/azure/azure-functions/functions-scale)
  * [Supported languages and platforms](https://docs.microsoft.com/en-us/azure/azure-functions/supported-languages)
  * [Pricing](https://azure.microsoft.com/en-us/pricing/details/functions)
  * [Relationship with Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
  * Limitations for Functions with .NET 5
* [Triggers and bindings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings)
  * [Focus on blob bindings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob) and [Event Grid bindings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-grid)
* Azure Functions and [Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview)
  * Introduction into Service Bus
  * [Queues, Topics, Subscriptions](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions)
  * [Service Bus Triggers and Bindings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-service-bus)

## Out of scope

* Details about all possible triggers and bindings
* Introduction to container technology (Docker, K8s etc.)
* Hosting of functions in K8s

## Exercise

* Build "Hello World" Azure Function with HTTP trigger
  * Used to introduce some fundamentals about Azure Function C# projects (e.g. startup, function classes/methods, DI, local debugging etc.)
* Build function to generate SAS
  * [.NET 5](exercise/FileUploader.Server)
  * [.NET Core 3.1](exercise/FileUploader.Functions)
* Build function triggered by Blob upload
  * Downloads blob (CSV)
  * Azure SQL DB with Managed Identity
  * Parses CSV, writes content in Azure SQL DB, moves blob into "processed" container
* Error handling
  * Function sends message to Topic
  * Logic app sends email when error message appears in Subscription
* Create Azure Function with ARM Template + Azure CLI

## Important Links

* [Blob Storage bindings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob)
* [Functions in C# 5](https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide)
* [.NET 5 Functions worker in GitHub](https://github.com/Azure/azure-functions-dotnet-worker)
* [Functions and Managed Identity - limitations](https://docs.microsoft.com/en-us/azure/azure-functions/functions-reference#configure-an-identity-based-connection)
