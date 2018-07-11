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
        public class Job
        {
            /**
             * Monitor job status using polling (re-try) mechanism 
             */
            internal static JObject GetJobStatuswithReTry(HttpRequestWrapper Request,
                                                          string jobId)
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

            //Helper methods for fetching (and retrying) the current Job Status.
            /**
             * Gets job status using job id.
             */
            private static JObject GetJobStatus(HttpRequestWrapper Request,
                                                string JobId)
            {
                IHttpResponse jobStatusResponse = Request.Get(JobId, null);
                Assert.Equal(HttpStatusCode.OK, jobStatusResponse.StatusCode);
                return JObject.Parse(jobStatusResponse.Content);
            }


            //Helper method to verify if job was successful.
            internal static void AssertJobwasCompletedSuccessfully(string content, int jobType, HttpRequestWrapper request)
            {
                // Check if job was submitted successfully.
                var job = JObject.Parse(content);
                Assert.Equal<int>(Constants.Jobs.JOB_IN_PROGRESS, job["Status"].ToObject<int>());
                Assert.Equal<int>(jobType, job["Type"].ToObject<int>());

                // Get Job status by polling. This is to verify if job was successful.
                var tagJobStatus = GetJobStatuswithReTry(request, job["JobId"].ToString());

                // Assert to see if last try yielded correct status.
                Assert.Equal<int>(Constants.Jobs.JOB_COMPLETED, tagJobStatus["Status"].ToObject<int>());
                Assert.Equal<int>(jobType, tagJobStatus["Type"].ToObject<int>());

                // TODO: Verify Result Statistics from the response JSON.
            }
        }

        public class Device
        {
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

            // Assert device ID is not null OR empty and
            // other required properties are set.
            internal static void AssertCommonDeviceProperties(string id, JObject createdDevice)
            {
                string createdDeviceId = createdDevice["Id"].ToString();

                if (String.IsNullOrEmpty(id))
                {
                    Assert.False(string.IsNullOrEmpty(createdDeviceId));
                }
                else
                {
                    Assert.Equal(createdDeviceId, id);
                }

                Assert.False(createdDevice["IsSimulated"].ToObject<bool>());
                Assert.True(createdDevice["Enabled"].ToObject<bool>());
            }

            // Assert auth type and credentials for Symmetric auth.
            internal static void AssertSymmetricAuthentication(string primaryKey, string secondaryKey, JObject createdDevice)
            {
                var authentication = createdDevice["Authentication"];
                string createdPrimaryKey = authentication["PrimaryKey"].ToString();
                string createdSecondaryKey = authentication["SecondaryKey"].ToString();

                Assert.Equal(Constants.Auth.SYMMETRIC, authentication["AuthenticationType"]);

                if (!(String.IsNullOrEmpty(primaryKey) &&
                    String.IsNullOrEmpty(secondaryKey)))
                {
                    Assert.Equal(primaryKey, createdPrimaryKey);
                    Assert.Equal(secondaryKey, createdSecondaryKey);
                }
                else
                {
                    Assert.False(string.IsNullOrEmpty(createdPrimaryKey));
                    Assert.False(string.IsNullOrEmpty(createdSecondaryKey));
                }
            }

            // Assert auth type and credentials for X509 auth.
            internal static void AssertX509Authentication(
                string primaryThumbprint,
                string secondaryThumbprint,
                JObject createdDevice
                )
            {
                var authentication = createdDevice["Authentication"];
                string createdPrimaryThumbprint = authentication["PrimaryThumbprint"].ToString();
                string createdSecondaryThumbprint = authentication["SecondaryThumbprint"].ToString();

                Assert.Equal(Constants.Auth.X509, authentication["AuthenticationType"]);
                Assert.Equal(primaryThumbprint, createdPrimaryThumbprint);
                Assert.Equal(secondaryThumbprint, createdSecondaryThumbprint);
            }
        }
    }
}
