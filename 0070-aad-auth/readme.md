# Protect System using AAD

## Theory and Concepts

* [Recap: OAuth2 and OpenID Connect](https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/auth-oidc)
  * [JWT](https://docs.microsoft.com/en-us/azure/active-directory/develop/security-tokens)
  * [Consent](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent)
* [App registrations](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app) and [Service Principals](https://docs.microsoft.com/en-us/azure/active-directory/develop/app-objects-and-service-principals)
* Single-tenant vs. multi-tenant apps
* [API scopes](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-expose-web-apis)
  * [Relationship with Microsoft Graph API](https://docs.microsoft.com/en-us/graph/overview)
* [Microsoft Identity Platform](https://docs.microsoft.com/en-us/azure/active-directory/develop/)
* [Overview AAD External Identities](https://docs.microsoft.com/en-us/azure/active-directory/external-identities/)

## Out of scope

* Discussion of general topics regarding users, groups, roles, hybrid solutions, AAD security mechanisms (e.g. MFA, conditional access etc.)

## Exercise

* Register apps in AAD
  * Backend API
  * Blazor UI
  * Portal vs. Azure CLI
* Extend Blazor UI
  * Login and logout
  * Acquire access tokens for backend access
* Extend Backend API with JWT validation

## Important Links

* [ASP.NET Core Blazor authentication and authorization](https://docs.microsoft.com/en-us/aspnet/core/blazor/security/)
* [Blazor Tutorial for Microsoft Identity Platform](https://docs.microsoft.com/en-us/azure/active-directory/develop/tutorial-blazor-webassembly)
* [Slides](https://slides.com/rainerstropek/gaa-2021-aad-auth-for-apis/fullscreen)