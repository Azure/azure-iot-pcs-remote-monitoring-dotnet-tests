// Copyright (c) Microsoft. All rights reserved.
using System;
using System.Net;
using Helpers.Http;
using Xunit;

namespace IoTHubManager
{
    
    [Collection("IoTHub Manager Tests")]
    public class CreateDeviceTest
    {
        private readonly HttpRequestWrapper Request;
        //Device Templates
        private readonly string DEVICE_TEMPLATE_AUTO_GEN_AUTH;
        private readonly string DEVICE_TEMPLATE_SYMMETRIC_AUTH;
        private readonly string DEVICE_TEMPLATE_X509_AUTH;

        public CreateDeviceTest()
        {
            //Create request wrapper object for interacting with iothub manager microservices for CRUD on devices
            this.Request = new HttpRequestWrapper(Constants.Urls.IOTHUB_ADDRESS, Constants.Urls.DEVICE_PATH);

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
        public void DeviceCreated_IfEmptyIDandAuthPassed()
        {
            // Arrange
            // DeviceId must be empty to be auto generated.
            var device = this.DEVICE_TEMPLATE_AUTO_GEN_AUTH.Replace(Constants.Keys.DEVICE_ID, "");

            // Act
            var response = this.Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helpers.Device.AssertCommonDeviceProperties(null, response);
            Helpers.Device.AssertSymmetricAuthentication(null, null, response);
            Helpers.Device.CheckIfDeviceExists(this.Request, response);
        }

        /**
         * Creates a device with Custom id and auto generated auth.
         * Verifies that the resultant device has correct id and system
         * generated symmetric auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfCustomIDandEmptyAuthPassed()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            var device = this.DEVICE_TEMPLATE_AUTO_GEN_AUTH.Replace(Constants.Keys.DEVICE_ID, id);

            // Act
            var response = this.Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helpers.Device.AssertCommonDeviceProperties(id, response);
            Helpers.Device.AssertSymmetricAuthentication(null, null, response);
            Helpers.Device.CheckIfDeviceExists(this.Request, response);
        }

        /**
         * Creates a device with custom id and symmetric auth
         * credentails. Verifies that the resultant device has 
         * correct id and auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfCustomIDandAuthPassed()
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

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helpers.Device.AssertCommonDeviceProperties(id, response);
            Helpers.Device.AssertSymmetricAuthentication(primaryKey, secondaryKey, response);
            Helpers.Device.CheckIfDeviceExists(this.Request, response);
        }

        /**
         * Creates a device with auto generated id and custom 
         * symmetric auth credentials. Verifies that the resultant device 
         * has system-generated id and correct auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfEmptyIdandCustomAuthPassed()
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

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helpers.Device.AssertCommonDeviceProperties(null, response);
            Helpers.Device.AssertSymmetricAuthentication(primaryKey, secondaryKey, response);
            Helpers.Device.CheckIfDeviceExists(this.Request, response);
        }

        /**
         * Creates a device with custom id and X509 auth credentails.
         * Verifies that the resultant device has correct id
         * and auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfCustomIdandX509AuthPassed()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            string primaryThumbprint = Helpers.Device.GenerateNewThumbPrint();
            string secondaryThumbprint = Helpers.Device.GenerateNewThumbPrint();

            string device = DEVICE_TEMPLATE_X509_AUTH.Replace(Constants.Keys.DEVICE_ID, id)
                                                     .Replace(Constants.Keys.PRIMARY_TH, primaryThumbprint)
                                                     .Replace(Constants.Keys.SECONDARY_TH, secondaryThumbprint);
            // Act
            var response = this.Request.Post(device);

            // Asserts 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helpers.Device.AssertCommonDeviceProperties(id, response);
            Helpers.Device.AssertX509Authentication(primaryThumbprint, secondaryThumbprint, response);
            Helpers.Device.CheckIfDeviceExists(this.Request, response);
        }

        /**
         * Creates a device with auto generated id and custom 
         * X509 auth credentials. Verifies that the resultant device 
         * has system-generated id and correct auth credentails.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceCreated_IfEmptyIdandX509AuthPassed()
        {
            // Arrange
            string primaryThumbprint = Helpers.Device.GenerateNewThumbPrint();
            string secondaryThumbprint = Helpers.Device.GenerateNewThumbPrint();
            // DeviceId must be empty to be auto generated.
            string device = this.DEVICE_TEMPLATE_X509_AUTH.Replace(Constants.Keys.DEVICE_ID, "")
                                                     .Replace(Constants.Keys.PRIMARY_TH, primaryThumbprint)
                                                     .Replace(Constants.Keys.SECONDARY_TH, secondaryThumbprint);
            // Act
            var response = this.Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Helpers.Device.AssertCommonDeviceProperties(null, response);
            Helpers.Device.AssertX509Authentication(primaryThumbprint, secondaryThumbprint, response);
            Helpers.Device.CheckIfDeviceExists(this.Request, response);
        }
    }
}