targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
param location string
@secure()
@metadata({azd: {
  type: 'inputs'
  autoGenerate: {
    sql: {
      password: { len: 10 }
    }
  }}
})
param inputs object


var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    location: location
    tags: tags
  }
}

module serviceBus 'serviceBus/aspire.hosting.azure.bicep.servicebus.bicep' = {
  name: 'serviceBus'
  scope: rg
  params: {
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalType: 'ServicePrincipal'
    queues: []
    serviceBusNamespaceName: 'servicebus'
    topics: []
  }
}
module sqlserver 'sqlserver/aspire.hosting.azure.bicep.sql.bicep' = {
  name: 'sqlserver'
  scope: rg
  params: {
    location: location
    databases: ['sqldb']
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalName: resources.outputs.MANAGED_IDENTITY_NAME
    serverName: 'sqlserver'
    inputs: inputs
  }
}
output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_NAME string = resources.outputs.MANAGED_IDENTITY_NAME
output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = resources.outputs.AZURE_LOG_ANALYTICS_WORKSPACE_NAME
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = resources.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID
output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN

output SERVICEBUS_SERVICEBUSENDPOINT string = serviceBus.outputs.serviceBusEndpoint
output SQLSERVER_SQLSERVERFQDN string = sqlserver.outputs.sqlServerFqdn
