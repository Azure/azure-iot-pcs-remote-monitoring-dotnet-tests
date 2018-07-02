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
:: PCS_IOTHUBREACT_ACCESS_CONNSTRING=HostName=iothub-test123.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=12345=
:: PCS_IOTHUB_CONNSTRING=HostName=iothub-test123.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=12345=
:: -----------------------------------------------------------------------------
SETX PCS_IOTHUBREACT_ACCESS_CONNSTRING ""
SETX PCS_IOTHUB_CONNSTRING ""

:: -----------------------------------------------------------------------------
:: IoTHub Info can be found in the Azure portal at:
:: {Your IoT Hub} > Endpoints > events
::
:: Example:
:: PCS_IOTHUBREACT_HUB_NAME=iothub-test123
:: PCS_IOTHUBREACT_HUB_ENDPOINT=Endpoint=sb://iothub-foo-iothub-bar-12345-12345.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=12345=
:: PCS_IOTHUBREACT_HUB_PARTITIONS=4
:: -----------------------------------------------------------------------------
SETX PCS_IOTHUBREACT_HUB_NAME ""
SETX PCS_IOTHUBREACT_HUB_ENDPOINT ""
SETX PCS_IOTHUBREACT_HUB_PARTITIONS ""

:: -----------------------------------------------------------------------------
:: EventHub Info can be found in the Azure portal at:
:: {Your EventHub} > Shared access policies > RootManageSharedAccessKey
::
:: Example:
:: PCS_EVENTHUB_CONNSTRING=Endpoint=sb://eventhub-foo-eventhub-bar-12345-12345.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=12345=
:: PCS_EVENTHUB_NAME=eventhub-test123
:: -----------------------------------------------------------------------------
SETX PCS_EVENTHUB_CONNSTRING ""
SETX PCS_EVENTHUB_NAME ""

:: -----------------------------------------------------------------------------
:: Storage Account information:
:: {Your storage account} > Access keys
::
:: Example: 
:: PCS_IOTHUBREACT_AZUREBLOB_ACCOUNT=storagetest123
:: PCS_IOTHUBREACT_AZUREBLOB_KEY=12345...6789==
:: PCS_ASA_DATA_AZUREBLOB_ACCOUNT=storagetest123
:: PCS_ASA_DATA_AZUREBLOB_KEY=12345...6789==
:: -----------------------------------------------------------------------------
SETX PCS_IOTHUBREACT_AZUREBLOB_ACCOUNT ""
SETX PCS_IOTHUBREACT_AZUREBLOB_KEY ""
SETX PCS_ASA_DATA_AZUREBLOB_ACCOUNT ""
SETX PCS_ASA_DATA_AZUREBLOB_KEY ""

:: -----------------------------------------------------------------------------
:: Storage Endpoint Suffix: 
:: {Your storage account} > Properties > Primary Blob Service Endpoint > value
:: in the URI after the "blob." (eg. `core.windows.net`)
::
:: Example:
:: PCS_IOTHUBREACT_AZUREBLOB_ENDPOINT_SUFFIX=core.windows.net
:: PCS_ASA_DATA_AZUREBLOB_ENDPOINT_SUFFIX=core.windows.net
:: -----------------------------------------------------------------------------
SETX PCS_IOTHUBREACT_AZUREBLOB_ENDPOINT_SUFFIX ""
SETX PCS_ASA_DATA_AZUREBLOB_ENDPOINT_SUFFIX ""

:: -----------------------------------------------------------------------------
:: DocumentDb Conn String:
:: {Your Document DB} > Keys > Primary Connection String
:: Note: use this value for all 3 environment variables below
::
:: Example:
:: PCS_STORAGEADAPTER_DOCUMENTDB_CONNSTRING=AccountEndpoint=https://documentdb-test123.documents.azure.com:443/;AccountKey=12345==;
:: PCS_TELEMETRY_DOCUMENTDB_CONNSTRING=AccountEndpoint=https://documentdb-test123.documents.azure.com:443/;AccountKey=12345==;
:: -----------------------------------------------------------------------------
SETX PCS_STORAGEADAPTER_DOCUMENTDB_CONNSTRING ""
SETX PCS_TELEMETRY_DOCUMENTDB_CONNSTRING ""

:: -----------------------------------------------------------------------------
:: Auth is disabled for local development
:: -----------------------------------------------------------------------------
SETX PCS_AUTH_REQUIRED "false"

:: -----------------------------------------------------------------------------
:: To add a bing map key, please follow the directions here:
:: https://msdn.microsoft.com/library/ff428642.aspx
:: -----------------------------------------------------------------------------
SETX PCS_AZUREMAPS_KEY "static"

:: -----------------------------------------------------------------------------
:: You can allow requests to another domain with the PCS_CORS_WHITELIST, see
:: the following link for more information:
:: https://docs.microsoft.com/aspnet/web-api/overview/security/enabling-cross-origin-requests-in-web-api
:: -----------------------------------------------------------------------------
SETX PCS_CORS_WHITELIST ""

:: URLs
SETX PCS_AUTHENTICATION_WEBSERVICE_URL "http://127.0.0.1:9001/v1"
SETX PCS_IOTHUBMANAGER_WEBSERVICE_URL "http://iothub-manager:9002/v1"
SETX PCS_DEVICESIMULATION_WEBSERVICE_URL "http://device-simulation:9003/v1"
SETX PCS_TELEMETRY_WEBSERVICE_URL "http://telemetry:9004/v1"
SETX PCS_CONFIG_WEBSERVICE_URL "http://pcs-config:9005/v1"
SETX PCS_STORAGEADAPTER_WEBSERVICE_URL "http://storage-adapter:9022/v1"
SETX PCS_TELEMETRYAGENT_WEBSERVICE_URL "http://127.0.0.1:9023/v1"
