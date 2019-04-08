:: -----------------------------------------------------------------------------
:: Required environment variables for local setup
:: -----------------------------------------------------------------------------
:: 1. Locate strings for each section below
:: 2. Paste your strings inside the quotation marks
:: 3. Save and run this file
:: -----------------------------------------------------------------------------

SETX PCS_KEYVAULT_NAME ""
SETX PCS_AAD_APPID ""
SETX PCS_AAD_APPSECRET ""

:: URLs
SETX PCS_AUTHENTICATION_WEBSERVICE_URL "http://localhost:9001/v1"
SETX PCS_IOTHUBMANAGER_WEBSERVICE_URL "http://localhost:9002/v1"
SETX PCS_DEVICESIMULATION_WEBSERVICE_URL "http://localhost:9003/v1"
SETX PCS_TELEMETRY_WEBSERVICE_URL "http://localhost:9004/v1"
SETX PCS_CONFIG_WEBSERVICE_URL "http://localhost:9005/v1"
SETX PCS_STORAGEADAPTER_WEBSERVICE_URL "http://localhost:9022/v1"
SETX PCS_TELEMETRYAGENT_WEBSERVICE_URL "http://localhost:9023/v1"
