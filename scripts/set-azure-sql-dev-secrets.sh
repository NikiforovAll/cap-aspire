#!/bin/bash

if [ -f .azure/cap-dev/.env ]
then
    export $(cat .azure/cap-dev/.env | sed 's/#.*//g' | xargs)
fi

db_password=$(jq -r '.inputs.sql.password' .azure/cap-dev/config.json)

dotnet user-secrets --project ./src/CapExample.AppHost/ set ConnectionStrings:sqldb \
    "Server=$SQLSERVER_SQLSERVERFQDN;Initial Catalog=sqldb;Persist Security Info=False;User ID=CloudSA;Password=$db_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"