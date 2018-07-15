// Copyright (c) Microsoft. All rights reserved.
using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Xunit;

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
            this.Request = new HttpRequestWrapper(Constants.IOT_HUB_ADDRESS, Constants.Urls.JOBS_PATH);

            // Get simulated devices required for the tests
            Simulation simulation = Simulation.GetSimulation();
            this.simulatedDeviceId = Constants.SimulatedDevices.SIMULATED_DEVICE + "." + simulation.healthyDeviceNo.ToString();
            this.simulatedFaultyDeviceId = Constants.SimulatedDevices.SIMULATED_FAULTY_DEVICE + "." + simulation.faultyDeviceNo.ToString();
        }

        /**
         * Creates Job for tagging on devices and 
         * checks the job status using polling 
         * mechanism to verify completion.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceDataUpdated_IfTagged()
        {
            // Arrange
            var tagsTemplate = System.IO.File.ReadAllText(Constants.Path.TAGS_FILE);

            string jobId = Guid.NewGuid().ToString();
            tagsTemplate = tagsTemplate.Replace(Constants.Keys.JOB_ID, jobId)
                                       .Replace(Constants.Keys.DEVICE_ID, this.simulatedDeviceId)
                                       .Replace(Constants.Keys.FAULTY_DEVICE_ID, this.simulatedFaultyDeviceId);
            // Act
            var response = Request.Post(tagsTemplate);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var job = JObject.Parse(response.Content);
            Helper.Job.AssertJobwasCompletedSuccessfully(Constants.Jobs.TAG_JOB, Request, job);
            Helper.Job.CheckIfDeviceIsTagged(tagsTemplate, this.simulatedDeviceId);
            Helper.Job.CheckIfDeviceIsTagged(tagsTemplate, this.simulatedFaultyDeviceId);
        }

        /*
         * Creates Job for running methods on 
         * devices and checks the job status 
         * using polling mechanism to verify 
         * completion.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceActedUpon_IfMethodExecuted()
        {
            TestRunMethod(Constants.Path.REBOOT_METHOD_FILE);               //Method with NO Args
            TestRunMethod(Constants.Path.FIRMWAREUPDATE_METHOD_FILE);       //Method with Args
        }

        /*
         * Helper for (Run_Method_On_Simulated_Devices)
         */
        private void TestRunMethod(string methodFile)
        {
            // Arrange
            var methods = System.IO.File.ReadAllText(methodFile);
            string jobId = Guid.NewGuid().ToString();

            methods = methods.Replace(Constants.Keys.JOB_ID, jobId)
                             .Replace(Constants.Keys.DEVICE_ID, this.simulatedDeviceId)
                             .Replace(Constants.Keys.FAULTY_DEVICE_ID, this.simulatedFaultyDeviceId);
            // Act
            var response = Request.Post(methods);
            Console.WriteLine(methods);
            Console.WriteLine(response.Content);
            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var job = JObject.Parse(response.Content);
            Helper.Job.AssertJobwasCompletedSuccessfully(Constants.Jobs.METHOD_JOB, Request, job);
        }

        /**
         * Creates Job for reconfiguring 
         * devices and checks the job status 
         * using polling mechanism to verify 
         * job completion.
         */
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void DeviceUpdated_IfReconfigured()
        {
            // Arrange
            var config = System.IO.File.ReadAllText(Constants.Path.RECONFIGURE_DEVICE_FILE);
            string jobId = Guid.NewGuid().ToString();

            config = config.Replace(Constants.Keys.JOB_ID, jobId)
                           .Replace(Constants.Keys.DEVICE_ID, simulatedDeviceId);
            // Act
            var response = Request.Post(config);

            // Asserts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var job = JObject.Parse(response.Content);
            Helper.Job.AssertJobwasCompletedSuccessfully(Constants.Jobs.RECONFIGURE_JOB, Request, job);
        }
    }
}
