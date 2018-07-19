// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using Helpers.Http;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ASAManager
{
    [Collection("ASA Manager Tests")]
    public class DeviceGroupsTests
    {
        private readonly IHttpClient httpClient;
        private readonly string blobStorageConnectionString;
        private const int SLEEP_MS = 70000;

        public DeviceGroupsTests()
        {
            this.httpClient = new HttpClient();
            this.blobStorageConnectionString = Helpers.GetStorageConnectionString();
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that device groups are written to reference
        /// data in blob storage.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public async Task DeviceGroupsInitiallyWritten()
        {
            // Arrange
            this.VerifyDependencies();

            // Act
            bool foundFile = await Helpers.CheckBlobFileExists(
                this.blobStorageConnectionString,
                Constants.ASAManager.REFERENCE_DATA_CONTAINER,
                Constants.ASAManager.DEVICE_GROUPS_FILENAME);

            // Assert
            Assert.True(foundFile, "Could not find initial device groups file");
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Tests after a new device group is added,
        /// device groups reference data is updated.
        /// Then checks if that device group is deleted,
        /// reference data will be updated again.
        /// This test takes 2+ minutes to run.
        /// </summary>
        ///  Temporarily disable flaky test
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ReferenceDataUpdated_IfDeviceGroupAddedAndDeleted()
        {
            // Arrange
            this.VerifyDependencies();
            DateTime start = DateTime.UtcNow;

            // Part 1: Write new device group
            // Act
            var response = this.CreateNewDeviceGroup();

            // Assert
            JObject jsonResponse = HttpHelpers.GetJsonResponseIfValid(response);
            string id = jsonResponse["Id"].ToString();
            Assert.NotNull(id);
            Console.WriteLine("created device group with id " + id);
            // Part 2: Check if new device groups file is written
            // Still delete device group if failed

            // Arrange
            Thread.Sleep(SLEEP_MS);

            // Act
            bool foundValidBlob = Helpers.CheckIfBlobExistsAndContainsString(
                                    start,
                                    this.blobStorageConnectionString,
                                    Constants.ASAManager.REFERENCE_DATA_CONTAINER,
                                    Constants.ASAManager.DEVICE_GROUPS_FILENAME,
                                    id);

            // Part 3: Delete device group
            // Still delete device group if did not find valid blob
            // Arrange
            start = DateTime.UtcNow;
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/devicegroups/" + id);

            // Act
            response = this.httpClient.DeleteAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(foundValidBlob, "Device Group file was not written after device group create");

            // Part 4: Check if new device groups file is written
            // Arrange
            Thread.Sleep(SLEEP_MS);

            // Act
            foundValidBlob = Helpers.CheckIfBlobExistsAndDoesNotContainString(
                start,
                this.blobStorageConnectionString,
                Constants.ASAManager.REFERENCE_DATA_CONTAINER,
                Constants.ASAManager.DEVICE_GROUPS_FILENAME,
                id);

            // Assert
            Assert.True(foundValidBlob, "Device Group file was not written after device group delete");
        }

        /**
         * In order for tests to run as expected, the service
         * must have at least 1 device and at least 1 existing device
         * group. Verify that Iot Hub Manager returns and least one
         * device and Config Service returns at least one device group.
         */
        private void VerifyDependencies()
        {

            // check for at least 1 device
            // Arrange
            string requestAddress = Constants.IOT_HUB_ADDRESS + "/devices";

            // Act
            var response = HttpHelpers.SendHttpGetRequestWithRetry(requestAddress, this.httpClient);

            // Assert
            Assert.NotNull(response);
            JObject jsonResponse = HttpHelpers.GetJsonResponseIfValid(response);
            JArray items = (JArray)jsonResponse["Items"];
            Assert.True(items.Count >= 1);

            // check for at least 1 device group
            // Arrange
            requestAddress = Constants.CONFIG_ADDRESS + "/devicegroups";

            // Act
            response = HttpHelpers.SendHttpGetRequestWithRetry(requestAddress, this.httpClient);

            // Assert
            Assert.NotNull(response);
            jsonResponse = HttpHelpers.GetJsonResponseIfValid(response);
            items = (JArray)jsonResponse["items"];
            Assert.True(items.Count >= 1);
        }

        /**
         * Create a new dummy device group with no conditions
         * (will match any device). Assert request was successful
         * and return id of new device group.
         */
        private IHttpResponse CreateNewDeviceGroup()
        {
            string body = @"{
                'displayName': 'Fake Group',
                'conditions': []
            }";
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/devicegroups");
            request.SetContent(body);

            return this.httpClient.PostAsync(request).Result;
        }
    }
}
