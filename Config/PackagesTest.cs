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
        private const string PKG_TYPE_PARAM_NAME = "type";
        private const string PACKAGE_PARAMETER_NAME = "package";
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
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldCreatePackage()
        {
            // Arange
            var packageName = "testPackage";
            var packageType = PackageType.EdgeManifest;
            var jsonManifest = Constants.TEST_PACKAGE_JSON;

            // Act
            var createdPackage = this.CreatePackage(packageName, packageType, jsonManifest);
            var uploadedPackageResponse = this.RetrieveAndVerifyPackage(createdPackage.Id, packageName, packageType);

            // Assert
            Assert.False(string.IsNullOrEmpty(createdPackage.Id));
            Assert.Equal(jsonManifest, uploadedPackageResponse);

            // Dispose
            disposeList.Add(createdPackage.Id);
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
            var jsonManifest = Constants.TEST_PACKAGE_JSON;

            // Arange
            for (var i = 0; i < numOfPackages; i++)
            {
                this.CreatePackage(packageName + i, packageType, jsonManifest);
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
            var jsonManifest = Constants.TEST_PACKAGE_JSON;

            // Arrange
            var createdPkg = this.CreatePackage(packageName, packageType, jsonManifest);
            var createdPkgId = createdPkg.Id;

            var response = this.RetrievePackage(createdPkgId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act
            this.DeletePackage(createdPkgId);

            response = this.RetrievePackage(createdPkgId);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private PackageApiModel CreatePackage(string packageName, PackageType packageType, string content)
        {
            // Arange
            var request = this.CreatePackageRequestWithPackageModel(packageType, content, packageName);

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
            Assert.Equal(type, uploadedPackage.Type);
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

        private HttpRequest CreateGetRequest(string packageId)
        {
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/packages/" + packageId);
            request.Options.Timeout = TIMEOUT_MS;
            return request;
        }

        private HttpRequest CreatePackageRequestWithPackageModel(PackageType packageType, string packageContent, string packageName)
        {
            var request = new HttpRequest(Constants.CONFIG_ADDRESS + "/packages");

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(packageType.ToString()), PKG_TYPE_PARAM_NAME);

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
