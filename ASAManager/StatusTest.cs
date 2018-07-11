// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using Helpers;
using Helpers.Http;
using Xunit;

namespace ASAManager
{
    [Collection("ASA Manager Tests")]
    public class StatusTest
    {
        private readonly IHttpClient httpClient;

        public StatusTest()
        {
            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that the service starts normally and returns ok status
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void Should_Return_OK_Status()
        {
            // Act
            var request = new HttpRequest(Constants.ASA_MANAGER_ADDRESS + "/status");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}