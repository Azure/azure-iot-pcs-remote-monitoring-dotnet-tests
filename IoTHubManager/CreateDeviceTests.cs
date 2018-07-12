// Copyright (c) Microsoft. All rights reserved.
using System;
using System.Net;
using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{

    [Collection("IoTHub Manager Tests")]
    public class CreateDeviceTest : IDisposable
    {
        private readonly HttpRequestWrapper Request;
        //Device Templates
        private readonly string DEVICE_TEMPLATE_AUTO_GEN_AUTH;
        private readonly string DEVICE_TEMPLATE_SYMMETRIC_AUTH;
        private readonly string DEVICE_TEMPLATE_X509_AUTH;
        private string deviceId = "";

        public CreateDeviceTest()
        {
            //Create request wrapper object for interacting with iothub manager microservices for CRUD on devices
            this.Request = new HttpRequestWrapper(Constants.IOT_HUB_ADDRESS, Constants.Urls.DEVICE_PATH);

            //Fetch different device templates
            this.DEVICE_TEMPLATE_AUTO_GEN_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_AUTO_GEN_AUTH);
            this.DEVICE_TEMPLATE_SYMMETRIC_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_SYMMETRIC_AUTH);
            this.DEVICE_TEMPLATE_X509_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_X509_AUTH);                     
        }

        /**
         * Creates a device with auto generated id and symmetric auth.
         * Verifies that the resultant device has system-generated
         * id as well as auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfEmptyIDAndAuthPassed()
        {
            // Arrange
            // DeviceId must be empty to be auto generated.
            var device = this.DEVICE_TEMPLATE_AUTO_GEN_AUTH.Replace(Constants.Keys.DEVICE_ID, "");

            // Act
            var response = this.Request.Post(device);
            var createdDevice = JObject.Parse(response.Content);
            deviceId = createdDevice["Id"].ToString();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helper.Device.AssertCommonDeviceProperties(null, createdDevice);
            Helper.Device.AssertSymmetricAuthentication(null, null, createdDevice);
            Helper.Device.CheckIfDeviceExists(this.Request, createdDevice);
        }

        /**
         * Creates a device with Custom id and auto generated auth.
         * Verifies that the resultant device has correct id and system
         * generated symmetric auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfCustomIDAndEmptyAuthPassed()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            var device = this.DEVICE_TEMPLATE_AUTO_GEN_AUTH.Replace(Constants.Keys.DEVICE_ID, id);

            // Act
            var response = this.Request.Post(device);
            var createdDevice = JObject.Parse(response.Content);
            deviceId = createdDevice["Id"].ToString();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helper.Device.AssertCommonDeviceProperties(id, createdDevice);
            Helper.Device.AssertSymmetricAuthentication(null, null, createdDevice);
            Helper.Device.CheckIfDeviceExists(this.Request, createdDevice);
        }

        /**
         * Creates a device with custom id and symmetric auth
         * credentails. Verifies that the resultant device has 
         * correct id and auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfCustomIDAndAuthPassed()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            string primaryKey = Guid.NewGuid().ToString("N");
            string secondaryKey = Guid.NewGuid().ToString("N");
            string device = this.DEVICE_TEMPLATE_SYMMETRIC_AUTH.Replace(Constants.Keys.DEVICE_ID, id)
                                                          .Replace(Constants.Keys.PRIMARY_KEY, primaryKey)
                                                          .Replace(Constants.Keys.SECONDARY_KEY, secondaryKey);
            // Act
            var response = this.Request.Post(device);
            var createdDevice = JObject.Parse(response.Content);
            deviceId = createdDevice["Id"].ToString();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helper.Device.AssertCommonDeviceProperties(id, createdDevice);
            Helper.Device.AssertSymmetricAuthentication(primaryKey, secondaryKey, createdDevice);
            Helper.Device.CheckIfDeviceExists(this.Request, createdDevice);
        }

        /**
         * Creates a device with auto generated id and custom 
         * symmetric auth credentials. Verifies that the resultant device 
         * has system-generated id and correct auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfEmptyIdAndCustomAuthPassed()
        {
            //Arrange
            string primaryKey = Guid.NewGuid().ToString("N");
            string secondaryKey = Guid.NewGuid().ToString("N");
            // DeviceId must be empty to be auto generated.
            string device = this.DEVICE_TEMPLATE_SYMMETRIC_AUTH.Replace(Constants.Keys.DEVICE_ID, "")
                                                          .Replace(Constants.Keys.PRIMARY_KEY, primaryKey)
                                                          .Replace(Constants.Keys.SECONDARY_KEY, secondaryKey);
            //Act
            var response = this.Request.Post(device);
            var createdDevice = JObject.Parse(response.Content);
            deviceId = createdDevice["Id"].ToString();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helper.Device.AssertCommonDeviceProperties(null, createdDevice);
            Helper.Device.AssertSymmetricAuthentication(primaryKey, secondaryKey, createdDevice);
            Helper.Device.CheckIfDeviceExists(this.Request, createdDevice);
        }

        /**
         * Creates a device with custom id and X509 auth credentails.
         * Verifies that the resultant device has correct id
         * and auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfCustomIdAndX509AuthPassed()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            string primaryThumbprint = Helper.Device.GenerateNewThumbPrint();
            string secondaryThumbprint = Helper.Device.GenerateNewThumbPrint();

            string device = DEVICE_TEMPLATE_X509_AUTH.Replace(Constants.Keys.DEVICE_ID, id)
                                                     .Replace(Constants.Keys.PRIMARY_TH, primaryThumbprint)
                                                     .Replace(Constants.Keys.SECONDARY_TH, secondaryThumbprint);
            // Act
            var response = this.Request.Post(device);
            var createdDevice = JObject.Parse(response.Content);
            deviceId = createdDevice["Id"].ToString();

            // Asserts 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helper.Device.AssertCommonDeviceProperties(id, createdDevice);
            Helper.Device.AssertX509Authentication(primaryThumbprint, secondaryThumbprint, createdDevice);
            Helper.Device.CheckIfDeviceExists(this.Request, createdDevice);
        }

        /**
         * Creates a device with auto generated id and custom 
         * X509 auth credentials. Verifies that the resultant device 
         * has system-generated id and correct auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfEmptyIdAndX509AuthPassed()
        {
            // Arrange
            string primaryThumbprint = Helper.Device.GenerateNewThumbPrint();
            string secondaryThumbprint = Helper.Device.GenerateNewThumbPrint();
            // DeviceId must be empty to be auto generated.
            string device = this.DEVICE_TEMPLATE_X509_AUTH.Replace(Constants.Keys.DEVICE_ID, "")
                                                     .Replace(Constants.Keys.PRIMARY_TH, primaryThumbprint)
                                                     .Replace(Constants.Keys.SECONDARY_TH, secondaryThumbprint);
            // Act
            var response = this.Request.Post(device);
            var createdDevice = JObject.Parse(response.Content);
            deviceId = createdDevice["Id"].ToString();

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helper.Device.AssertCommonDeviceProperties(null, createdDevice);
            Helper.Device.AssertX509Authentication(primaryThumbprint, secondaryThumbprint, createdDevice);
            Helper.Device.CheckIfDeviceExists(this.Request, createdDevice);
        }

        public void Dispose()
        {
            var response = Request.Delete(deviceId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}