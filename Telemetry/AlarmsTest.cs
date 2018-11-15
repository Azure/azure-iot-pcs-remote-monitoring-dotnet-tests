// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Helpers;
using Helpers.Http;
using Helpers.Models.TelemetryAlarms;
using Helpers.Models.TelemetryRules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Telemetry
{
    [Collection("Telemetry Tests")]
    public class AlarmsTest : IDisposable
    {
        private readonly IHttpClient httpClient;
        private ITestOutputHelper logger;

        private const string DEFAULT_CHILLERS_GROUP_ID = "default_Chillers";
        private string instantChillerRuleId;
        private string averageChillerRuleId;
        private const int ALARM_TRIGGER_WAIT_MSEC = 15000;
        private const int ALARM_CHECK_RETRY_COUNT = 16;

        private const string ALARMS_ENDPOINT_SUFFIX = "/alarms";
        private const string RULES_ENDPOINT_SUFFIX = "/rules";
        private const string ALARMSBYRULE_ENDPOINT_SUFFIX = "/alarmsbyrule";

        // list of rules to delete when tests are complete
        private List<string> disposeRulesList;

        public AlarmsTest(ITestOutputHelper logger)
        {
            this.httpClient = new HttpClient();
            this.logger = logger;
            this.disposeRulesList = new List<string>();
            this.instantChillerRuleId = "INSTANT_CHILLER_TEST_RULE";
            this.averageChillerRuleId = "AVERAGE_CHILLER_TEST_RULE";

            // Wait for seed data to run simulation before checking alarms
            Assert.True(SeedData.WaitForSeedComplete());

            // Create rules guaranteed to generate alarms with seed data.
            this.CreateRuleWithCalculation("Average", "60000", ref this.averageChillerRuleId, false);
            this.CreateRuleWithCalculation("Instant", "0", ref this.instantChillerRuleId, true);
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that the service starts normally and returns ok status
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void GetListReturnsAlarms_ForAllDevicesInTheLastHour()
        {
            // Arrange 
            const string PARAMETERS = "?devices=chiller-01.0%2C" +
                                      "elevator-02.0%2Cchiller-02.0%2C" +
                                      "prototype-02.0%2Ctruck-02.0%2C" +
                                      "truck-01.0%2Cengine-01.0%2C" +
                                      "prototype-01.0%2Cengine-02.0%2C" +
                                      "elevator-01.0&from=NOW-PT1H&to=NOW";

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + ALARMS_ENDPOINT_SUFFIX + PARAMETERS);
            Assert.True(this.AlarmsAreGenerated(request));
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void GetByIdReturnsDetails_ForExistingAlarm()
        {
            // Arrange 
            this.VerifyChillerAlarmsAreGenerated();

            const string PARAMETERS = "?devices=chiller-01.0%2Cchiller-02.0%2C";

            var listRequest = new HttpRequest(Constants.TELEMETRY_ADDRESS + ALARMS_ENDPOINT_SUFFIX + PARAMETERS);

            var listResponse = this.httpClient.GetAsync(listRequest).Result;
            var alarmList = JsonConvert.DeserializeObject<AlarmListApiModel>(listResponse.Content);
            var alarmId = alarmList.Items[0].Id;

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + ALARMS_ENDPOINT_SUFFIX + "/" + alarmId);
            var response = this.httpClient.GetAsync(request).Result;
            var alarm = JsonConvert.DeserializeObject<AlarmApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(alarm.ETag);
            Assert.NotNull(alarm.Id);
            Assert.NotNull(alarm.DeviceId);
            Assert.NotNull(alarm.Rule);
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void UpdateExistingAlarm_AcknowledgesAlarm()
        {
            // Arrange 
            this.VerifyChillerAlarmsAreGenerated();

            const string ACKNOWLEDGED_STATUS = "acknowledged";
            const string PARAMETERS = "?devices=chiller-01.0%2Cchiller-02.0%2C";

            var listRequest = new HttpRequest(Constants.TELEMETRY_ADDRESS + ALARMS_ENDPOINT_SUFFIX + PARAMETERS);
            var listResponse = this.httpClient.GetAsync(listRequest).Result;
            var alarmList = JsonConvert.DeserializeObject<AlarmListApiModel>(listResponse.Content);
            var alarmRequest = alarmList.Items[0];

            alarmRequest.Status = ACKNOWLEDGED_STATUS;

            // Act
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + ALARMS_ENDPOINT_SUFFIX + "/" + alarmRequest.Id);
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(alarmRequest));


            var response = this.httpClient.PatchAsync(request).Result;
            var updatedAlarm = JsonConvert.DeserializeObject<AlarmApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(alarmRequest.Id, updatedAlarm.Id);
            Assert.Equal(ACKNOWLEDGED_STATUS, updatedAlarm.Status);
        }

        /// <summary>
        /// This test verifies that alarms are being generated for the instant rule created
        /// in the test constructor.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void GetAlarmsByRuleReturnsAlarms_ForInstantRuleCalculation()
        {
            // Arrange 
            var request = new HttpRequest(
                Constants.TELEMETRY_ADDRESS +
                ALARMSBYRULE_ENDPOINT_SUFFIX +
                "/" + this.instantChillerRuleId);

            // Act 
            Assert.True(this.AlarmsByRuleAreGenerated(request));
        }

        /// <summary>
        /// This test verifies that alarms are being generated for the average rule created
        /// in the test constructor.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void GetAlarmsByRuleReturnsAlarms_ForAverageRuleCalculation()
        {
            // Arrange 
            var request = new HttpRequest(
                Constants.TELEMETRY_ADDRESS +
                ALARMSBYRULE_ENDPOINT_SUFFIX +
                "/" + this.averageChillerRuleId);

            // Act
            Assert.True(this.AlarmsByRuleAreGenerated(request));
        }

        private void CreateRuleWithCalculation(string calculation, string timePeriod, ref string id, bool includeActions)
        {
            var condition = new ConditionApiModel()
            {
                Field = "temperature",
                Operator = "GreaterThan",
                Value = "1"
            };

            var conditions = new List<ConditionApiModel> { condition };

            var newRule = new RuleApiModel()
            {
                Id = id,
                Name = calculation + " Faulty Test Rule " + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid(),
                Description = "Test Description",
                GroupId = DEFAULT_CHILLERS_GROUP_ID,
                Severity = "Info",
                Enabled = true,
                Calculation = calculation,
                TimePeriod = timePeriod,
                Conditions = conditions
            };

            if (includeActions)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "Notes", "Fake Note" },
                    { "Subject", "Fake Subject" }
                };
                var emails = new JArray { "fakeEmail@outlook.com" };
                parameters.Add("Recipients", emails);
                ActionApiModel action = new ActionApiModel
                {
                    Type = "Email",
                    Parameters = parameters
                };
                newRule.Actions = new List<ActionApiModel> { action };
            }
            
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + RULES_ENDPOINT_SUFFIX);
            request.AddHeader("Content-Type", "application/json");
            request.SetContent(JsonConvert.SerializeObject(newRule));

            var response = this.httpClient.PostAsync(request).Result;
            var ruleResponse = JsonConvert.DeserializeObject<RuleApiModel>(response.Content);

            // Update the saved rule ID
            id = ruleResponse.Id;

            // Dispose after tests run
            this.disposeRulesList.Add(ruleResponse.Id);
        }

        /// <summary>
        /// Try to delete all created rules upon completion.
        /// Each test should add the id of any rules created
        /// to the disposeRuleslist.
        /// </summary>
        public void Dispose()
        {
            this.logger.WriteLine("Alarms test cleanup: Deleting " + this.disposeRulesList.Count + " rules.");

            foreach (var ruleId in this.disposeRulesList)
            {
                var request = new HttpRequest(Constants.STORAGE_ADAPTER_ADDRESS + "/collections/rules/values/" + ruleId);

                var response = this.httpClient.DeleteAsync(request).Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    this.logger.WriteLine("Unable to delete test rule id:" + ruleId);
                }
            }
        }

        private void VerifyChillerAlarmsAreGenerated()
        {
            // Arrange
            const string PARAMETERS = "?devices=chiller-01.0%2Cchiller-02.0%2C";

            var listRequest = new HttpRequest(Constants.TELEMETRY_ADDRESS + ALARMS_ENDPOINT_SUFFIX + PARAMETERS);

            // Act - Assert - There are alarms
            Assert.True(this.AlarmsAreGenerated(listRequest));
        }

        private bool AlarmsAreGenerated(HttpRequest request)
        {
            for (var i = 0; i < ALARM_CHECK_RETRY_COUNT; i++)
            {
                var response = this.httpClient.GetAsync(request).Result;
                var alarmResponse = JsonConvert.DeserializeObject<AlarmListApiModel>(response.Content);

                if (response.StatusCode == HttpStatusCode.OK &&
                    alarmResponse.Items != null &&
                    alarmResponse.Items.Any())
                {
                    return true;
                }

                // wait before retry if able
                if (i < ALARM_CHECK_RETRY_COUNT - 1) System.Threading.Thread.Sleep(ALARM_TRIGGER_WAIT_MSEC);
                this.logger.WriteLine("Alarms check retry count: " + i + " out of " + ALARM_CHECK_RETRY_COUNT);
            }

            return false;
        }

        private bool AlarmsByRuleAreGenerated(HttpRequest request)
        {
            for (var i = 0; i < ALARM_CHECK_RETRY_COUNT; i++)
            {
                var response = this.httpClient.GetAsync(request).Result;
                var alarmResponse = JsonConvert.DeserializeObject<AlarmByRuleListApiModel>(response.Content);

                if (response.StatusCode == HttpStatusCode.OK &&
                    alarmResponse.Items != null &&
                    alarmResponse.Items.Any())
                {
                    return true;
                }

                // wait before retry if able
                if (i < ALARM_CHECK_RETRY_COUNT - 1) System.Threading.Thread.Sleep(ALARM_TRIGGER_WAIT_MSEC);
                this.logger.WriteLine("Alarms check retry count: " + i + " out of " + ALARM_CHECK_RETRY_COUNT);
            }

            return false;
        }
    }
}
