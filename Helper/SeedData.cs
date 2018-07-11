using Helpers.Http;
using System.Net;

namespace Helpers
{
    public static class SeedData
    {
        private const int TIMEOUT = 10000;
        private const int RETRYLIMIT = 10;
        private static HttpClient httpClient = new HttpClient();

        public static bool WaitForSeedComplete()
        {
            int retryCount = 0;
            while (retryCount < RETRYLIMIT)
            {
                var request = new HttpRequest(Constants.STORAGE_ADAPTER_ADDRESS + "/collections/solution-settings/values/seedCompleted");
                request.Options.Timeout = TIMEOUT;
                var response = httpClient.GetAsync(request).Result;
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    return true;
                }
                else if (HttpStatusCode.NotFound == response.StatusCode)
                {
                    System.Threading.Thread.Sleep(TIMEOUT);
                    retryCount++;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
