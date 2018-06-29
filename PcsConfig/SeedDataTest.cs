// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Helpers.Http;
using Newtonsoft.Json.Linq;
using Xunit;

namespace PcsConfig
{
    public class SeedDataTest
    {
        private readonly IHttpClient httpClient;
        private const string CONFIG_ADDRESS = "http://127.0.0.1:9005/v1";
        private const string TELEMETRY_ADDRESS = "http://127.0.0.1:9004/v1";
        private const string SIMULATION_ADDRESS = "http://127.0.0.1:9003/v1";

        public SeedDataTest()
        {
            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that the service starts normally and returns ok status
        /// </summary>
        [Fact]
        public void Should_Return_OK_Status()
        {
            // Act
            var request = new HttpRequest(CONFIG_ADDRESS + "/status");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Bootstrap a real HTTP server and test that seed data
        /// is created.
        /// </summary>
        [Fact, Trait("Type", "IntegrationTest")]
        public void DeviceGroupsAreCreatedOnStartup()
        {
            // Arrange
            // wait for config to run seed data
            System.Threading.Thread.Sleep(60000);

            // Act
            var request = new HttpRequest(CONFIG_ADDRESS + "/devicegroups");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Bootstrap a real HTTP server and test that seed data
        /// is created.
        /// </summary>
        [Fact, Trait("Type", "IntegrationTest")]
        public void RulesAreCreatedOnStartup()
        {
            // Arrange
            // wait for config to run seed data
            System.Threading.Thread.Sleep(60000);

            // Act
            var request = new HttpRequest(TELEMETRY_ADDRESS + "/rules");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            JObject jsonResponse = JObject.Parse(response.Content);


            Assert.True(jsonResponse.HasValues);

            JArray items = (JArray) jsonResponse["Items"];
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
        [Fact, Trait("Type", "IntegrationTest")]
        public void SimulationIsCreatedOnStartup()
        {
            // Arrange
            // wait for config to run seed data
            System.Threading.Thread.Sleep(60000);

            // Act
            var request = new HttpRequest(SIMULATION_ADDRESS + "/simulations");
            request.AddHeader("X-Foo", "Bar");
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
