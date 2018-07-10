 using System;
using System.Threading;
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
            Simulation simulation = Simulation.GetSimulation();

            simulatedDeviceId = Constants.SimulatedDevices.SIMULATED_DEVICE + "." + simulation.healthyDeviceNo.ToString();
            simulatedFaultyDeviceId = Constants.SimulatedDevices.SIMULATED_FAULTY_DEVICE + "." + simulation.faultyDeviceNo.ToString();
        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void Creates_Tags_On_Simulated_Devices()
        {
            
            var tagJobResponse = CreateTags();
            Assert.Equal(HttpStatusCode.OK, tagJobResponse.StatusCode);

            var tagJob = JObject.Parse(tagJobResponse.Content);
            Assert.Equal<int>(7, tagJob["Status"].ToObject<int>());
            Assert.Equal<int>(4, tagJob["Type"].ToObject<int>());

            var tagJobStatus = GetJobStatus(tagJob["JobId"].ToString());

            for (int trials = 0;  trials < Constants.Jobs.MAX_TRIALS; trials++ )
            {
                if (3 == tagJobStatus["Status"].ToObject<int>())
                {
                    break;
                }
                Thread.Sleep(Constants.Jobs.WAIT);
                tagJobStatus = GetJobStatus(tagJob["JobId"].ToString());
            }

            Assert.Equal<int>(3, tagJobStatus["Status"].ToObject<int>());   //Assert to see if Last try yielded correct status.
            Assert.Equal<int>(4, tagJobStatus["Type"].ToObject<int>());

        }

        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void Run_Method_On_Simulated_Devices()
        {
            TestRunMethod(Constants.Path.REBOOT_METHOD_FILE);               //Method with NO Args
            TestRunMethod(Constants.Path.FIRMWAREUPDATE_METHOD_FILE);       //Method with Args
        }

        private void TestRunMethod(string methodFile)
        {
            var methodJobResp = RunMethods(methodFile);
            Assert.Equal(HttpStatusCode.OK, methodJobResp.StatusCode);

            var methodJob = JObject.Parse(methodJobResp.Content);
            Assert.Equal<int>(7, methodJob["Status"].ToObject<int>());
            Assert.Equal<int>(3, methodJob["Type"].ToObject<int>());

            var methodJobStatus = ReTry_GetJobStatus(methodJob["JobId"].ToString());

            Assert.Equal<int>(3, methodJobStatus["Status"].ToObject<int>()); //Assert to see if Last try yielded correct status.
            Assert.Equal<int>(3, methodJobStatus["Type"].ToObject<int>());

        }


        private IHttpResponse CreateTags()
        {
            var TAGS = System.IO.File.ReadAllText(Constants.Path.TAGS_FILE);
            string jobId = Guid.NewGuid().ToString();

            TAGS = TAGS.Replace(Constants.TemplateKeys.JOB_ID, jobId)
                       .Replace(Constants.TemplateKeys.DEVICE_ID, simulatedDeviceId)
                       .Replace(Constants.TemplateKeys.FAULTY_DEVICE_ID, simulatedFaultyDeviceId);
            return Request(TAGS);
        }

        private IHttpResponse RunMethods(string methodFile)
        {
            var METHODS = System.IO.File.ReadAllText(methodFile);
            string jobId = Guid.NewGuid().ToString();
            
            METHODS = METHODS.Replace(Constants.TemplateKeys.JOB_ID, jobId)
                             .Replace(Constants.TemplateKeys.DEVICE_ID, simulatedDeviceId);
            return Request(METHODS);
        }

        private JObject GetJobStatus(string JobId)
        {
            IHttpResponse jobStatusResponse = Request(JobId, null);
            Assert.Equal(HttpStatusCode.OK, jobStatusResponse.StatusCode);
            return JObject.Parse(jobStatusResponse.Content);
        }

        private JObject ReTry_GetJobStatus(string jobId)
        {
            var jobStatus = GetJobStatus(jobId);

            for (int trials = 0; trials < Constants.Jobs.MAX_TRIALS; trials++)
            {
                if (3 == jobStatus["Status"].ToObject<int>())
                {
                    break;
                }
                Thread.Sleep(Constants.Jobs.WAIT);
                jobStatus = GetJobStatus(jobId);
            }

            return jobStatus;
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
