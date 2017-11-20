// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebService.Test.helpers.Http;
using Xunit;
using Xunit.Abstractions;

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
            Assert.True(items.Count == 5);

            List<string> ruleIds = new List<string>();
            foreach (var rule in items)
            {
                ruleIds.Add(rule["Id"].ToString());
            }

            Assert.True(ruleIds.Contains("default_Chiller_Pressure_High"));
            Assert.True(ruleIds.Contains("default_Prototyping_Temperature_High"));
            Assert.True(ruleIds.Contains("default_Elevator_Vibration_Stopped"));
            Assert.True(ruleIds.Contains("default_Truck_Temperature_High"));
            Assert.True(ruleIds.Contains("default_Engine_Fuel_Empty"));
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

            JArray devices = (JArray)jsonResponse["DeviceTypes"];
            Assert.True(devices.Count == 10);
        }
    }
}
