using System;
using System.Collections.Generic;
using System.Text;

namespace IoTHubManager
{
    class Constants
    {

        public const string TEST = "Type";
        public const string INTEGRATION_TEST = "IntegrationTest";

        public const string IOTHUB_ADDRESS = "http://localhost:9002/v1";
        public const string DEVICE_PATH = "/devices";

        private const string DEVICES_DIR = "./resources/devices/";
        public const string DEVICE_FILE_AUTO_GEN_AUTH = DEVICES_DIR + "Device_Template_Auto_Generated_Auth.json",
               DEVICE_FILE_SYMMETRIC_AUTH = DEVICES_DIR + "Device_Template_Symmetric_Auth.json",
               DEVICE_FILE_X509_AUTH = DEVICES_DIR + "Device_Template_X509_Auth.json";


        public const string DEVICE_SIMULATION_ADDRESS = "http://localhost:9003/v1";
        public const string SIMULATION_PATH = "/simulations/";
        public const string SIMULATED_DEVICE = "chiller-01";
        public const string SIMULATED_FAULTY_DEVICE = "elevator-02";

    }
}
