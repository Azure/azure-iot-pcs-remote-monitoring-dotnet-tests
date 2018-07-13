using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Helpers.Http
{
    public static class HttpHelpers
    {
        private const int RETRY_COUNT = 10;
        private const int RETRY_PAUSE_MSEC = 10000;
        private const int TIMEOUT_MSEC = 30000;

        /**
         * Send GET request using given httpClient against given requestAddress.
         * Retry up to RETRY_COUNT times on non-OK status code, waiting
         * RETRY_PAUSE_MSEC between requests. Return http response on success,
         * null on failure.
         */
        public static IHttpResponse SendHttpGetRequestWithRetry(
            string requestAddress,
            IHttpClient httpClient)
        {
            var request = new HttpRequest(requestAddress);
            request.Options.Timeout = TIMEOUT_MSEC;

            int retryCount = 0;
            while (retryCount < RETRY_COUNT)
            {
                try
                {
                    retryCount++;
                    var response = httpClient.GetAsync(request).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return response;
                    }
                    else if (retryCount < RETRY_COUNT)
                    {
                        Thread.Sleep(RETRY_PAUSE_MSEC);
                    }
                }
                catch (Exception)
                {
                    if (retryCount < RETRY_COUNT)
                    {
                        Thread.Sleep(RETRY_PAUSE_MSEC);
                    }

                }
            }

            return null;
        }

        /**
         * Given an HttpResponse, assert it has an OK status code and it's
         * response is valid json. Return the json response, fail if
         * it is invalid.
         */
        public static JObject GetJsonResponseIfValid(IHttpResponse response)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            JObject jsonResponse = JObject.Parse(response.Content);
            Assert.True(jsonResponse.HasValues);
            return jsonResponse;
        }
    }
}
