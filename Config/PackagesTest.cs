// Copyright (c) Microsoft. All rights reserved.

using Helpers.Http;
using Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Xunit;
using Newtonsoft.Json;
using Config.Models;
using System.Net.Http;

namespace Config
{
    [Collection("Config Tests")]
    public class PackagesTest : IDisposable
    {
        private readonly IHttpClient httpClient;
        private const int TIMEOUT_MS = 10000;
        private const string PKG_TYPE_PARAM_NAME = "PackageType";
        private const string CONFIG_TYPE_PARAM_NAME = "ConfigType";
        private const string PACKAGE_PARAMETER_NAME = "Package";
        private List<string> disposeList = new List<string>();

        public PackagesTest()
        {
            this.httpClient = new Helpers.Http.HttpClient();
        }

        // Add test for invalid characters in package name

        /// <summary>
        /// Test that the service returns 404 when package is not found.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldReturnNotFoundForMissingPackage()
        {
            // Arrange
            var request = this.CreateGetRequest("nonexistantPackage");

            // Act
            var response = this.httpClient.GetAsync(request).Result;

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Test that the service can create a new Device Group
        /// </summary>
        [Theory, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public void ShouldCreatePackage(bool isEdgePackage, bool isCustomConfigType)
        {
            // Arange
            var packageName = "testPackage";
            var packageType = isEdgePackage ? PackageType.EdgeManifest : 
                PackageType.DeviceConfiguration;
            var configType = isEdgePackage ? string.Empty : ConfigType.FirmwareUpdate.ToString();
            var jsonManifest = isEdgePackage ? Constants.EDGE_PACKAGE_JSON : 
                Constants.ADM_PACKAGE_JSON;

            configType = isCustomConfigType ? "CustomConfig" : configType;

            // Act
            var createdPackage = this.CreatePackage(
                packageName, 
                packageType, 
                configType, 
                jsonManifest);

            var uploadedPackageResponse = this.RetrieveAndVerifyPackage(
                createdPackage.Id,
                packageName,
                packageType);

            // Assert
            Assert.False(string.IsNullOrEmpty(createdPackage.Id));
            Assert.Equal(jsonManifest, uploadedPackageResponse);

            // Dispose
            disposeList.Add(createdPackage.Id);
        }

        /// <summary>
        /// Test that the service can create a new config-types doc if NOT present
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldCreateNewConfigTypeDoc()
        {
            // Arange
            DeleteConfigTypes();

            // Act
            // Create package of type device configurations and config type Firmware update
            var packageName = "testPackage";
            var packageType = PackageType.DeviceConfiguration;
            var configType = ConfigType.FirmwareUpdate.ToString();
            var jsonManifest = Constants.ADM_PACKAGE_JSON;

            var createdPackage = this.CreatePackage(
                packageName,
                packageType,
                configType,
                jsonManifest);

            var uploadedPackageResponse = this.RetrieveAndVerifyPackage(
                createdPackage.Id,
                packageName,
                packageType);

            // Check if package was created successfully
            Assert.False(string.IsNullOrEmpty(createdPackage.Id));

            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/configtypes/");
            request.Options.Timeout = TIMEOUT_MS;

            var response = this.httpClient.GetAsync(request).Result;
            var configTypes = JsonConvert.DeserializeObject<ConfigTypeListApiModel>(response.Content);

            // Assert
            Assert.Single(configTypes.configTypes);
            Assert.Equal(configTypes.configTypes[0], ConfigType.FirmwareUpdate.ToString());
        }

        /// <summary>
        /// Test bulk retrieval of all uploaded packages
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldReturnAllPackages()
        {
            var testGuid = Guid.NewGuid();
            int numOfPackages = 5;

            var packageName = "testPackage" + testGuid;
            var packageType = PackageType.EdgeManifest;
            var configType = string.Empty;
            var jsonManifest = Constants.EDGE_PACKAGE_JSON;

            // Arange
            for (var i = 0; i < numOfPackages; i++)
            {
                if (i == 1 || i == 2)
                {
                    packageType = PackageType.DeviceConfiguration;
                    configType = (i == 1) ? ConfigType.FirmwareUpdate.ToString() : "CustomConfig";
                }

                this.CreatePackage(packageName + i, packageType, configType, jsonManifest);
            }

            // Act
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/packages");
            request.Options.Timeout = TIMEOUT_MS;
            var response = this.httpClient.GetAsync(request).Result;
            var uploadedPackages = JsonConvert.DeserializeObject<PackageListApiModel>(response.Content);

            // Assert
            var pkgNameAndIds = uploadedPackages.Items.Where(package => package.Name.StartsWith(packageName))
                                                      .OrderBy(package => package.Name)
                                                      .Select(package => new Tuple<string, string>(package.Name,package.Id));
            Assert.Equal(numOfPackages, pkgNameAndIds.Count());

            int count = 0;
            foreach (var pkgNameAndId in pkgNameAndIds)
            {
                Assert.Equal(packageName + (count++), pkgNameAndId.Item1);
                disposeList.Add(pkgNameAndId.Item2);
            }
        }

        /// <summary>
        /// Test that the service can create a new Device Group
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldDeletePackage()
        {
            var packageName = "testDeletePackage";
            var packageType = PackageType.EdgeManifest;
            var configType = string.Empty;
            var jsonManifest = Constants.EDGE_PACKAGE_JSON;

            // Arrange
            var createdPkg = this.CreatePackage(packageName, packageType, configType, jsonManifest);
            var createdPkgId = createdPkg.Id;

            var response = this.RetrievePackage(createdPkgId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            this.DeletePackage(createdPkgId);

            response = this.RetrievePackage(createdPkgId);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private PackageApiModel CreatePackage(
            string packageName,
            PackageType packageType,
            string configType,
            string content)
        {
            // Arange
            var request = this.CreatePackageRequestWithPackageModel(
                packageType,
                configType,
                content,
                packageName);

            // Act
            var response = this.httpClient.PostAsync(request).Result;
            var responsePackage = JsonConvert.DeserializeObject<PackageApiModel>(response.Content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(packageName, responsePackage.Name);
            Assert.Equal(content, responsePackage.Content);

            return responsePackage;
        }

        private string RetrieveAndVerifyPackage(string packageId, string packageName, PackageType type)
        {
            var response = this.RetrievePackage(packageId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var uploadedPackage = JsonConvert.DeserializeObject<PackageApiModel>(response.Content);
            Assert.Equal(type, uploadedPackage.packageType);
            Assert.Equal(packageId, uploadedPackage.Id);
            Assert.Equal(packageName, uploadedPackage.Name);
            return uploadedPackage.Content;
        }

        private IHttpResponse RetrievePackage(string packageId)
        {
            var request = this.CreateGetRequest(packageId);
            return this.httpClient.GetAsync(request).Result;
        }

        private void DeletePackage(string packageName)
        {
            var deleteRequest = this.CreateGetRequest(packageName);

            // Act
            var deleteResponse = this.httpClient.DeleteAsync(deleteRequest).Result;
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        private void DeleteConfigTypes()
        {
            // Delete config-types from Document DB.
            var deleteConfigUrl = Constants.STORAGE_ADAPTER_ADDRESS +
                "/collections/packages/values/config-types";
            var request = new HttpRequest(deleteConfigUrl);
            request.Options.Timeout = TIMEOUT_MS;
            var response = this.httpClient.DeleteAsync(request).Result;

            // Assert document config-types deleted successfully.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private HttpRequest CreateGetRequest(string packageId)
        {
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/packages/" + packageId);
            request.Options.Timeout = TIMEOUT_MS;
            return request;
        }

        private HttpRequest CreatePackageRequestWithPackageModel(
            PackageType packageType,
            string configType,
            string packageContent,
            string packageName)
        {
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/packages");

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(packageType.ToString()), PKG_TYPE_PARAM_NAME);
            content.Add(new StringContent(configType.ToString()), CONFIG_TYPE_PARAM_NAME);

            var jsonAsBytes = System.Text.Encoding.UTF8.GetBytes(packageContent);
            ByteArrayContent bytes = new ByteArrayContent(jsonAsBytes);
            content.Add(bytes, PACKAGE_PARAMETER_NAME, packageName);

            request.SetContent(content);
            request.Options.Timeout = TIMEOUT_MS;
            return request;
        }

        public void Dispose()
        {
            foreach (string packageId in disposeList)
            {
                this.DeletePackage(packageId);
            }
        }
    }
}
