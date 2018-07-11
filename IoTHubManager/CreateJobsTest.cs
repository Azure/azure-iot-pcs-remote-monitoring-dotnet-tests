using System;
using System.Net;
using Helpers.Http;
using Xunit;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    [Collection("IoTHub Manager Tests")]
    public class CreateJobsTest
    {
        private readonly HttpRequestWrapper Request;
        private readonly string simulatedDeviceId;
        private readonly string simulatedFaultyDeviceId;


        /**
         * Initialises simulated devices used for the tests
         */
        public CreateJobsTest()
        {
            Console.WriteLine("Running CreateJobsTest Tests");
            this.Request = new HttpRequestWrapper(Constants.Urls.IOTHUB_ADDRESS, Constants.Urls.JOBS_PATH);
            Console.WriteLine("Running CreateJobsTest Tests :: HttpRequestWrapper initialzed");
            Simulation simulation = Simulation.GetSimulation();
            simulatedDeviceId = Constants.SimulatedDevices.SIMULATED_DEVICE + "." + simulation.healthyDeviceNo.ToString();
            simulatedFaultyDeviceId = Constants.SimulatedDevices.SIMULATED_FAULTY_DEVICE + "." + simulation.faultyDeviceNo.ToString();
            Console.WriteLine("Starting CreateJobsTest Tests");
        }


        /**
         * Creates Job for tagging on devices and 
         * checks the job status using polling 
         * mechanism to verify job completion.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void Creates_Tags_On_Simulated_Devices()
        {
            Console.WriteLine("Running 1st CreateJobsTest Tests");
            var tagJobResponse = CreateTags();
            Assert.Equal(HttpStatusCode.OK, tagJobResponse.StatusCode);

            var tagJob = JObject.Parse(tagJobResponse.Content);
            Assert.Equal<int>(Constants.Jobs.JOB_IN_PROGRESS, tagJob["Status"].ToObject<int>());
            Assert.Equal<int>(Constants.Jobs.TAG_JOB, tagJob["Type"].ToObject<int>());

            var tagJobStatus = Helpers.ReTry_GetJobStatus(Request, tagJob["JobId"].ToString());

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

            var methodJobStatus = Helpers.ReTry_GetJobStatus(Request, methodJob["JobId"].ToString());

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

            var configJobStatus = Helpers.ReTry_GetJobStatus(Request, configJob["JobId"].ToString());

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

            TAGS = TAGS.Replace(Constants.Keys.JOB_ID, jobId)
                       .Replace(Constants.Keys.DEVICE_ID, simulatedDeviceId)
                       .Replace(Constants.Keys.FAULTY_DEVICE_ID, simulatedFaultyDeviceId);
            return Request.Post(TAGS);
        }

        /**
         * Runs methods by reading the template and replacing the template keys.
         */
        private IHttpResponse RunMethods(string methodFile)
        {
            var METHODS = System.IO.File.ReadAllText(methodFile);
            string jobId = Guid.NewGuid().ToString();
            
            METHODS = METHODS.Replace(Constants.Keys.JOB_ID, jobId)
                             .Replace(Constants.Keys.DEVICE_ID, simulatedDeviceId);
            return Request.Post(METHODS);
        }

        /**
         * Reconfigure devices by reading the template and replacing the template keys.
         */
        private IHttpResponse ReconfigureDevice()
        {
            var CONFIG = System.IO.File.ReadAllText(Constants.Path.RECONFIGURE_DEVICE_FILE);
            string jobId = Guid.NewGuid().ToString();

            CONFIG = CONFIG.Replace(Constants.Keys.JOB_ID, jobId)
                           .Replace(Constants.Keys.DEVICE_ID, simulatedDeviceId);
            return Request.Post(CONFIG);
        }

    }
}
