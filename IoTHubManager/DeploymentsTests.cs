// Copyright (c) Microsoft. All rights reserved.

using Helpers.Http;
using System;
using System.Net;
using IoTHubManager.Models;
using Xunit;
using Newtonsoft.Json;

namespace IoTHubManager
{
    [Collection("IoTHub Manager Tests")]
    public class DeploymentsTests
    {
        private readonly string edgePackageContent;
        private readonly string admPackageContent;
        private readonly string deviceGroupId;
        private readonly string deviceGroupQuery;
        private readonly int priority;
        private readonly HttpRequestWrapper Request;

        public DeploymentsTests()
        {
            this.edgePackageContent = Constants.EDGE_PACKAGE_JSON;
            this.admPackageContent = Constants.ADM_PACKAGE_JSON;
            this.deviceGroupId = "deviceGroupId";
            this.deviceGroupQuery = "[{\"key\":\"Properties.Reported.Type\"," + 
                                    "\"operator\":\"EQ\",\"value\":\"Elevator\"}]";
            this.priority = 10;
            this.Request = new HttpRequestWrapper(Constants.IOT_HUB_ADDRESS, Constants.Urls.DEPLOYMENTS_PATH);
        }

        // Create deployment
        [Theory, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldCreateDeployment(bool isEdgeDeployment)
        {
            var deploymentName = "depName";
            var deviceGroupName = "dvcGroupName";
            var packageName = "packageName";
            var response = this.CreateDeployment(
                deploymentName,
                deviceGroupName,
                isEdgeDeployment,
                packageName);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var deployment = JsonConvert.DeserializeObject<DeploymentApiModel>(response.Content);
            Assert.NotNull(deployment.DeploymentId);
            Assert.Equal(deploymentName, deployment.Name);
            Assert.Equal(this.deviceGroupId, deployment.DeviceGroupId);
            Assert.Equal(deviceGroupName, deployment.DeviceGroupName);
            Assert.Equal(packageName, deployment.PackageName);
            Assert.Equal(this.priority, deployment.Priority);

            var elapsed = DateTime.UtcNow - deployment.CreatedDateTimeUtc;
            Assert.True(elapsed.TotalSeconds <= 5);

            this.DeleteDeployment(deployment.DeploymentId);
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldRetrieveCreatedDeployment()
        {
            var deploymentName = "test-retrieve-deployment";
            var response = this.CreateDeployment(deploymentName);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var deployment = JsonConvert.DeserializeObject<DeploymentApiModel>(response.Content);

            var getResponse = this.Request.Get(deployment.DeploymentId, string.Empty);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            deployment = JsonConvert.DeserializeObject<DeploymentApiModel>(getResponse.Content);
            Assert.NotNull(deployment.DeploymentId);
            Assert.Equal(deploymentName, deployment.Name);
            Assert.Equal(this.deviceGroupId, deployment.DeviceGroupId);
            Assert.Equal(this.priority, deployment.Priority);

            var elapsed = DateTime.UtcNow - deployment.CreatedDateTimeUtc;
            Assert.True(elapsed.TotalSeconds <= 5);

            this.DeleteDeployment(deployment.DeploymentId);
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldRetrieveAllCreatedDeployments()
        {
            // Arrange, create 3 deployments
            var deploymentName = "test-retrieve-all-deployments";
            var dvcGroup = "dvcGroup";
            var packageName = "packageName";
            var numDeployments = 3;
            for(int i = 0; i < numDeployments; i++) 
            {
                var response = this.CreateDeployment(
                    deploymentName + i,
                    dvcGroup + i,
                    false,
                    packageName + i);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            // Act retrieve all
            var getResponse = this.Request.Get();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            // Assert each deployment exists and no extras.
            var deployments = JsonConvert.DeserializeObject<DeploymentListApiModel>(getResponse.Content);
            int deploymentCount = 0;
            foreach(DeploymentApiModel deployment in deployments.Items)
            {
                if(!deployment.Name.StartsWith(deploymentName))
                {
                    continue;
                }

                this.DeleteDeployment(deployment.DeploymentId);
                Assert.StartsWith(dvcGroup, deployment.DeviceGroupName);
                Assert.StartsWith(packageName, deployment.PackageName);
                Assert.Equal(this.deviceGroupId, deployment.DeviceGroupId);
                deploymentCount++;
            }

            Assert.Equal(deploymentCount, numDeployments);
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ShouldDeleteDeployment()
        {
            // Arrange - Create package to be deleted, and verify it exists
            var deploymentName = "test-delete-deployment";
            var response = this.CreateDeployment(deploymentName);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var deployment = JsonConvert.DeserializeObject<DeploymentApiModel>(response.Content);

            var getResponse = this.Request.Get(deployment.DeploymentId, string.Empty);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            // Act - Delete package
            var deleteResponse = this.DeleteDeployment(deployment.DeploymentId);
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            // Assert - Verify package no longer exists
            getResponse = this.Request.Get(deployment.DeploymentId, string.Empty);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        private IHttpResponse CreateDeployment(
            string name,
            string deviceGroupName = "dvcGroup",
            bool isEdgeDeployment = false,
            string packageName = "")
        {
            var input = new DeploymentApiModel
            {
                Name = name,
                DeviceGroupId = this.deviceGroupId,
                DeviceGroupName = deviceGroupName,
                DeviceGroupQuery = this.deviceGroupQuery,
                PackageContent = isEdgeDeployment ? this.edgePackageContent : this.admPackageContent,
                PackageName = packageName,
                Priority = this.priority,
                PackageType = isEdgeDeployment ? PackageType.EdgeManifest : 
                                    PackageType.DeviceConfiguration,
                ConfigType = isEdgeDeployment ? null : ConfigType.FirmwareUpdate.ToString()
            };

            return this.Request.Post(JsonConvert.SerializeObject(input, Formatting.None));
        }

        private IHttpResponse DeleteDeployment(string deploymentId)
        {
            return this.Request.Delete(deploymentId);
        }
    }
}
