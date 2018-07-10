// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using Helpers.Http;
using Xunit;

namespace AsaManager
{
    public class StatusTest
    {
        private readonly IHttpClient httpClient;
        private const string ASA_MANAGER_ADDRESS = "http://localhost:9024/v1";

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
            var request = new HttpRequest(ASA_MANAGER_ADDRESS + "/status");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}