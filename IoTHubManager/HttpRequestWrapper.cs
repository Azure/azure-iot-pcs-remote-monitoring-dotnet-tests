using System;
using System.Net;
using System.Security.Cryptography;
using Helpers.Http;
using Newtonsoft.Json.Linq;

namespace IoTHubManager
{
    class HttpRequestWrapper
    {
        private readonly IHttpClient httpClient;
        private string uri;

        internal HttpRequestWrapper(string host, string path)
        {
            httpClient = new HttpClient();
            if (!String.IsNullOrEmpty(host))
            {
                this.uri = host;
            }
            if (!String.IsNullOrEmpty(path))
            {
                this.uri += path;
            }
        }

        internal IHttpResponse Get()
        {
            var request = new HttpRequest(uri);
            return this.httpClient.GetAsync(request).Result;
        }

        //TODO: Method should accept Map of query parameters and build the query
        internal IHttpResponse Get(string path, string query)
        {
            string uri = this.uri;
            if (!String.IsNullOrEmpty(path))
            {
                uri += path;
            }
            if (!String.IsNullOrEmpty(query))
            {
                uri += "?"+query;
            }

            var request = new HttpRequest(uri);
            return this.httpClient.GetAsync(request).Result;
        }

        internal IHttpResponse Post(string content)
        {
            var request = new HttpRequest(uri);
            request.SetContent(content);
            return this.httpClient.PostAsync(request).Result;
           
        }

        internal IHttpResponse Put(JObject content)
        {
            var request = new HttpRequest(uri);
            request.SetContent(content);
            return this.httpClient.PutAsync(request).Result;
        }
    }
}
