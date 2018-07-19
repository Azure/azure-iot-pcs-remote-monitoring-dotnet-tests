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
    public class RulesTests
    {
        private readonly IHttpClient httpClient;
        private readonly string blobStorageConnectionString;
        private const int SLEEP_MS = 15000;

        public RulesTests()
        {
            this.httpClient = new HttpClient();
            this.blobStorageConnectionString = Helpers.GetStorageConnectionString();
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that rules are written to reference
        /// data in blob storage.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public async Task RulesInitiallyWritten()
        {
            // Arrange
            this.CheckForRulesInTelemetry();

            // Act
            bool foundFile = await Helpers.CheckBlobFileExists(
                this.blobStorageConnectionString,
                Constants.ASAManager.REFERENCE_DATA_CONTAINER,
                Constants.ASAManager.RULES_FILENAME);

            // Assert
            Assert.True(foundFile);
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Tests after a new rule is added,
        /// rules reference data is updated.
        /// Then checks if that rule is deleted,
        /// reference data will be updated again.
        /// This test takes 30+ seconds to run.
        /// </summary>
        // Todo: Temorarily commenting this test so travis won't block other PRs
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ReferenceDataUpdated_IfRuleAddedAndDeleted()
        {
            // Arrange
            this.CheckForRulesInTelemetry();
            DateTime start = DateTime.UtcNow;

            // Part 1: Write new rule
            // Act
            var response = this.AddNewRule();

            // Assert
            JObject jsonResponse = HttpHelpers.GetJsonResponseIfValid(response);
            string id = jsonResponse["Id"].ToString();
            Assert.NotNull(id);

            // Part 2: Check if new rules file is written
            // Arrange
            Thread.Sleep(SLEEP_MS);

            // Act
            bool foundValidBlob = Helpers.CheckIfBlobExistsAndContainsString(
                start,
                this.blobStorageConnectionString,
                Constants.ASAManager.REFERENCE_DATA_CONTAINER,
                Constants.ASAManager.RULES_FILENAME,
                id);

            // Part 3: Delete rule, still delete rule if did not find valid blob
            // Arrange
            start = DateTime.UtcNow;
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/" + id);

            // Act
            response = this.httpClient.DeleteAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(foundValidBlob, "Rules file was not written");

            // Part 4: Check if new rules file is written
            Thread.Sleep(SLEEP_MS);

            // Act
            foundValidBlob = Helpers.CheckIfBlobExistsAndDoesNotContainString(
                start,
                this.blobStorageConnectionString,
                Constants.ASAManager.REFERENCE_DATA_CONTAINER,
                Constants.ASAManager.RULES_FILENAME,
                id);

            // Assert
            Assert.True(foundValidBlob);
        }

        /**
         * Query telemetry service for rules and confirm there is
         * at least 1 rule.
         */
        private void CheckForRulesInTelemetry()
        {
            // Arrange
            string requestAddress = Constants.TELEMETRY_ADDRESS + "/rules";

            // Act
            var response = HttpHelpers.SendHttpGetRequestWithRetry(requestAddress, this.httpClient);

            // Assert
            Assert.NotNull(response);
            JObject jsonResponse = HttpHelpers.GetJsonResponseIfValid(response);
            JArray items = (JArray)jsonResponse["Items"];
            Assert.True(items.Count >= 1);
        }

        /**
         * Add new rule to the telemetry service and return
         * the id of the new rule
         */
        private IHttpResponse AddNewRule()
        { // Arrange
            string body = @"{
                'Name': 'Fake Rule',
                'Description': 'Fake Rule Description',
                'GroupId': 'Fake Group',
                'Severity': 'Warning',
                'Enabled':true,
                'Calculation':'average',
                'TimePeriod':'300000',
                'Conditions':[
                {
                    'Field':'temperature',
                    'Operator':'lessthan',
                    'Value':'120'
                }]
            }";
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + "/rules/");
            request.SetContent(body);

            // Act
            return this.httpClient.PostAsync(request).Result;
        }
    }
}
