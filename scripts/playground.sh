#!/bin/bash


resourceGroup="rg-cap-dev"

# az config set defaults.group="$resourceGroup"
az servicebus namespace list --query '[0].name'
namespace=$(az servicebus namespace list --query '[0].name')
az servicebus namespace authorization-rule list --namespace-name "$namespace"
az servicebus namespace authorization-rule keys list --namespace-name "$namespace" --name RootManageSharedAccessKey --query 'primaryConnectionString'

dotnet user-secrets --project ./src/CapExample.AppHost/ set ConnectionStrings:serviceBus "<connectionString>"
dotnet user-secrets --project ./src/CapExample.AppHost/ list

dotnet user-secrets --project ./src/CapExample.AppHost/ set ConnectionStrings:sqldb \
    "Server=$db_server;Initial Catalog=sqldb;Persist Security Info=False;User ID=CloudSA;Password=$db_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
