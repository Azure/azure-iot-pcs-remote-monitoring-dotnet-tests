using Helpers.Http;
using Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;
using Newtonsoft.Json;

namespace Config
{
    [Collection("Config Tests")]
    public class DeviceGroupsTests : IDisposable
    {
        private readonly IHttpClient httpClient;
        private const int TIMEOUT_MS = 10000;
        private List<string> disposeList = new List<string>();

        public DeviceGroupsTests()
        {
            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// Test that the service can create a new Device Group
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldCreateNewDeviceGroup()
        {
            var createdDeviceGroup = this.CreateDeviceGroup("TEST-CreateDeviceGroup");

            // Dispose
            disposeList.Add(createdDeviceGroup.Id);
        }

        /// <summary>
        /// Test that the service can update a created Device Group
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldUpdateCreatedDeviceGroup()
        {
            // Create device group
            var createdDeviceGroup = this.CreateDeviceGroup("TEST-CreateDeviceGroup");

            // Arange for update
            var inputDeviceGroup = createdDeviceGroup;
            inputDeviceGroup.DisplayName = "TEST-UpdateDeviceGroup";
            var request = new HttpRequest();
            request.Options.Timeout = TIMEOUT_MS;
            request.SetUriFromString(Constants.CONFIG_ADDRESS + "/devicegroups/" + createdDeviceGroup.Id);
            request.SetContent(inputDeviceGroup);

            // Act for update
            var response = this.httpClient.PutAsync(request).Result;

            // Assert for update
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            createdDeviceGroup = JsonConvert.DeserializeObject<DeviceGroupApiModel>(response.Content);
            Assert.Equal(inputDeviceGroup.DisplayName, createdDeviceGroup.DisplayName);
            Assert.Equal(inputDeviceGroup.Conditions.ToString(), createdDeviceGroup.Conditions.ToString());

            // Dispose
            disposeList.Add(createdDeviceGroup.Id);
        }

        /// <summary>
        /// Test that the service can delete a created Device Group
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldDeleteCreatedDeviceGroup()
        {
            // Create device group
            var createdDeviceGroup = this.CreateDeviceGroup("TEST-CreateDeviceGroup");

            // Arange for delete
            var request = new HttpRequest();
            request.Options.Timeout = TIMEOUT_MS;
            request.SetUriFromString(Constants.CONFIG_ADDRESS + "/devicegroups/" + createdDeviceGroup.Id);
            request.SetContent("");

            // Act for delete
            var response = this.httpClient.DeleteAsync(request).Result;

            // Assert for delete
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Test that the service can get a created Device Group
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldGetCreatedDeviceGroup()
        {
            string inputDeviceGroupName = "TEST-CreateDeviceGroup";
            // Create device group
            var createdDeviceGroup = this.CreateDeviceGroup(inputDeviceGroupName);

            // Arange for get
            var request = new HttpRequest();
            request.Options.Timeout = TIMEOUT_MS;
            request.SetUriFromString(Constants.CONFIG_ADDRESS + "/devicegroups/" + createdDeviceGroup.Id);
            request.SetContent("");

            // Act for get
            var response = this.httpClient.GetAsync(request).Result;

            // Assert for get
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            createdDeviceGroup = JsonConvert.DeserializeObject<DeviceGroupApiModel>(response.Content);
            Assert.Equal(inputDeviceGroupName, createdDeviceGroup.DisplayName);
        }

        /// <summary>
        /// Test that the service can get a created Device Group
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldGetMultipleCreatedDeviceGroup()
        {
            // Create device group 1
            var createdDeviceGroup = this.CreateDeviceGroup("TEST-CreateDeviceGroup-1");
            // Dispose 1
            disposeList.Add(createdDeviceGroup.Id);

            // Create device group 2
            createdDeviceGroup = this.CreateDeviceGroup("TEST-CreateDeviceGroup-2");
            // Dispose 2
            disposeList.Add(createdDeviceGroup.Id);

            // Arange for get multiple
            var request = new HttpRequest();
            request.Options.Timeout = TIMEOUT_MS;
            request.SetUriFromString(Constants.CONFIG_ADDRESS + "/devicegroups");
            request.SetContent("");

            // Act for get multiple
            var response = this.httpClient.GetAsync(request).Result;

            // Assert for get multiple
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var DeviceGroupList = JsonConvert.DeserializeObject<DeviceGroupListApiModel>(response.Content);
            Assert.True(new List<DeviceGroupApiModel>(DeviceGroupList.Items).Count > 1);
        }

        private List<DeviceGroupCondition> CreateFakeDeviceGroupConditionList()
        {
            return new List<DeviceGroupCondition>()
            {
                new DeviceGroupCondition()
                {
                    Key = "TestKey",
                    Operator = OperatorType.EQ,
                    Value ="TestValue"
                }
            };
        }

        private DeviceGroupApiModel CreateDeviceGroup(string deviceGroupName){
            // Arange
            var inputDeviceGroup = new DeviceGroupApiModel
            {
                Id = "",
                DisplayName = deviceGroupName,
                Conditions = CreateFakeDeviceGroupConditionList(),
                ETag = "",
                Metadata = null
            };
            var request = new HttpRequest();
            request.SetContent(inputDeviceGroup);
            request.SetUriFromString(Constants.CONFIG_ADDRESS + "/devicegroups");
            request.Options.Timeout = TIMEOUT_MS;

            // Act
            var response = this.httpClient.PostAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var createdDeviceGroup = JsonConvert.DeserializeObject<DeviceGroupApiModel>(response.Content);
            Assert.Equal(inputDeviceGroup.DisplayName, createdDeviceGroup.DisplayName);
            Assert.Equal(inputDeviceGroup.Conditions.ToString(), createdDeviceGroup.Conditions.ToString());

            return createdDeviceGroup;
        }

        public void Dispose()
        {
            foreach (string id in disposeList)
            {
                var request = new HttpRequest();
                request.SetUriFromString(Constants.CONFIG_ADDRESS + "/devicegroups/" + id);
                request.Options.Timeout = TIMEOUT_MS;
                try
                {
                    var response = this.httpClient.DeleteAsync(request).Result;
                }
                catch (Exception)
                {
                    Console.Write("Unable to delete Device Group -" + id);
                }
            }
        }
    }
}
