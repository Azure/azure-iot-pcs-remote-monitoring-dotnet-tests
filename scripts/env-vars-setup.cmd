:: -----------------------------------------------------------------------------
:: Required environment variables for local setup
:: -----------------------------------------------------------------------------
:: 1. Locate strings for each section below
:: 2. Paste your strings inside the quotation marks
:: 3. Save and run this file
:: -----------------------------------------------------------------------------

:: -----------------------------------------------------------------------------
:: IoTHub connection string:
:: {Your IoT Hub} > Shared access policies > Connection string -- primary key
:: Note: use this value for both environment variables below
::
:: Example:
:: PCS_IOTHUBREACT_ACCESS_CONNSTRING=HostName "iothub-test123.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=12345="
:: PCS_IOTHUB_CONNSTRING=HostName "iothub-test123.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=12345="
:: -----------------------------------------------------------------------------
SETX PCS_IOTHUBREACT_ACCESS_CONNSTRING ""
SETX PCS_IOTHUB_CONNSTRING ""

:: -----------------------------------------------------------------------------
:: IoTHub Info can be found in the Azure portal at:
:: {Your IoT Hub} > Built-in Endpoints > events
::
:: Example:
:: PCS_IOTHUBREACT_HUB_NAME "iothub-test123"
:: PCS_IOTHUBREACT_HUB_ENDPOINT "Endpoint=sb://iothub-foo-iothub-bar-12345-12345.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=12345="
:: PCS_IOTHUBREACT_HUB_PARTITIONS "4"
:: -----------------------------------------------------------------------------
SETX PCS_IOTHUBREACT_HUB_NAME ""
SETX PCS_IOTHUBREACT_HUB_ENDPOINT ""
SETX PCS_IOTHUBREACT_HUB_PARTITIONS ""

:: -----------------------------------------------------------------------------
:: EventHub Info can be found in the Azure portal at:
:: {Your EventHub} > Shared access policies > RootManageSharedAccessKey
::
:: Example:
:: PCS_EVENTHUB_CONNSTRING "Endpoint=sb://eventhub-foo-eventhub-bar-12345-12345.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=12345="
:: PCS_EVENTHUB_NAME "eventhub-test123"
:: -----------------------------------------------------------------------------
SETX PCS_EVENTHUB_CONNSTRING ""
SETX PCS_EVENTHUB_NAME ""
SETX PCS_ACTION_EVENTHUB_NAME ""
SETX PCS_ACTION_EVENTHUB_CONNSTRING ""
:: -----------------------------------------------------------------------------
:: Storage Account information:
:: {Your storage account} > Access keys
::
:: Example: 
:: PCS_IOTHUBREACT_AZUREBLOB_ACCOUNT "storagetest123"
:: PCS_IOTHUBREACT_AZUREBLOB_KEY "12345...6789=="
:: PCS_ASA_DATA_AZUREBLOB_ACCOUNT "storagetest123"
:: PCS_ASA_DATA_AZUREBLOB_KEY "12345...6789=="
:: -----------------------------------------------------------------------------
SETX PCS_ASA_DATA_AZUREBLOB_ACCOUNT ""
SETX PCS_ASA_DATA_AZUREBLOB_KEY ""
SETX PCS_AZUREBLOB_CONNSTRING ""

:: -----------------------------------------------------------------------------
:: Storage Endpoint Suffix: 
:: {Your storage account} > Properties > Primary Blob Service Endpoint > value
:: in the URI after the "blob." (eg. `core.windows.net`)
::
:: Example:
:: PCS_IOTHUBREACT_AZUREBLOB_ENDPOINT_SUFFIX "core.windows.net"
:: PCS_ASA_DATA_AZUREBLOB_ENDPOINT_SUFFIX "core.windows.net"
:: -----------------------------------------------------------------------------
SETX PCS_ASA_DATA_AZUREBLOB_ENDPOINT_SUFFIX ""

