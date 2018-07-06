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

            }

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
