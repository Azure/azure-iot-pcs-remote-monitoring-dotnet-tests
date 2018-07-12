// Copyright (c) Microsoft. All rights reserved.
using System;
using System.Net;
using Helpers.Http;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    internal class Simulation
    {
        //Simulation (Device count for Ids)
        private static Simulation simulation;
        internal int healthyDeviceNo = 0;
        internal int faultyDeviceNo = 0;
        //Http Request
        private HttpRequestWrapper Request;

        internal static Simulation GetSimulation()
        {
            if (simulation != null)
            {
                simulation = new Simulation();
            }
            return new Simulation();
        }

        private Simulation()
        {
            this.Request = new HttpRequestWrapper(Constants.SIMULATION_ADDRESS, Constants.Urls.SIMULATION_PATH);

            if (!CheckSimulationExists())
            {
                CreateSimulation();
            }
            CreateSimulatedDevices();
        }


        private void CreateSimulation()
        {
            IHttpResponse response = Request.Post(System.IO.File.ReadAllText(Constants.Path.SIMULATION_FILE));

            if (HttpStatusCode.OK != response.StatusCode)
            {
                throw new Exception("Error while creating simulated devices. Request to device simulation service failed with " + response.StatusCode + " status code.");
            }
        }

        private bool CheckSimulationExists()
        {
            try
            {
                GetSimulatedDevices();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void CreateSimulatedDevices()
        {
            var devices = GetSimulatedDevices();
            var deviceModels = devices["DeviceModels"];
 
            foreach (JObject device in deviceModels)
            {
                if (device["Id"].ToString() == Constants.SimulatedDevices.SIMULATED_DEVICE)
                {
                    healthyDeviceNo = device["Count"].ToObject<int>();
                    device["Count"] = (healthyDeviceNo + 1).ToString();
                }
                if (device["Id"].ToString() == Constants.SimulatedDevices.SIMULATED_FAULTY_DEVICE)
                {
                    faultyDeviceNo = device["Count"].ToObject<int>();
                    device["Count"] = (faultyDeviceNo + 1).ToString();
                }
            }
            PutSimulatedDevices(devices);
        }

        private void PutSimulatedDevices(JObject devices)
        {
            IHttpResponse response = Request.Put(devices);

            if (HttpStatusCode.OK != response.StatusCode)
            {
                throw new Exception("Create simulated device failure. Request to device simulation service failed with " + response.StatusCode + " status code.");
            }
        }

        public JObject GetSimulatedDevices()
        {
            IHttpResponse response = Request.Get();

            if (HttpStatusCode.OK != response.StatusCode)
            {
                throw new Exception("Couldn't fetch simulated devices. Request to device simulation service failed with " + response.StatusCode + " status code.");
            }
            return JObject.Parse(response.Content);
        }
    }
}
