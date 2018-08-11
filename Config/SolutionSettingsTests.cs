using Helpers;
using Helpers.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Config
{
    [Collection("Config Tests")]
    public class SolutionSettingsTests
    {
        private readonly IHttpClient httpClient;
        private const int TIMEOUT_MS = 10000;
        private Dictionary<string, string> disposeList = new Dictionary<string, string>();
        private const string COLLECTION_ID = "/solution-settings";
        private const string THEME_KEY = "/theme";
        private const string LOGO_KEY = "/logo";
        private const string AZURE_MAPS_KEY = "AzureMapsKey";
        private const string TEST_JSON = "{\"testKey\": \"testValue12345\"}";
        private readonly byte[] logoFile = null;

        public SolutionSettingsTests()
        {
            this.httpClient = new HttpClient();
            this.logoFile = System.IO.File.ReadAllBytes(Constants.Path.LOGO_FILE);
        }

        /// <summary>
        /// Test that the service can set a solution-settings file in CosmosDB
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldSetSolutionSettingsTheme()
        {
            var request = new HttpRequest();
            request.Options.Timeout = TIMEOUT_MS;
            request.SetUriFromString(Constants.CONFIG_ADDRESS + COLLECTION_ID + THEME_KEY);
            request.SetContent(TEST_JSON);

            // Act for update
            var response = this.httpClient.PutAsync(request).Result;

            // Assert for update
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var themeOut = JsonConvert.DeserializeObject(response.Content) as JToken ?? new JObject();
            Assert.NotNull(themeOut[AZURE_MAPS_KEY]);
        }

        /// <summary>
        /// Test that the service can get a solution-settings file in CosmosDB
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldGetSolutionSettingsTheme()
        {
            // Arrange
            this.ShouldSetSolutionSettingsTheme();
            var request = new HttpRequest();
            request.Options.Timeout = TIMEOUT_MS;
            request.SetUriFromString(Constants.CONFIG_ADDRESS + COLLECTION_ID + THEME_KEY);

            // Act
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var themeOut = JsonConvert.DeserializeObject(response.Content) as JToken ?? new JObject();
            Assert.NotNull(themeOut[AZURE_MAPS_KEY]);
        }

        /// <summary>
        /// Test that the service can set a solution-settings file in CosmosDB
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldSetSolutionSettingsLogo()
        {
            var request = new HttpRequest();
            request.Options.Timeout = TIMEOUT_MS;
            request.SetUriFromString(Constants.CONFIG_ADDRESS + COLLECTION_ID + LOGO_KEY);
            request.SetContent(this.logoFile);

            // Act for update
            var response = this.httpClient.PutAsync(request).Result;

            // Assert for update
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(Convert.ToBase64String(this.logoFile), response.Content);
        }

        /// <summary>
        /// Test that the service can get a solution-settings file in CosmosDB
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldGetSolutionSettingsLogo()
        {
            // Arrange
            this.ShouldSetSolutionSettingsLogo();
            var request = new HttpRequest();
            request.Options.Timeout = TIMEOUT_MS;
            request.SetUriFromString(Constants.CONFIG_ADDRESS + COLLECTION_ID + LOGO_KEY);

            // Act
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(Convert.ToBase64String(this.logoFile), response.Content);
        }
    }
}