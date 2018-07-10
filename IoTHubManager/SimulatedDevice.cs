using System;
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
            this.httpClient = new HttpClient();

            if (!CheckSimulationExists())
            {
                CreateSimulation();
            }

            CreateSimulatedDevices();
        }


        private void CreateSimulation()
        {
            var request = new HttpRequest(Constants.Urls.DEVICE_SIMULATION_ADDRESS + Constants.Urls.SIMULATION_PATH);
            request.SetContent(System.IO.File.ReadAllText(Constants.Path.SIMULATION_FILE));
            IHttpResponse response = this.httpClient.PostAsync(request).Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var simulation = JObject.Parse(response.Content);
            ETag = simulation["Etag"].ToString();
        }

        private bool CheckSimulationExists()
        {
            try
            {
                GetSimulatedDevices();
            }
            catch (Exception e)
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
            IHttpResponse response = this.httpClient.PutAsync(request).Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }



        public JObject GetSimulatedDevices()
        {
            var request = new HttpRequest(Constants.Urls.DEVICE_SIMULATION_ADDRESS + Constants.Urls.SIMULATION_PATH);
            IHttpResponse response = this.httpClient.GetAsync(request).Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return JObject.Parse(response.Content);
        }
    }
}
