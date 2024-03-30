#!/bin/bash

resourceGroup="rg-cap-dev"

namespace=$(
    az servicebus namespace list \
        --query '[0].name' \
        --resource-group $resourceGroup \
        --output tsv \
        2>/dev/null
)
azbConnectionString=$(
    az servicebus namespace authorization-rule keys list \
        --namespace-name "$namespace" \
        --name RootManageSharedAccessKey \
        --resource-group $resourceGroup \
        --query 'primaryConnectionString' \
        --output tsv \
        2>/dev/null
)

dotnet user-secrets --project ./src/CapExample.AppHost/ \
    set ConnectionStrings:serviceBus $azbConnectionString
