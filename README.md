# fluent-service
Simplified REST API to create azure resources

This is an experimental project that exposes a simplified REST API interface to create subset of Azure resources. It uses Azure Libraries for .NET https://github.com/Azure/azure-libraries-for-net underneath.

## How to run

In order to tryout the project, follow below instructions:

1. Clone the repo, open in VS 2017 and build
2. Set the environment variable AZURE_AUTH_LOCATION  with value as path to service principal auth file.  You can generate this file using [Azure CLI 2.0](https://github.com/Azure/azure-cli) through the following command. Make sure you selected your subscription by `az account set --subscription <name or id>` and you have the privileges to create service principals.

```bash
az ad sp create-for-rbac --sdk-auth > my.azureauth
```

3. Set the environment variable FLUENT_SERVICE_STORAGE_CONNECTION_STRING with value as Azure storage connection string in the format 

```bash
DefaultEndpointsProtocol=https;AccountName=<account-name>;AccountKey=<account-key>;EndpointSuffix=core.windows.net
```

4. From command line switch to - `root\fluent-service\src\AnuChandy.Fluent.Service.BackEnd\bin\Debug\netcoreapp2.0` and run

```bash
> dotnet AnuChandy.Fluent.Service.BackEnd.dll
```
 
5. From another command line switch to `root\fluent-service\src\src\AnuChandy.Fluent.Service.REST\bin\Debug\netcoreapp2.0` and run

```bash
> dotnet AnuChandy.Fluent.Service.REST.dll
```

  This will output the url where the service is listening e.g. `http://localhost:5000`
 
6. Install [postman](https://www.getpostman.com/) and start sending the POST request to service endpoint `http://localhost:5000/BeginCreate` with payload as described in the following section. Remember to set header [Content-Type application/json] in postman


## Sample payloads

### Create an Azure VM with default configuration

```json
{
    "virtualMachines": [
        {
            "name": "fluentvm1",
            "region": "northcentralus",
            "linux": {
                "imageId": "Canonical:UbuntuServer:16.04.0-LTS",
                "credentials": {
                    "userName": "lxuser",
                    "password": "AbcXyz123!**"
                }
            }
        }
    ]
}

```

### Create an Azure VM with public-ip

```json
{
    "virtualMachines": [
        {
            "name": "fluentvm2",
            "region": "northcentralus",
            "linux": {
                "imageId": "Canonical:UbuntuServer:16.04.0-LTS",
                "credentials": {
                    "userName": "lxuser",
                    "password": "AbcXyz123!**"
                }
            },
            "newPrimaryPublicIPAddress": {
                "leafDomainLabel": "contoso7jts"
            }
        }
    ]
}

```

### Create two Azure VMs with new shared virtual network

```json
{
    "virtualMachines": [
        {
            "name": "fluentvm3",
            "region": "northcentralus",
            "linux": {
                "imageId": "Canonical:UbuntuServer:16.04.0-LTS",
                "credentials": {
                    "userName": "lxuser",
                    "password": "AbcXyz123!**"
                }
            },
            "newPrimaryNetwork": {
                "ref": "networks[0]"
            }
        },
        {
            "name": "fluentvm4",
            "region": "eastus2",
            "linux": {
                "imageId": "Canonical:UbuntuServer:16.04.0-LTS",
                "credentials": {
                    "userName": "lxuser",
                    "password": "AbcXyz123!**"
                }
            },
            "newPrimaryNetwork": {
                "ref": "networks[0]"
            }
        }
    ],
    "networks": [
        {
            "region": "northcentralus",
            "addressSpace": {
                "cidr": "10.0.0.0/28",
                "subnets": { "subnet-1": "10.0.0.0/28" }
            }
        }
    ]
}

```

### Create an Azure VM with an existing virtual network

```json
{
    "virtualMachines": [
        {
            "name": "fluentvm3",
            "region": "northcentralus",
            "linux": {
                "imageId": "Canonical:UbuntuServer:16.04.0-LTS",
                "credentials": {
                    "userName": "lxuser",
                    "password": "AbcXyz123!**"
                }
            },
            "existingPrimaryNetwork": {
                "resourceGroup": "myvnetResourceGroup"",
                 "name": "myvnet"
            }
        }
    ]
}

```
