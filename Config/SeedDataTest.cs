// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Helpers.Http;
using Helpers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Config
{
    [Collection("Config Tests")]
    public class SeedDataTest
    {
        private readonly IHttpClient httpClient;
        private const int TIMEOUT_MS = 10000;
        private bool seedComplete = false;

        public SeedDataTest()
        {
            this.httpClient = new HttpClient();
            this.seedComplete = SeedData.WaitForSeedComplete();
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that the service starts normally and returns ok status
        /// </summary>
        [Fact]
        public void Should_Return_OK_Status()
        {
            // Act
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/status");
            request.Options.Timeout = TIMEOUT_MS;
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Bootstrap a real HTTP server and test that seed data
        /// is created.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceGroupsAreCreatedOnStartup()
        {

            Assert.True(this.seedComplete, "Seed data failed.");

            // Act
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/devicegroups");
            request.Options.Timeout = TIMEOUT_MS;
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Bootstrap a real HTTP server and test that seed data
        /// is created.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void RulesAreCreatedOnStartup()
        {

            Assert.True(this.seedComplete, "Seed data failed.");

            //Arrange
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules");
            request.Options.Timeout = TIMEOUT_MS;

            // Act
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            JObject jsonResponse = JObject.Parse(response.Content);

            Assert.True(jsonResponse.HasValues);

            JArray items = (JArray)jsonResponse["Items"];
            //TODO: Make it equal to 5 once fresh storage is created
            //Since we are using same storage account for now this number should be great or equal to 5
            Assert.True(items.Count >= 4);

            List<string> groupIds = new List<string>();
            foreach (var rule in items)
            {
                groupIds.Add(rule["GroupId"].ToString());
            }

            Assert.Contains("default_Chillers", groupIds);
            Assert.Contains("default_PrototypingDevices", groupIds);
            Assert.Contains("default_Trucks", groupIds);
            Assert.Contains("default_Elevators", groupIds);
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Bootstrap a real HTTP server and test that seed data
        /// is created.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void SimulationIsCreatedOnStartup()
        {
            Assert.True(this.seedComplete, "Seed data failed.");

            // Act
            var request = new HttpRequest(Constants.SIMULATION_ADDRESS + "/simulations");
            request.Options.Timeout = TIMEOUT_MS;
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            JObject jsonResponse = JObject.Parse(response.Content);

            Assert.True(jsonResponse.HasValues);

            JArray items = (JArray)jsonResponse["Items"];
            JToken deviceModels = items[0]["DeviceModels"];
            Assert.True(deviceModels.Count() == 10);
        }
    }
}
