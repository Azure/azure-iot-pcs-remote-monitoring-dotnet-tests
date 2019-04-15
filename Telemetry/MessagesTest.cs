﻿
// Copyright (c) Microsoft. All rights reserved.

using System.Linq;
using System.Net;
using Helpers;
using Helpers.Http;
using Helpers.Models.TelemetryMessages;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Telemetry
{
    [Collection("Telemetry Tests")]
    public class MessagesTest
    {
        private readonly IHttpClient httpClient;
        private readonly ITestOutputHelper logger;

        private const int MESSAGES_WAIT_MSEC = 15000;
        private const int MESSAGES_CHECK_RETRY_COUNT = 16;

        private const string MESSAGES_ENDPOINT_SUFFIX = "/messages";

        public MessagesTest(ITestOutputHelper logger)
        {
            this.httpClient = new HttpClient();
            this.logger = logger;

            // Wait for seed data to run simulation
            Assert.True(SeedData.WaitForSeedComplete());

            // Wait for messages to be generated by simulation
            this.logger.WriteLine("Check if messages are being generated");
            if (!this.MessagesAreGenerated())
            {
                this.logger.WriteLine("Failure -- messages are not being generated.");
            }
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void GetMessages_ReturnsList()
        {
            // Arrange  
            var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + MESSAGES_ENDPOINT_SUFFIX);

            // Act
            var response = this.httpClient.GetAsync(request).Result;
            var messageResponse = JsonConvert.DeserializeObject<MessageListApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(messageResponse.Items);
        }

        private bool MessagesAreGenerated()
        {
            for (var i = 0; i < MESSAGES_CHECK_RETRY_COUNT; i++)
            {
                var request = new HttpRequest(Constants.TELEMETRY_ADDRESS + MESSAGES_ENDPOINT_SUFFIX);
                var response = this.httpClient.GetAsync(request).Result;
                var messageResponse = JsonConvert.DeserializeObject<MessageListApiModel>(response.Content);

                if (response.StatusCode == HttpStatusCode.OK &&
                    messageResponse.Items != null &&
                    messageResponse.Items.Any())
                {
                    return true;
                }

                // wait before retry if able
                if (i < MESSAGES_CHECK_RETRY_COUNT - 1)
                {
                    System.Threading.Thread.Sleep(MESSAGES_WAIT_MSEC);
                }
                this.logger.WriteLine("Messages check retry count: " + i + " out of " + MESSAGES_CHECK_RETRY_COUNT);
            }

            return false;
        }
    }
}