// Copyright (c) Microsoft. All rights reserved.

namespace Helpers
{
    public class Constants
    {
        // Endpoints
        public const string CONFIG_ADDRESS = "http://127.0.0.1:9005/v1";
        public const string TELEMETRY_ADDRESS = "http://127.0.0.1:9004/v1";
        public const string SIMULATION_ADDRESS = "http://127.0.0.1:9003/v1";
        public const string STORAGE_ADAPTER_ADDRESS = "http://127.0.0.1:9022/v1";
        public const string ASA_MANAGER_ADDRESS = "http://127.0.0.1:9024/v1";
        public const string IOT_HUB_ADDRESS = "http://127.0.0.1:9002/v1";

        // Environment Variables
        public const string AZUREBLOB_ACCOUNT_ENV_VAR = "PCS_ASA_DATA_AZUREBLOB_ACCOUNT";
        public const string AZUREBLOB_KEY_ENV_VAR = "PCS_ASA_DATA_AZUREBLOB_KEY";
        public const string AZUREBLOB_ENDPOINT_ENV_VAR = "PCS_ASA_DATA_AZUREBLOB_ENDPOINT_SUFFIX";

        // Test Tags
        public const string TEST = "Type";
        public const string INTEGRATION_TEST = "IntegrationTest";

        // ASA Manager Specific Constants
        public class ASAManager
        {
            public const string REFERENCE_DATA_CONTAINER = "referenceinput";
            public const string DEVICE_GROUPS_FILENAME = "devicegroups.csv";
            public const string RULES_FILENAME = "rules.json";
        }

        // Config Specific Constants
        public class Path
        {
            // Test Image for logo test
            public const string LOGO_FILE = "./resources/" + "TestImage.jpg";
        }
    }
}
