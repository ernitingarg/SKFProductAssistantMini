# Build state
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./SKFProductAssistant.Function ./SKFProductAssistant.Function
WORKDIR "/src/SKFProductAssistant.Function"
RUN dotnet restore "SKFProductAssistant.Function.csproj"
RUN dotnet publish "SKFProductAssistant.Function.csproj" -c Release -o /publish

# Final state
FROM mcr.microsoft.com/azure-functions/dotnet:4-dotnet8.0
WORKDIR /home/site/wwwroot

COPY --from=build /publish .
COPY ./SKFProductAssistant.Function/Products/datasheets ./Products/datasheets

EXPOSE 80
EXPOSE 443

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

# Start .NET application.
CMD ["/azure-functions-host/Microsoft.Azure.WebJobs.Script.WebHost" ]
