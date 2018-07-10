﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Security.Cryptography;
using Helpers.Http;
using Xunit;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    internal class Simulation
    {
        private static Simulation simulation;
        private readonly IHttpClient httpClient;
        private string ETag;

        internal int healthyDeviceNo = 0;
        internal int faultyDeviceNo = 0;

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
            this.Request = new HttpRequestWrapper(Constants.Urls.DEVICE_SIMULATION_ADDRESS, Constants.Urls.SIMULATION_PATH);

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
            var request = new HttpRequest(Constants.Urls.DEVICE_SIMULATION_ADDRESS + Constants.Urls.SIMULATION_PATH);
            request.SetContent(devices);
            IHttpResponse response = Request.Put(devices);

            if (HttpStatusCode.OK != response.StatusCode)
            {
                throw new Exception("Create simulated device failure. Request to device simulation service failed with " + response.StatusCode + " status code.");
            }
        }



        public JObject GetSimulatedDevices()
        {
            var request = new HttpRequest(Constants.Urls.DEVICE_SIMULATION_ADDRESS + Constants.Urls.SIMULATION_PATH);
            IHttpResponse response = this.httpClient.GetAsync(request).Result;

            if (HttpStatusCode.OK != response.StatusCode)
            {
                throw new Exception("Couldn't fetch simulated devices. Request to device simulation service failed with " + response.StatusCode + " status code.");
            }

            return JObject.Parse(response.Content);
        }
    }
}