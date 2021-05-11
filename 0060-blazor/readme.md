# ASP.NET Core Blazor

## Theory and Concepts

* [General introduction into Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
  * Client- vs. server-side Blazor
  * Introduction into WASM
* [Project structure](https://docs.microsoft.com/en-us/aspnet/core/blazor/project-structure)
* [Routing](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing)
* [Configuration](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/configuration)
* [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection)
* [Components](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/)
  * Data binding
  * Event handling
  * Lifecycle
* [Debugging WASM](https://docs.microsoft.com/en-us/aspnet/core/blazor/debug)
* [Calling web APIs](https://docs.microsoft.com/en-us/aspnet/core/blazor/call-web-api)
* [Deployment as a static web app](https://docs.microsoft.com/en-us/azure/static-web-apps/deploy-blazor)

## Out of scope

* Details about server-side Blazor
* SignalR integration

## Exercise

* Build "Hello World" Blazor app
  * Used to introduce some fundamentals about Blazor projects
* Build file upload sample
  * Add Blazor component for file upload
  * Extend web API for file upload
* Create Storage Account for static web hosting with ARM Template + Azure CLI
  * Extend ARM Template
* Goal at the end: Like before, BUT...
  * ...user can upload CSV file through browser UI
