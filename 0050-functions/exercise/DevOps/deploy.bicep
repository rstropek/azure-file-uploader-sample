@description('Name of the Resource Group')
param rgName string = 'HelloAspNet'

@description('Base name of the project. All resource names will be derived from that.')
param baseName string = 'helloaspnet'

param location string = resourceGroup().location
param uploadContainer string = 'csv-upload'
param processedContainer string = 'csv-processed'

param serverName string = 'sql-${uniqueString(baseName)}'
param sqlDBName string = 'sqldb-${uniqueString(baseName)}'

@description('Indicates whether it should be possible to access this SQL Server over the public internet')
param allowInternetAccess bool = true

@description('Object ID of the AAD group that should become DB administrators')
param aadAdminTeamId string

param administratorLogin string = 'axadmin'
@secure()
param administratorLoginPassword string

var insightsName = concat('insights-', uniqueString(baseName))
var appServiceName = concat('app-', uniqueString(baseName))
var functionName = concat('func-', uniqueString(baseName))
var storageName = concat('st', uniqueString(baseName))
var serviceBusName = concat('sb-', uniqueString(baseName))
var successTopicName = 'importsuccess'
var errorTopicName = 'importerror'
var successSubscriptionName = 'successlog'
var errorSubscriptionName = 'errorlog'
var serviceBusAuthorizationName = 'sendandlisten'

// For details about application insights see https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview
resource insights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: insightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource csvStorage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageName
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
    tier: 'Premium'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
  }
}

resource csvUploadContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' = {
  name: concat(csvStorage.name, '/default/', uploadContainer)
  properties: {
    publicAccess: 'None'
  }
}

resource csvProcessedContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' = {
  name: concat(csvStorage.name, '/default/', processedContainer)
  properties: {
    publicAccess: 'None'
  }
}

resource server 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: serverName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: allowInternetAccess ? 'Enabled' : 'Disabled'
  }
  resource aadAdmin 'administrators@2020-11-01-preview' = {
    name: 'ActiveDirectory'
    properties: {
      administratorType: 'ActiveDirectory'
      login: 'Applications Team - Database Administrator'
      sid: aadAdminTeamId
      tenantId: subscription().tenantId
    }
  }
  resource fwAzureApps 'firewallRules@2020-11-01-preview' = {
    name: concat(serverName, '-fw-azureApps')
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
  resource fwInternetAccess 'firewallRules@2020-11-01-preview' = if (allowInternetAccess) {
    name: concat(serverName, '-fw-internet')
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '255.255.255.255'
    }
  }
  resource sqlDB 'databases@2020-11-01-preview' = {
    name: sqlDBName
    location: location
    sku: {
      name: 'S0'
      tier: 'Standard'
    }
  }
}

resource hosting 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: appServiceName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource sbName 'Microsoft.ServiceBus/namespaces@2021-01-01-preview' = {
  name: serviceBusName
  location: location
  sku: {
    name: 'Standard'
  }
  resource successTopic 'topics@2018-01-01-preview' = {
    name: successTopicName
    properties: {
      autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
      defaultMessageTimeToLive: 'PT5M'
    }
    resource successSubscription 'subscriptions@2018-01-01-preview' = {
      name: successSubscriptionName
      properties: {
        autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
        defaultMessageTimeToLive: 'PT5M'
      }
    }
  }
  resource errorTopic 'topics@2018-01-01-preview' = {
    name: errorTopicName
    properties: {
      autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
      defaultMessageTimeToLive: 'PT5M'
    }
    resource errorSubscription 'subscriptions@2018-01-01-preview' = {
      name: errorSubscriptionName
      properties: {
        autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
        defaultMessageTimeToLive: 'PT5M'
      }
    }
  }
  resource serviceBusAuthorization 'AuthorizationRules@2017-04-01' = {
    name: serviceBusAuthorizationName
    properties: {
      rights: [
        'Send'
        'Listen'
      ]
    }
  }
}

// Test environment
resource testApp 'Microsoft.Web/sites@2020-12-01' = {
  name: functionName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: hosting.id
    siteConfig: {
      netFrameworkVersion: 'v5.0'
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: insights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~3'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
        }
        {
          name: 'ServiceBusConnection'
          value: 'Endpoint=sb://${serviceBusName}.servicebus.windows.net/;SharedAccessKeyName=${serviceBusAuthorizationName};SharedAccessKey=${listKeys(sbName::serviceBusAuthorization.id, '2017-04-01').primaryKey}'
        }
        {
          name: 'StorageConnection'
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
        }
        {
          name: 'SqlConnection'
          value: 'Server=${server.name}.database.windows.net; Authentication=Active Directory MSI; Initial Catalog=${sqlDBName};'
        }
      ]
    }
  }
}

output storageConnection string = 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
output serviceBusConnection string = 'Endpoint=sb://${serviceBusName}.servicebus.windows.net/;SharedAccessKeyName=${serviceBusAuthorizationName};SharedAccessKey=${listKeys(sbName::serviceBusAuthorization.id, '2017-04-01').primaryKey}'
output sqlConnection string = 'Server=${server.name}.database.windows.net; Authentication=Active Directory MSI; Initial Catalog=${sqlDBName};'
