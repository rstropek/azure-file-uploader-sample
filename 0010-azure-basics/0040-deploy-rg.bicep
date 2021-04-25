// this file can only be deployed at a subscription scope
targetScope = 'subscription'

@description('Name of the Resource Group to create')
param rgName string = 'DemoResourceGroup'

@description('Location for the Resource Group')
param rgLocation string = 'westeurope'

resource demoRG 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: rgName
  location: rgLocation
  tags: {
    Dept: 'IT'
    Environment: 'Test'
  }
}

output resourceID string = demoRG.id
