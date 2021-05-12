@description('Name of the Resource Group')
param rgName string = 'AxaFileUploader'

@description('Base name of the project. All resource names will be derived from that.')
param baseName string = 'helloaspnet'

@description('Location of the resources')
param location string = resourceGroup().location

@description('Name of the blob storage container receiving CSV uploads')
param uploadContainer string = 'csv-upload'

@description('Name of the blob storage container receiving processed CSV uploads')
param processedContainer string = 'csv-processed'

@description('Name of the SQL server')
param serverName string = 'sql-${uniqueString(baseName)}'

@description('Name of the SQL server database')
param sqlDBName string = 'sqldb-${uniqueString(baseName)}'

@description('Indicates whether it should be possible to access the SQL Server over the public internet')
param allowInternetAccess bool = true

@description('Object ID of the AAD group that should become SQL DB administrators')
param aadAdminTeamId string

@description('Login name for SQL Server admin account')
param administratorLogin string = 'axadmin'

@description('Password for the SQL Server admin account')
@secure()
param administratorLoginPassword string

@description('Address prefix for generated virtual network')
param vnetAddressPrefix string = '10.0.0.0/16'

@description('Address prefix for subnet for non-private endpoint resources')
param subnetDefaultAddressPrefix string = '10.0.1.0/24'

@description('Address prefix for subnet private endpoint resources')
param subnetPrivateEndpointAddressPrefix string = '10.0.2.0/24'

@description('Size of demo VM')
param VmSize string = 'Standard_DS2_v2'

@description('Login name for VM admin account')
param vmAdminUsername string = 'otto'

@description('Password for the VM admin account')
@secure()
param vmAdminPassword string

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

var vnetName = 'vnet-${baseName}-${uniqueString(baseName)}'
var vmSubnet = 'subnet-${baseName}-vm'
var privateEndpointSubnet = 'subnet-${baseName}-pe'
var sqlPeName = 'pe-${serverName}'
var privateDnsZoneName = 'privatelink${environment().suffixes.sqlServerHostname}'
var pvtendpointdnsgroupname = '${sqlPeName}/pednsgroupname'
var publicIpAddressName_var = 'ip-${baseName}-${uniqueString(baseName)}'
var vmName = 'vm-${baseName}-${uniqueString(baseName)}'
var networkInterfaceName_var = 'nic-${baseName}-${uniqueString(baseName)}'

// Add virtual network for private resources
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
    subnets: [
      {
        name: vmSubnet
        properties: {
          addressPrefix: subnetDefaultAddressPrefix
        }
      }
      {
        name: privateEndpointSubnet
        properties: {
          addressPrefix: subnetPrivateEndpointAddressPrefix
          privateEndpointNetworkPolicies: 'Disabled'
        }
      }
    ]
  }
}

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

resource sqlPe 'Microsoft.Network/privateEndpoints@2020-06-01' = {
  name: sqlPeName
  location: location
  properties: {
    subnet: {
      id: resourceId('Microsoft.Network/virtualNetworks/subnets', vnetName, privateEndpointSubnet)
    }
    privateLinkServiceConnections: [
      {
        name: sqlPeName
        properties: {
          privateLinkServiceId: server.id
          groupIds: [
            'sqlServer'
          ]
        }
      }
    ]
  }
  dependsOn: [
    vnet
  ]
}

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-01-01' = {
  name: privateDnsZoneName
  location: 'global'
  properties: { }

  resource privateDnsZoneName_privateDnsZoneName_link 'virtualNetworkLinks@2020-01-01' = {
    name: '${privateDnsZoneName}-link'
    location: 'global'
    properties: {
      registrationEnabled: false
      virtualNetwork: {
        id: vnet.id
      }
    }
  }
}

resource pvtendpointdnsgroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2020-06-01' = {
  name: pvtendpointdnsgroupname
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'config1'
        properties: {
          privateDnsZoneId: privateDnsZone.id
        }
      }
    ]
  }
  dependsOn: [
    sqlPe
  ]
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
      vnetName: vnetName
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
        {
          name: 'WEBSITE_DNS_SERVER'
          value: '168.63.129.16'
        }
        {
          name: 'WEBSITE_VNET_ROUTE_ALL'
          value: '1'
        }
      ]
    }
  }
}

resource publicIpAddressName 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: publicIpAddressName_var
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: toLower(vmName)
    }
  }
}

resource networkInterfaceName 'Microsoft.Network/networkInterfaces@2020-06-01' = {
  name: networkInterfaceName_var
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipConfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: publicIpAddressName.id
          }
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', vnetName, vmSubnet)
          }
        }
      }
    ]
  }
  dependsOn: [
    vnet
  ]
}

resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: vmName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: VmSize
    }
    osProfile: {
      computerName: vmName
      adminUsername: vmAdminUsername
      adminPassword: vmAdminPassword
    }
    storageProfile: {
      imageReference: {
        publisher: 'Canonical'
        offer: 'UbuntuServer'
        sku: '18.04-LTS'
        version: 'latest'
      }
      osDisk: {
        name: '${vmName}OsDisk'
        caching: 'ReadWrite'
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'Standard_LRS'
        }
        diskSizeGB: 128
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: networkInterfaceName.id
        }
      ]
    }
  }
}

output storageConnection string = 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
output serviceBusConnection string = 'Endpoint=sb://${serviceBusName}.servicebus.windows.net/;SharedAccessKeyName=${serviceBusAuthorizationName};SharedAccessKey=${listKeys(sbName::serviceBusAuthorization.id, '2017-04-01').primaryKey}'
output sqlConnection string = 'Server=${server.name}.database.windows.net; Authentication=Active Directory MSI; Initial Catalog=${sqlDBName};'
