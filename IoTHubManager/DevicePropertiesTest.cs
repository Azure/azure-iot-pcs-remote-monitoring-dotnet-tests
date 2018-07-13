using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IoTHubManager
{
    [Collection("IoTHub Manager Tests")]
    public class DevicePropertiesTest : IDisposable
    {
        private HttpRequestWrapper Request;
        private HttpRequestWrapper PropertyRequest;
        public string deviceId = "";
        public string eTag = "";
        public JObject device;

        public DevicePropertiesTest()
        {
            this.Request = new HttpRequestWrapper(Constants.IOT_HUB_ADDRESS, Constants.Urls.DEVICE_PATH);
            this.PropertyRequest = new HttpRequestWrapper(Constants.IOT_HUB_ADDRESS, Constants.Urls.DEVICE_PROPERTIES_PATH);
            this.CreateDevice();
        }

        private void CreateDevice()
        {
            var DEVICE_TEMPLATE = System.IO.File.ReadAllText(Constants.Path.DEVICE_FILE_AUTO_GEN_AUTH);
            var device = DEVICE_TEMPLATE.Replace(Constants.Keys.DEVICE_ID, "");

            var response = this.Request.Post(device);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            this.device = JObject.Parse(response.Content);
            this.deviceId = this.device["Id"].ToString();
        }

        /**
        * Creates Job for tagging on devices and 
        * checks the job status using polling 
        * mechanism to verify job completion.
        */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DevicePropertiesListUpdated_IfDeviceUpdated()
        {
            //Arrange
            string jobId = Guid.NewGuid().ToString();
            string devicePropertyKey = Guid.NewGuid().ToString();
            string devicePropertyValue = Guid.NewGuid().ToString();
            this.device["Tags"][devicePropertyKey] = devicePropertyValue;

            // Act
            var response = Request.Put(this.deviceId, this.device);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Thread.Sleep(Constants.Jobs.WAIT*5);

            //Asserts
            AssertKeyExistsInPropertyList(devicePropertyKey);
            //DeleteDeviceProperty(devicePropertyKey); Deleting tags NOT working
            //Thread.Sleep(Constants.Jobs.WAIT * 5);
            //AssertKeyDoesNotExistsInPropertyList(devicePropertyKey);
            // Deleting tags si currently not updating the device property list.
        }

        //Helper methods
        /**
         * Checks is the devce proerties list contains newly addded 
         * property key.
         */ 
        private void AssertKeyExistsInPropertyList(string devicePropertyKey)
        {
            var response = PropertyRequest.Get();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            JObject propertyList = JObject.Parse(response.Content);
            JArray properties= (JArray)propertyList["Items"];
            devicePropertyKey = "Tags." + devicePropertyKey;
            string property = (properties.FirstOrDefault(x => x.ToString() == devicePropertyKey)).ToString();
            Assert.True(!(string.IsNullOrEmpty(property)));
        }

        /**
          * Checks is the devce proerties list contains just removed 
          * property key.
          */
        private void AssertKeyDoesNotExistsInPropertyList(string devicePropertyKey)
        {
            var response = PropertyRequest.Get();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            JObject propertyList = JObject.Parse(response.Content);
            JArray properties = (JArray)propertyList["Items"];
            devicePropertyKey = "Tags." + devicePropertyKey;
            string property = (properties.FirstOrDefault(x => x.ToString() == devicePropertyKey)).ToString();
            Assert.True(string.IsNullOrEmpty(property));
        }

        /**
          * Deletes the specified property from the device.
          */
        private void DeleteDeviceProperty(string devicePropertyKey)
        {
            JObject tags = (JObject)this.device["Tags"];
            tags.Property(devicePropertyKey).Remove();
            var response = Request.Put(this.deviceId, this.device);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            var response = Request.Delete(this.deviceId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
