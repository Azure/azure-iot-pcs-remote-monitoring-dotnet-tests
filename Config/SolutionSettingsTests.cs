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
        private const string TEST_JSON = @"{
                                        testObject: '123456789',
                                        Type: 'application/json'
                                        }";
        private const string TEST_IMAGE = @"/9j/4AAQSkZJRgABAQEASABIAAD/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAA
EAAAAAAAD//gAzUHJvY2Vzc2VkIEJ5IGVCYXkgd2l0aCBJbWFnZU1hZ2ljaywgejEuMS4wLiB8fEIyAP/bAEMAAgEBAgEBAgICAgICAgIDBQMD
AwMDBgQEAwUHBgcHBwYHBwgJCwkICAoIBwcKDQoKCwwMDAwHCQ4PDQwOCwwMDP/bAEMBAgICAwMDBgMDBgwIBwgMDAwMDAwMDAwMDAwMDAwMDA
wMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDP/AABEIABgAGAMBIgACEQEDEQH/xAAfAAABBQEBAQEBAQAAAAAAAAAAAQIDBAUGBwgJ
Cgv/xAC1EAACAQMDAgQDBQUEBAAAAX0BAgMABBEFEiExQQYTUWEHInEUMoGRoQgjQrHBFVLR8CQzYnKCCQoWFxgZGiUmJygpKjQ1Njc4OTpDRE
VGR0hJSlNUVVZXWFlaY2RlZmdoaWpzdHV2d3h5eoOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna
4eLj5OXm5+jp6vHy8/T19vf4+fr/xAAfAQADAQEBAQEBAQEBAAAAAAAAAQIDBAUGBwgJCgv/xAC1EQACAQIEBAMEBwUEBAABAncAAQIDEQQFIT
EGEkFRB2FxEyIygQgUQpGhscEJIzNS8BVictEKFiQ04SXxFxgZGiYnKCkqNTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqC
g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2dri4+Tl5ufo6ery8/T19vf4+fr/2gAMAwEAAhEDEQ
A/AP0+/a2/4KbWPwe+NmrfCW20nUtH8WfZIZ9O1DUJ7e2t9VWWB5Q1rucsY8xywmd0CJJFKuCVGfIPgF8cPij8Sf2l/hprOt+NtU0vSY9Sm09v
DbX6zpqsUgliIkSM+XIFO6RZpHaRfIUBcs2Ps39r/ULPQ/gPrWoXf9mxi3jERmu40cxwyMqT+XuH3zCXwBye3NfHek3ejfs9/GHQb5rq60bw54
Z8SRR3UId5rFLOWNik/k8pyZFfegBRgxGWQ5/n3jjN8RlXFeGqVcTUdOTp6JqMIc0pJJrm5ZJ2blKUOZe6lK2i/RuH8LQxWVVYRox50pK+7k0l
qtLp6pJJ666J6n6KKflFFeY+Df2rvDHjX4zr4Hs2lk1K4sru/tp43jlt5ktbkW0qllY7X3EOqnrGwY4J20V+84XGUMTD2mHkpK7V13XQ/P62Hq
UZctVWe+pyn7eN4dQ8A6ToX9h+KtSXU71bn7Zo1nJeCw8h4yVmjjDOySo7pwpwNx6hQfhrx38IfHnjL4Qy6bDo/inVNavL1Li7t5fDOp2cUcKp
5exXkt1BVTl8Z3YwFDlfmKK/IeNeF8HmOa/WMRe/uLSy+G7XTWzk3rffskl99w3mVXDYJRp23b118v0Wx9ff8Ey/hunw/wDhxqWm31lq39rafe
bxPf6PNYRIjhzttxOiyN8xlZn2rnzQOmACiiv0fhXBUsJlVLD0VaMU/wA2235t6vzPkM8rTq46pUnu3+iP/9k=";

        public SolutionSettingsTests()
        {
            this.httpClient = new HttpClient();
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
            request.SetContent(TEST_IMAGE);

            // Act for update
            var response = this.httpClient.PutAsync(request).Result;

            // Assert for update
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(TEST_IMAGE, response.Content);
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
            Assert.Equal(TEST_IMAGE, response.Content);
        }
    }
}