# Azure VNets and Private Endpoints

## Theory and Concepts

* General introduction into Azure Networking
  * [Azure VNet](https://docs.microsoft.com/en-us/azure/virtual-network/virtual-networks-overview)
  * Relationship to VPN Gateways, API Gateways, Frontdoor, App Gateways
  * Address spaces and subnets
  * [Relationship with App Service](https://docs.microsoft.com/en-us/azure/app-service/web-sites-integrate-with-vnet)
* [General introduction into Private Endpoints](https://docs.microsoft.com/en-us/azure/private-link/private-endpoint-overview)
  * Private Links
  * Private Endpoints
  * [Private DNS Zones](https://docs.microsoft.com/en-us/azure/private-link/private-endpoint-dns)

## Out of scope

* Complex network topologies like hub/spoke
* Details about firewalls and gateways

## Exercise

* Create VNet with ARM Template + Azure CLI
* Create Private Endpoints incl. DNS Zones with ARM Template + Azure CLI
  * SQL DB
* Change ARM Template to integrate Azure Function with VNet
* Protect SQL DB from access over public internet
