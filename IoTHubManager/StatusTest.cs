// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using Helpers.Http;
using Xunit;

namespace IoTHubManager
{
    [Collection("IoTHub Manager Tests")]
    public class StatusTest
    {
        private readonly IHttpClient httpClient;
        private const string IOTHUB_ADDRESS = "http://localhost:9002/v1";

        public StatusTest()
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
            var request = new HttpRequest(IOTHUB_ADDRESS + "/status");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}