:: -----------------------------------------------------------------------------
:: DocumentDb Conn String:
:: {Your Document DB} > Keys > Primary Connection String
:: Note: use this value for all 3 environment variables below
::
:: Example:
:: PCS_STORAGEADAPTER_DOCUMENTDB_CONNSTRING=AccountEndpoint "https://documentdb-test123.documents.azure.com:443/;AccountKey=12345==;"
:: PCS_TELEMETRY_DOCUMENTDB_CONNSTRING=AccountEndpoint "https://documentdb-test123.documents.azure.com:443/;AccountKey=12345==;"
:: -----------------------------------------------------------------------------
SETX PCS_STORAGEADAPTER_DOCUMENTDB_CONNSTRING ""
SETX PCS_TELEMETRY_DOCUMENTDB_CONNSTRING ""

:: -----------------------------------------------------------------------------
:: Auth is disabled for local development
:: -----------------------------------------------------------------------------
SETX PCS_AUTH_REQUIRED "false"

:: -----------------------------------------------------------------------------
:: Azure maps key
:: -----------------------------------------------------------------------------
SETX PCS_AZUREMAPS_KEY "static"

:: -----------------------------------------------------------------------------
:: You can allow requests to another domain with the PCS_CORS_WHITELIST, see
:: the following link for more information:
:: https://docs.microsoft.com/aspnet/web-api/overview/security/enabling-cross-origin-requests-in-web-api
:: -----------------------------------------------------------------------------
SETX PCS_CORS_WHITELIST ""

:: -----------------------------------------------------------------------------
:: Endpoint of logic app workflow. 
:: See Azure Portal => Your resource group => Your Logic App => Logic App Designer => When a Http Request is received => HTTP POST URL
:: -----------------------------------------------------------------------------
SETX PCS_LOGICAPP_ENDPOINT_URL ""

:: -----------------------------------------------------------------------------
:: Solution URL
:: -----------------------------------------------------------------------------
SETX PCS_SOLUTION_WEBSITE_URL ""

:: -----------------------------------------------------------------------------
:: Environment variables for TSI
:: The FQDN (Fully Qualified Domain Name) for the Time Series endpoint
:: see: Azure Portal => Your Resource Group => Time Series Insights Environment => Data Access FQDN
:: -----------------------------------------------------------------------------
SETX PCS_TELEMETRY_STORAGE_TYPE "tsi"
SETX PCS_TSI_FQDN ""

:: -----------------------------------------------------------------------------
:: The tenant for the Azure Active Directory application
:: see: Azure Portal => Azure Active Directory => Properties => Directory ID
:: -----------------------------------------------------------------------------
SETX PCS_AAD_TENANT "{enter the Azure Active Directory Tenant for the application here}"

:: -----------------------------------------------------------------------------
:: The Application ID registered with Azure Active Directory
:: see: Azure Portal => Azure Active Directory => App Registrations => Your App => Application ID
:: -----------------------------------------------------------------------------
SETX PCS_AAD_APPID "{enter Azure Active Directory application ID here}"

:: -----------------------------------------------------------------------------
:: The Application Secret for your Azure Active Directory Application
:: see: Azure Portal => Azure Active Directory => App Registrations => Your App => Settings => Passwords
:: -----------------------------------------------------------------------------
SETX PCS_AAD_APPSECRET "{enter your application secret here}"

:: URLs
SETX PCS_AUTHENTICATION_WEBSERVICE_URL "http://localhost:9001/v1"
SETX PCS_IOTHUBMANAGER_WEBSERVICE_URL "http://localhost:9002/v1"
SETX PCS_DEVICESIMULATION_WEBSERVICE_URL "http://localhost:9003/v1"
SETX PCS_TELEMETRY_WEBSERVICE_URL "http://localhost:9004/v1"
SETX PCS_CONFIG_WEBSERVICE_URL "http://localhost:9005/v1"
SETX PCS_STORAGEADAPTER_WEBSERVICE_URL "http://localhost:9022/v1"
SETX PCS_TELEMETRYAGENT_WEBSERVICE_URL "http://localhost:9023/v1"
