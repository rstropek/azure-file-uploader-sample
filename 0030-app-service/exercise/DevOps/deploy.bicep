@description('Name of the Resource Group')
param rgName string = 'HelloAspNet'

@description('Base name of the project. All resource names will be derived from that.')
param baseName string = 'helloaspnet'

param location string = resourceGroup().location

var insightsName = concat('insights-', uniqueString(baseName))
var appServiceName = concat('app-', uniqueString(baseName))
var testAppName = concat('web-test-', uniqueString(baseName))
var prodAppName = concat('web-', uniqueString(baseName))
var storageName = concat('st', uniqueString(baseName))

var roleIds = {
  storageBlobDataContributor: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
}

// For details about application insights see https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview
resource insights 'Microsoft.Insights/components@2015-05-01' = {
  name: insightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
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

resource hosting 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: appServiceName
  location: location
  sku: {
    name: 'P1V2'          // See also https://azure.microsoft.com/en-us/pricing/details/app-service/windows/
    capacity: 1
  }
}

// Test environment
resource testApp 'Microsoft.Web/sites@2020-12-01' = {
  name: testAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hosting.id
    siteConfig: {
      netFrameworkVersion: 'v5.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: insights.properties.InstrumentationKey
        }
      ]
    }
  }
}

// Prod environment
resource prodApp 'Microsoft.Web/sites@2020-12-01' = {
  name: prodAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hosting.id
    siteConfig: {
      netFrameworkVersion: 'v5.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: insights.properties.InstrumentationKey
        }
      ]
    }
  }
}

resource testStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(csvStorage.id, testApp.id)
  scope: csvStorage
  properties: {
    principalId: testApp.identity.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.storageBlobDataContributor)
      // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
      // Note also bug/limitation https://github.com/Azure/bicep/issues/2031#issuecomment-816743989
  }
}

resource prodStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(csvStorage.id, prodApp.id)
  scope: csvStorage
  properties: {
    principalId: prodApp.identity.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.storageBlobDataContributor)
      // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
      // Note also bug/limitation https://github.com/Azure/bicep/issues/2031#issuecomment-816743989
  }
}
