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
    internal class SimulatedDevices
    {
        private readonly IHttpClient httpClient;
        internal int simulatedDeviceCount = 0;
        internal int simulatedFaultyDeviceCount = 0;

        internal SimulatedDevices()
        {
            this.httpClient = new HttpClient();
            CreateSimulatedDevice();

        }

        private void CreateSimulatedDevice()
        {
            var devices = GetSimulatedDevices();
            var deviceModels = devices["DeviceModels"];

            foreach (JObject device in deviceModels)
            {
                if (device["Id"].ToString() == Constants.SIMULATED_DEVICE)
                {
                    simulatedDeviceCount = device["Count"].ToObject<int>();
                    device["Count"] = (simulatedDeviceCount + 1).ToString();
                }
                if (device["Id"].ToString() == Constants.SIMULATED_FAULTY_DEVICE)
                {
                    simulatedFaultyDeviceCount = device["Count"].ToObject<int>();
                    device["Count"] = (simulatedFaultyDeviceCount + 1).ToString();
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
            return JObject.Parse(response.Content);
        }
    }
}
