 using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using Helpers.Http;
using Xunit;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    public class CreateJobsTest
    {
        private readonly IHttpClient httpClient;
        private readonly string simulatedDeviceId;
        private readonly string simulatedFaultyDeviceId;

        public CreateJobsTest()
        {
            this.httpClient = new HttpClient();
            SimulatedDevices simulatedDevices = new SimulatedDevices();
            simulatedDeviceId = Constants.SIMULATED_DEVICE + "." + simulatedDevices.healthyDeviceNo.ToString();
            simulatedFaultyDeviceId = Constants.SIMULATED_FAULTY_DEVICE + "." + simulatedDevices.faultyDeviceNo.ToString();
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void Creates_Tags_Failson_InCorrectORMissingTags()
        {
            var tagJobResponse = CreateTags();
            Assert.Equal(HttpStatusCode.OK, tagJobResponse.StatusCode);

            var tagJob = JObject.Parse(tagJobResponse.Content);
            Assert.Equal<int>(4, tagJob["status"].ToObject<int>());
            Assert.Equal<int>(7, tagJob["Type"].ToObject<int>());

            var tagJobStatus = getTagJobStatus(tagJob["JobId"].ToString());
            Assert.Equal<int>(4, tagJob["status"].ToObject<int>());
            Assert.Equal<int>(3, tagJob["Type"].ToObject<int>());
            Assert.Equal<int>(2, tagJob["ResultStatistics"]["DeviceCount"].ToObject<int>());
        }

        private JObject getTagJobStatus(string JobId)
        {
            IHttpResponse jobStatusResponse = Request(JobId, null);
            Assert.Equal(HttpStatusCode.OK, jobStatusResponse.StatusCode);
            return JObject.Parse(jobStatusResponse.Content);
        }

        private IHttpResponse CreateTags()
        {
            var TAGS = System.IO.File.ReadAllText(Constants.PATH.TAGS_FILE);
            string jobId = Guid.NewGuid().ToString();

            TAGS = TAGS.Replace("{JobId}", jobId)
                       .Replace("{DeviceId}", simulatedDeviceId)
                       .Replace("{FaultyDeviceId}", simulatedFaultyDeviceId);
            return Request(TAGS);
        }

       

        private IHttpResponse Request(string content)
        {
            var request = new HttpRequest(Constants.Urls.IOTHUB_ADDRESS + Constants.Urls.JOBS_PATH);
            request.SetContent(content);
            return this.httpClient.PostAsync(request).Result;
        }

        private IHttpResponse Request(string path, string query)
        {
            string uri = Constants.Urls.IOTHUB_ADDRESS + Constants.Urls.JOBS_PATH;
            if (!String.IsNullOrEmpty(path))
            {
                uri += path;
            }
            if (!String.IsNullOrEmpty(query))
            {
                uri += query;
            }
            var request = new HttpRequest(uri);
            return this.httpClient.GetAsync(request).Result;
        }
    }


}
