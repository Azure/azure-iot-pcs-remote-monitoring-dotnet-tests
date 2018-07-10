using System;
using System.Collections.Generic;
using System.Text;

namespace IoTHubManager
{
    class Constants
    {

        public const string TEST = "Type";
        public const string INTEGRATION_TEST = "IntegrationTest";

        public class Urls
        {
            public const string IOTHUB_ADDRESS = "http://localhost:9002/v1";
            public const string DEVICE_PATH = "/devices";
            public const string JOBS_PATH = "/jobs/";

            public const string DEVICE_SIMULATION_ADDRESS = "http://localhost:9003/v1";
            public const string SIMULATION_PATH = "/simulations/1";
        }

        public class Path
        {
            private const string DEVICES_DIR = "./resources/devices/";
            private const string JOBS_DIR = "./resources/jobs/";

            public const string
                DEVICE_FILE_AUTO_GEN_AUTH = DEVICES_DIR + "Device_Template_Auto_Generated_Auth.json",
                DEVICE_FILE_SYMMETRIC_AUTH = DEVICES_DIR + "Device_Template_Symmetric_Auth.json",
                DEVICE_FILE_X509_AUTH = DEVICES_DIR + "Device_Template_X509_Auth.json";

            public const string
                TAGS_FILE = JOBS_DIR + "Tags.json",
                REBOOT_METHOD_FILE = JOBS_DIR + "Method_Chiller_Reboot.json",
                FIRMWAREUPDATE_METHOD_FILE = JOBS_DIR + "Method_Elevator_FrimwareUpdate.json",
                SIMULATION_FILE = JOBS_DIR + "Simulation.json";

        }

        public class TemplateKeys
        {
            public const string DEVICE_ID = "{DeviceId}";
            public const string PRIMARY_KEY = "{PrimaryKey}";
            public const string SECONDARY_KEY = "{SecondaryKey}";
            public const string PRIMARY_TH = "{PrimaryThumbprint}";
            public const string SECONDARY_TH = "{SecondaryThumbprint}";

            public const string JOB_ID = "{JobId}";
            public const string FAULTY_DEVICE_ID = "{FaultyDeviceId}";
        }

        public class SimulatedDevices
        {
            public const string SIMULATED_DEVICE = "chiller-01";
            public const string SIMULATED_FAULTY_DEVICE = "elevator-02";
        }

        public class Jobs
        {
            public const int WAIT = 600;
            public const int MAX_TRIALS = 5;
        }


    }
}
