#!/bin/bash

resourceGroup="rg-cap-dev"

az config set defaults.group="$resourceGroup"
az servicebus namespace list --query '[0].name'
namespace=$(az servicebus namespace list --query '[0].name' --output tsv)
az servicebus namespace authorization-rule list --namespace-name "$namespace"
azbConnectionString=$(az servicebus namespace authorization-rule keys list --namespace-name "$namespace" --name RootManageSharedAccessKey --query 'primaryConnectionString')

dotnet user-secrets --project ./src/CapExample.AppHost/ set ConnectionStrings:serviceBus $azbConnectionString
dotnet user-secrets --project ./src/CapExample.AppHost/ list

if [ -f .env ]
then
    export $(cat .env | sed 's/#.*//g' | xargs)
fi

db_password=$(jq -r '.inputs.sql.password' .azure/cap-dev/config.json)

dotnet user-secrets --project ./src/CapExample.AppHost/ set ConnectionStrings:sqldb \
        "Server=$SQLSERVER_SQLSERVERFQDN;Initial Catalog=sqldb;Persist Security Info=False;User ID=CloudSA;Password=$db_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

