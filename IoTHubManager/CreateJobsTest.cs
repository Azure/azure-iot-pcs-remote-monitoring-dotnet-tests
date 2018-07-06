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
    class CreateJobsTest
    {
        private readonly IHttpClient httpClient;
        private int simulatedDeviceCount = 0;
        private int simulatedFaultyDeviceCount = 0;

        public CreateJobsTest()
        {
            this.httpClient = new HttpClient();
            createSimulatedDevice();
        }

        





        private void createSimulatedDevice()
        {
            var devices = getSimulatedDevices();
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
            putSimulatedDevices(devices);

        }

        private void putSimulatedDevices(JObject devices)
        {
            var request = new HttpRequest(Constants.DEVICE_SIMULATION_ADDRESS + Constants.SIMULATION_PATH);
            request.SetContent(devices);
            IHttpResponse response = this.httpClient.PutAsync(request).Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private JObject getSimulatedDevices()
        {
            var request = new HttpRequest(Constants.DEVICE_SIMULATION_ADDRESS + Constants.SIMULATION_PATH);
            IHttpResponse response = this.httpClient.GetAsync(request).Result;
            return JObject.Parse(response.Content);
        }

        private IHttpResponse request(string content)
        {
            var request = new HttpRequest(Constants.IOTHUB_ADDRESS + Constants.DEVICE_PATH);
            request.SetContent(content);
            return this.httpClient.PostAsync(request).Result;
        }
    }
}
