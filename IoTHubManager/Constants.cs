// Copyright (c) Microsoft. All rights reserved.

namespace IoTHubManager
{
    class Constants : Helpers.Constants
    {

        // Web Service access URIs 
        public class Urls
        {
            // IOTHUB Manager
            public const string DEVICE_PATH = "/devices/";
            public const string JOBS_PATH = "/jobs/";
            public const string DEVICE_PROPERTIES_PATH = "/deviceproperties/";

            // Device Simulation
            public const string SIMULATION_PATH = "/simulations/1";
        }

        // JSON template file paths. Templates for Devices and Jobs.
        public class Path
        {
            private const string DEVICES_DIR = "./resources/devices/";
            private const string JOBS_DIR = "./resources/jobs/";

            // Device Files (JSON object holding device descriptions.)
            public const string DEVICE_FILE_AUTO_GEN_AUTH = DEVICES_DIR + "Device_Template_Auto_Generated_Auth.json";
            public const string DEVICE_FILE_SYMMETRIC_AUTH = DEVICES_DIR + "Device_Template_Symmetric_Auth.json";
            public const string DEVICE_FILE_X509_AUTH = DEVICES_DIR + "Device_Template_X509_Auth.json";

            // Job Files (JSON object holding job (tag/reconfigure) descriptions.)
            public const string TAGS_FILE = JOBS_DIR + "Tags.json";
            public const string DEVICE_PROPERTIES_FILE = JOBS_DIR + "Properties.json";
            public const string REBOOT_METHOD_FILE = JOBS_DIR + "Method_Chiller_Reboot.json";
            public const string FIRMWAREUPDATE_METHOD_FILE = JOBS_DIR + "Method_Elevator_FrimwareUpdate.json";
            public const string RECONFIGURE_DEVICE_FILE = JOBS_DIR + "Reconfigure_Chiller_Update_Model.json";
            public const string SIMULATION_FILE = JOBS_DIR + "Simulation.json";
        }

        // keys that should be replaced in the above defined JSON files.
        public class Keys
        {
            // Device and Jobs
            public const string DEVICE_ID = "{DeviceId}";
            public const string FAULTY_DEVICE_ID = "{FaultyDeviceId}";
            public const string ETAG = "{ETag}";
            public const string PROPERTIES = "{Properties}";
            public const string JOB_ID = "{JobId}";

            // Authentication
            public const string PRIMARY_KEY = "{PrimaryKey}";               //Symmetric
            public const string SECONDARY_KEY = "{SecondaryKey}";
            public const string PRIMARY_TH = "{PrimaryThumbprint}";         //X.509
            public const string SECONDARY_TH = "{SecondaryThumbprint}";     //X.509

            // Device Property Tests
            public const string PropertyKey = "{PropertyKey}";
            public const string PropertyValue = "{PropertyValue}";
        }

        public class Auth
        {
            public const int SYMMETRIC = 0;
            public const int X509 = 1;
        }

        // Simulated devices on which Job test(s) are run on.
        public class SimulatedDevices
        {
            public const string SIMULATED_DEVICE = "chiller-01";            //Non faulty
            public const string SIMULATED_FAULTY_DEVICE = "elevator-02";    //Faulty
        }

        public class Jobs
        {
            // Job Type
            public const int TAG_JOB = 4;
            public const int METHOD_JOB = 3;
            public const int RECONFIGURE_JOB = 4;

            // Job Status
            public const int JOB_IN_PROGRESS = 2;
            public const int JOB_QUEUED = 7;
            public const int JOB_COMPLETED = 3;

            // Retry 
            public const int WAIT= 30000;
            public const int MAX_TRIALS = 5;
        }
    }
}
