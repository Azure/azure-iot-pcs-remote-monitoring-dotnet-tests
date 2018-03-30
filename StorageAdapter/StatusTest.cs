using System;
using System.Net;
using Helpers.Http;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace StorageAdapter
{
    public class StatusTest
    {
        private readonly IHttpClient httpClient;
        private const string STORAGE_ADAPTER_ADDRESS_FORMAT = "http://{0}:9022/v1";
        private readonly string STORAGE_ADAPTER_ADDRESS;

        public StatusTest()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .AddEnvironmentVariables()
                .Build();

            string host = config["HOST"];
            this.httpClient = new HttpClient();
            this.STORAGE_ADAPTER_ADDRESS = string.Format(STORAGE_ADAPTER_ADDRESS_FORMAT, host);

        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Test that the service starts normally and returns ok status
        /// </summary>
        [Fact]
        public void Should_Return_OK_Status()
        {
            // Act
            var request = new HttpRequest(STORAGE_ADAPTER_ADDRESS + "/status");
            request.AddHeader("X-Foo", "Bar");
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
