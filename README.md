# AzureCosmosDemo 

Example solution on how to wrap calls to the Cosmos Client to perform CRUD operations with Azure Cosmos DB.

This project is made up of 3 assemblies

|Assembly|Purpose|
|---|---|
|CosmosDemoApp|A sample console application that interacts with a Cosmos DB Collection.|
|CosmosCoreUtil|The workhorse of the solution. This provides the wrappers around the Azure Cosmos DB service.|
|Logging.Local|Mocked out ILogger interface to print to the console.|
|CosmosModels|An example class library that contains the definition of a class used to read/write a Cosmos Collection.|

## Pre-Requisites

- You must have an Azure subscription to follow along with this code. You can get a [free](https://azure.microsoft.com/en-us/free) if you need/want one. Be aware, there is limited funds on a free account and you will NOT run over that amount testing this project. 
- Install the [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) on your machine/environment.
    - Once installed, perform the `az login` command and ensure you are logged into Azure locally.
- Login to the [Azure Portal](https://portal.azure.com), find your subscription and create a Resource Group for the following resources.
- Create an [Azure Cosmos DB](#azure-cosmos-db) and follow the directions in that section.
- From the blade of your Azure Cosmos DB.
    - Copy the Endpoint value from the blade and put it into CosmosDemoApp/Program.cs on line 18.
    - Copy the URL of the Cosmos DB instance and trim it down to 
        - /subscriptions/YOURSUB/resourceGroups/YOURRG/providers/Microsoft.DocumentDb/databaseAccounts/YOURDB and pit it into CosmosDemoApp/Program.cs on line 19.
- Open Visual Studio.
- Set CosmosDemoApp as the Startup Project
- Hit F5 to start the program.

#### Azure Cosmos DB

- In the Azure Portal create a resource group.
- In the Azure Portal create an Azure Cosmos DB in the resource group you created.
    - Choose Azure Cosmos DB for NoSQL as the option.
- When the resource is created, go to the Cosmos DB blade and choose option Data Explorer
- In the drop down with New Container, select New Container
- For Database ID enter :  Customers
- For Container ID enter : Customer
- For parition key enter : /partitionKey
