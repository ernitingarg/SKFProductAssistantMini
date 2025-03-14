# SKFProductAssistant

## Overview
The **SKF Product Assistant** is an Azure Function that extracts product attributes from datasheets using AI-powered query processing. 
It integrates with Azure OpenAI to extract product name and then searches pre-defined JSON datasheets for attribute details.

## Features
- **OpenAI Query Processing:** Uses Azure OpenAI to extract the product name from user query.
- **Datasheet Lookup:** Searches through JSON datasheets to retrieve attribute details.
- **Caching:** Implements distributed caching (**In-memory or/and redis**) to improve performance.
- **Error Handling:** Error handling with detailed error logs.
- **Docker Support:** Fully containerized for deployment.

## Workflow
When a user submits a query, the Azure Function follows these steps:

- **Check Cache:**
	- The function first checks the cache for the query's hashed key.
	- If a cache hit occurs, the result is returned immediately.
- **Extract Product Name:**
	- For cache miss, the function calls `Azure OpenAI` to extract the product name from the query.
	- If OpenAI returns a valid product name, it is stored in the cache; otherwise, the function responds with an error message.
- **Find Product Attribute:**
	- The function then looks up the corresponding JSON datasheet for the retrieved product.
	- It attempts to find the best match for the requested attribute within the datasheet.
- **Handle Missing Attributes:**
	- If the requested attribute is not found, the function returns an error message.
	- If found, the attribute details are returned to the user.
- **Cache the Final Result:**
	- The function stores the final response in the cache to optimize future queries.

## Azure Deployment
- Please use Azure URL - https://skf-product-assistant.azurewebsites.net/
	- [healthcheck](https://skf-product-assistant.azurewebsites.net/api/healthcheck)
	- [GetInfo](https://skf-product-assistant.azurewebsites.net/api/getinfo)
	- [Query- What's the width of 6205?](https://skf-product-assistant.azurewebsites.net/api/query?q=%22What%20is%20the%20width%20of%206205?%22?)

## Running Locally
- Prerequisites
	- .NET 8 SDK: [Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
	- Azure Functions Core Tools: [Download](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-powershell)
	- Azure OpenAI Service API details
	- Azure Redis cache connection string (if redis cache is used)
- Clone the Repository
- cd `SKFProductAssistant.Function`
- Update local.settings.json with the required configuration.
	```json
	{
		"IsEncrypted": false,
		"Values": {
			"ASPNETCORE_ENVIRONMENT": "Local",
			"AzureWebJobsStorage": "UseDevelopmentStorage=true",
			"FUNCTIONS_WORKER_RUNTIME": "dotnet",
			"OpenAIConfig:Endpoint": "your_api_end_point",
			"OpenAIConfig:ApiKey": "your_api_api_key",
			"CacheProvider": "memory",
		}
	}
	```
- If `redis` cache needs to be used instead of in-memory.
	```json
	{
		"IsEncrypted": false,
		"Values": {
			"ASPNETCORE_ENVIRONMENT": "Local",
			"AzureWebJobsStorage": "UseDevelopmentStorage=true",
			"FUNCTIONS_WORKER_RUNTIME": "dotnet",
			"OpenAIConfig:Endpoint": "your_api_end_point",
			"OpenAIConfig:ApiKey": "your_api_api_key",
			"CacheProvider": "redis",
			"ConnectionStrings:CacheConnection": "your_redis_connection_string"
		}
	}
	```
- Run Azure Functions Locally
	```
	dotnet build
	func start
	```

## Running Locally with Docker
- Build Docker Image
	```
	docker build -t skf-product-assistant .
	```
- Run the Container using memory cache
	```
	docker run -p 7071:80 \
		-e OpenAIConfig__EndPoint="your_api_end_point" \
		-e OpenAIConfig__ApiKey="your_api_api_key" \
		-e CacheProvider="memory" \
		skf-product-assistant
	```
- Run the Container using redis cache
	```
	docker run -p 7071:80 \
		-e OpenAIConfig__EndPoint="your_api_end_point" \
		-e OpenAIConfig__ApiKey="your_api_api_key" \
		-e CacheProvider="redis" \
		-e ConnectionStrings__CacheConnection="your_redis_connection_string" \
		skf-product-assistant
	```
