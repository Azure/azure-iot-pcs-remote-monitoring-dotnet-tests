 using System;
using System.Threading;
using System.Net;
using Helpers.Http;
using Xunit;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    public class CreateJobsTest
    {
        private readonly HttpRequestWrapper Request;

        private readonly string simulatedDeviceId;
        private readonly string simulatedFaultyDeviceId;

        /**
         * Initialises simulated devices usewd for the tests
         */
        public CreateJobsTest()
        {
            this.Request = new HttpRequestWrapper(Constants.Urls.IOTHUB_ADDRESS, Constants.Urls.JOBS_PATH);

            Simulation simulation = Simulation.GetSimulation();
            simulatedDeviceId = Constants.SimulatedDevices.SIMULATED_DEVICE + "." + simulation.healthyDeviceNo.ToString();
            simulatedFaultyDeviceId = Constants.SimulatedDevices.SIMULATED_FAULTY_DEVICE + "." + simulation.faultyDeviceNo.ToString();
        }

        /**
         * Creates Job for tagging on devices and 
         * checks the job status using polling 
         * mechanism to verify job completion.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void Creates_Tags_On_Simulated_Devices()
        {
            
            var tagJobResponse = CreateTags();
            Assert.Equal(HttpStatusCode.OK, tagJobResponse.StatusCode);

            var tagJob = JObject.Parse(tagJobResponse.Content);
            Assert.Equal<int>(Constants.Jobs.JOB_IN_PROGRESS, tagJob["Status"].ToObject<int>());
            Assert.Equal<int>(Constants.Jobs.TAG_JOB, tagJob["Type"].ToObject<int>());

            var tagJobStatus = ReTry_GetJobStatus(tagJob["JobId"].ToString());

            Assert.Equal<int>(Constants.Jobs.JOB_COMPLETED, tagJobStatus["Status"].ToObject<int>());   //Assert to see if Last try yielded correct status.
            Assert.Equal<int>(Constants.Jobs.TAG_JOB, tagJobStatus["Type"].ToObject<int>());

        }

        /**
         * Creates Job for running methods on 
         * devices and checks the job status 
         * using polling mechanism to verify 
         * job completion.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void Run_Method_On_Simulated_Devices()
        {
            TestRunMethod(Constants.Path.REBOOT_METHOD_FILE);               //Method with NO Args
            TestRunMethod(Constants.Path.FIRMWAREUPDATE_METHOD_FILE);       //Method with Args
        }

        /**
         * Helper for (Run_Method_On_Simulated_Devices)
         */
        private void TestRunMethod(string methodFile)
        {
            var methodJobResp = RunMethods(methodFile);
            Assert.Equal(HttpStatusCode.OK, methodJobResp.StatusCode);

            var methodJob = JObject.Parse(methodJobResp.Content);
            Assert.Equal<int>(Constants.Jobs.JOB_IN_PROGRESS, methodJob["Status"].ToObject<int>());
            Assert.Equal<int>(Constants.Jobs.METHOD_JOB, methodJob["Type"].ToObject<int>());

            var methodJobStatus = ReTry_GetJobStatus(methodJob["JobId"].ToString());

            Assert.Equal<int>(Constants.Jobs.JOB_COMPLETED, methodJobStatus["Status"].ToObject<int>()); //Assert to see if Last try yielded correct status.
            Assert.Equal<int>(Constants.Jobs.METHOD_JOB, methodJobStatus["Type"].ToObject<int>());

        }


        /**
         * Creates Job for reconfiguring 
         * devices and checks the job status 
         * using polling mechanism to verify 
         * job completion.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ReConfigure_Simulated_Devices()
        {
            var configJobResponse = ReconfigureDevice();
            Assert.Equal(HttpStatusCode.OK, configJobResponse.StatusCode);

            var configJob = JObject.Parse(configJobResponse.Content);
            Assert.Equal<int>(Constants.Jobs.JOB_IN_PROGRESS, configJob["Status"].ToObject<int>());
            Assert.Equal<int>(Constants.Jobs.RECONFIGURE_JOB, configJob["Type"].ToObject<int>());

            var configJobStatus = ReTry_GetJobStatus(configJob["JobId"].ToString());

            Assert.Equal<int>(Constants.Jobs.JOB_COMPLETED, configJobStatus["Status"].ToObject<int>());   //Assert to see if Last try yielded correct status.
            Assert.Equal<int>(Constants.Jobs.RECONFIGURE_JOB, configJobStatus["Type"].ToObject<int>());

        }



        //Helper methods
        /**
         * Creates tags by reading tags template and replacing the template keys.
         */
        private IHttpResponse CreateTags()
        {
            var TAGS = System.IO.File.ReadAllText(Constants.Path.TAGS_FILE);
            string jobId = Guid.NewGuid().ToString();

            TAGS = TAGS.Replace(Constants.TemplateKeys.JOB_ID, jobId)
                       .Replace(Constants.TemplateKeys.DEVICE_ID, simulatedDeviceId)
                       .Replace(Constants.TemplateKeys.FAULTY_DEVICE_ID, simulatedFaultyDeviceId);
            return Request.Post(TAGS);
        }

        /**
         * Runs methods by reading the template and replacing the template keys.
         */
        private IHttpResponse RunMethods(string methodFile)
        {
            var METHODS = System.IO.File.ReadAllText(methodFile);
            string jobId = Guid.NewGuid().ToString();
            
            METHODS = METHODS.Replace(Constants.TemplateKeys.JOB_ID, jobId)
                             .Replace(Constants.TemplateKeys.DEVICE_ID, simulatedDeviceId);
            return Request.Post(METHODS);
        }

        /**
         * Reconfigure devices by reading the template and replacing the template keys.
         */
        private IHttpResponse ReconfigureDevice()
        {
            var CONFIG = System.IO.File.ReadAllText(Constants.Path.RECONFIGURE_DEVICE_FILE);
            string jobId = Guid.NewGuid().ToString();

            CONFIG = CONFIG.Replace(Constants.TemplateKeys.JOB_ID, jobId)
                           .Replace(Constants.TemplateKeys.DEVICE_ID, simulatedDeviceId);
            return Request.Post(CONFIG);
        }

        //Helper methods for fetching (and retrying) the current Job Status.
        /**
         * Gets job status using job id.
         */
        private JObject GetJobStatus(string JobId)
        {
            IHttpResponse jobStatusResponse = Request.Get(JobId, null);
            Assert.Equal(HttpStatusCode.OK, jobStatusResponse.StatusCode);
            return JObject.Parse(jobStatusResponse.Content);
        }

        /**
         * Monitor job status using polling (re-try) mechanism 
         */
        private JObject ReTry_GetJobStatus(string jobId)
        {
            var jobStatus = GetJobStatus(jobId);

            for (int trials = 0; trials < Constants.Jobs.MAX_TRIALS; trials++)
            {
                if (Constants.Jobs.JOB_COMPLETED == jobStatus["Status"].ToObject<int>())
                {
                    break;
                }
                Thread.Sleep(Constants.Jobs.WAIT);
                jobStatus = GetJobStatus(jobId);
            }

            return jobStatus;
        }
    }
}
