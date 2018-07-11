// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using System;
using System.Text;
using Helpers.Http;
using Xunit;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    [Collection("IoTHub Manager Tests")]
    public class CreateDeviceTest
    {

        internal HttpRequestWrapper Request;

        private string DEVICE_TEMPLATE_AUTO_GEN_AUTH;
        private string DEVICE_TEMPLATE_SYMMETRIC_AUTH;
        private string DEVICE_TEMPLATE_X509_AUTH;

        public CreateDeviceTest()
        {
        
            //Create request wrapper object for interacting with iothub manager microservices for CRUD on devices
            this.Request = new HttpRequestWrapper(Constants.Urls.IOTHUB_ADDRESS, Constants.Urls.DEVICE_PATH);

            //Fetch different device templates
            DEVICE_TEMPLATE_AUTO_GEN_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_AUTO_GEN_AUTH);
            DEVICE_TEMPLATE_SYMMETRIC_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_SYMMETRIC_AUTH);
            DEVICE_TEMPLATE_X509_AUTH = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_X509_AUTH);                     
        }

        /// <summary>
        /// Integration test using a real HTTP instance.
        /// Tests for creation of devices with all permutation of AUTH and Id creation
        /// </summary>
        

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
            var device = DEVICE_TEMPLATE_AUTO_GEN_AUTH.Replace(Constants.Keys.DEVICE_ID, "");

            // Act
            var response = Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString();
            string primaryKey = authentication["PrimaryKey"].ToString();
            string secondaryKey = authentication["SecondaryKey"].ToString();


            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.False(string.IsNullOrEmpty(createdDeviceId));
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
            Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);
            Assert.False(string.IsNullOrEmpty(primaryKey));
            Assert.False(string.IsNullOrEmpty(secondaryKey));
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
            var device = DEVICE_TEMPLATE_AUTO_GEN_AUTH.Replace(Constants.Keys.DEVICE_ID, id);

            // Act
            var response = Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString();
            string primaryKey = authentication["PrimaryKey"].ToString();
            string secondaryKey = authentication["SecondaryKey"].ToString();

            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.Equal(createdDeviceId, id);
            Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);
            Assert.False(string.IsNullOrEmpty(primaryKey));
            Assert.False(string.IsNullOrEmpty(secondaryKey));
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
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
            string primaryKey = Guid.NewGuid().ToString("n");
            string secondaryKey = Guid.NewGuid().ToString("n");
            string device = DEVICE_TEMPLATE_SYMMETRIC_AUTH.Replace(Constants.Keys.DEVICE_ID, id)
                                                          .Replace(Constants.Keys.PRIMARY_KEY, primaryKey)
                                                          .Replace(Constants.Keys.SECONDARY_KEY, secondaryKey);
            // Act
            var response = Request.Post(device);


            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString();
            string createdPrimaryKey = authentication["PrimaryKey"].ToString();
            string createdSecondaryKey = authentication["SecondaryKey"].ToString();

            // Assert device ID and auth are not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.Equal(createdDeviceId, id);
            Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);
            Assert.Equal(primaryKey, createdPrimaryKey);
            Assert.Equal(secondaryKey, createdSecondaryKey);
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());

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
            string primaryKey = Guid.NewGuid().ToString("N"),
            string secondaryKey = Guid.NewGuid().ToString("N");
            // DeviceId must be empty to be auto generated.
            string device = DEVICE_TEMPLATE_SYMMETRIC_AUTH.Replace(Constants.Keys.DEVICE_ID, "")
                                                          .Replace(Constants.Keys.PRIMARY_KEY, primaryKey)
                                                          .Replace(Constants.Keys.SECONDARY_KEY, secondaryKey);

            //Act
            var response = Request.Post(device);


            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString();
            string createdPrimaryKey = authentication["PrimaryKey"].ToString();
            string createdSecondaryKey = authentication["SecondaryKey"].ToString();



            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.False(string.IsNullOrEmpty(createdDeviceId));
            Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);
            Assert.Equal(primaryKey, createdPrimaryKey);
            Assert.Equal(secondaryKey, createdSecondaryKey);
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
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
            string primaryThumbprint = Helpers.GenerateNewThumbPrint();
            string secondaryThumbprint = Helpers.GenerateNewThumbPrint();

            string device = DEVICE_TEMPLATE_X509_AUTH.Replace(Constants.Keys.DEVICE_ID, id)
                                                     .Replace(Constants.Keys.PRIMARY_TH, primaryThumbprint)
                                                     .Replace(Constants.Keys.SECONDARY_TH, secondaryThumbprint);

            // Act
            var response = Request.Post(device);

            // Asserts 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString(),
                   createdPrimaryThumbprint = authentication["PrimaryThumbprint"].ToString(),
                   createdSecondaryThumbprint = authentication["SecondaryThumbprint"].ToString();


            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.Equal(createdDeviceId, id);
            Assert.Equal(Constants.Auth.X509, authentication["AuthenticationType"]);
            Assert.Equal(primaryThumbprint, createdPrimaryThumbprint);
            Assert.Equal(secondaryThumbprint, createdSecondaryThumbprint);
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
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
            string primaryThumbprint = Helpers.GenerateNewThumbPrint();
            string secondaryThumbprint = Helpers.GenerateNewThumbPrint();
            // DeviceId must be empty to be auto generated.
            string device = DEVICE_TEMPLATE_X509_AUTH.Replace(Constants.Keys.DEVICE_ID, "")
                                                     .Replace(Constants.Keys.PRIMARY_TH, primaryThumbprint)
                                                     .Replace(Constants.Keys.SECONDARY_TH, secondaryThumbprint);

            // Act
            var response = Request.Post(device);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var createdDevice = JObject.Parse(response.Content);
            var authentication = createdDevice["Authentication"];
            string createdDeviceId = createdDevice["Id"].ToString();
            string createdPrimaryThumbprint = authentication["PrimaryThumbprint"].ToString();
            string createdSecondaryThumbprint = authentication["SecondaryThumbprint"].ToString();


            // Assert device ID and auth or not null OR empty and
            // other required properties are set. Also check auth type.
            Assert.False(string.IsNullOrEmpty(createdDeviceId));
            Assert.Equal(Constants.Auth.X509, authentication["AuthenticationType"]);
            Assert.Equal(primaryThumbprint, createdPrimaryThumbprint);
            Assert.Equal(secondaryThumbprint, createdSecondaryThumbprint);
            Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
            Assert.True(createdDevice["Enabled"].ToObject<bool>());
        }
    }
}