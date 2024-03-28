# DotNetCore.CAP + Aspire [![.NET](https://github.com/NikiforovAll/cap-aspire/actions/workflows/dotnet.yml/badge.svg)](https://github.com/NikiforovAll/cap-aspire/actions/workflows/dotnet.yml)

Technologies:

* DotNetCore.CAP
* Aspire
* .NET9
* Azure Service Bus
* Azure SQL
* Bicep
* azd

Get your application up on Azure using [Azure Developer CLI](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/overview) (azd). Add your application code, write Infrastructure as Code assets in [Bicep](https://aka.ms/bicep) to get your application up and running quickly.

The following assets have been provided:

- Infrastructure-as-code (IaC) Bicep files under the `infra` folder that demonstrate how to provision resources and setup resource tagging for azd.
- A [dev container](https://containers.dev) configuration file under the `.devcontainer` directory that installs infrastructure tooling by default. This can be readily used to create cloud-hosted developer environments such as [GitHub Codespaces](https://aka.ms/codespaces).

![resources](/assets/resources.png)

## Getting Started

```bash
azd env new cap-dev
azd provision
```

```bash
dotnet user-secrets --project ./src/CapExample.AppHost/ set ConnectionStrings:serviceBus "<connectionString1>"
dotnet user-secrets --project ./src/CapExample.AppHost/ set ConnectionStrings:sqldb "<connectionString2>"
```

```bash
dotnet watch --project ./src/CapExample.AppHost
```

```htpp
GET http://localhost:5288/send
```
