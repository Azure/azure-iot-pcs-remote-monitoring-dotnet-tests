using System;
using System.Security.Cryptography;
using System.Net;
using Helpers.Http;
using Xunit;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Text;

namespace IoTHubManager
{
    class Helpers
    {
        private readonly HttpRequestWrapper Request;

        /*
        Generates random SHA1 hash mimicing the X509 thumb print
         */
        internal static string GenerateNewThumbPrint()
        {

            string input = Guid.NewGuid().ToString();
            SHA1Managed sha = new SHA1Managed();

            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var stringBuilder = new StringBuilder(hash.Length * 2);

            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        //Helper methods for fetching (and retrying) the current Job Status.
        /**
         * Gets job status using job id.
         */
        private static JObject GetJobStatus(HttpRequestWrapper Request, string JobId)
        {
            IHttpResponse jobStatusResponse = Request.Get(JobId, null);
            Assert.Equal(HttpStatusCode.OK, jobStatusResponse.StatusCode);
            return JObject.Parse(jobStatusResponse.Content);
        }

        /**
         * Monitor job status using polling (re-try) mechanism 
         */
        internal static JObject ReTry_GetJobStatus(HttpRequestWrapper Request, string jobId)
        {
            var jobStatus = GetJobStatus(Request, jobId);

            for (int trials = 0; trials < Constants.Jobs.MAX_TRIALS; trials++)
            {
                if (Constants.Jobs.JOB_COMPLETED == jobStatus["Status"].ToObject<int>())
                {
                    break;
                }
                Thread.Sleep(Constants.Jobs.WAIT);
                jobStatus = GetJobStatus(Request, jobId);
            }

            return jobStatus;
        }
    }
}
