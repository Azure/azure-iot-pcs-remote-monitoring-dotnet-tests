using Helpers;
using Helpers.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace StorageAdapter
{
    [Collection("Storage Adapter Tests")]
    public class ValuesTest : IDisposable
    {
        private readonly IHttpClient httpClient;
        private List<(string, string)> collectionAndKeysToCleanup;

        public ValuesTest()
        {
            this.httpClient = new HttpClient();
            this.collectionAndKeysToCleanup = new List<(string, string)>();
        }
        
        /// <summary>
        /// Integration test verifying an item can be posted to the values controller and 
        /// subsequently retrieved.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void CanPutAndRetrieveValues()
        {
            var itemSuffix = Guid.NewGuid().ToString();
            var collectionId = "testCollection" + itemSuffix;

            var contentToSet = new ValueApiModel()
            {
                Data = $"testData:{itemSuffix}"
            };

            var postRequest = new HttpRequest(Constants.STORAGE_ADAPTER_ADDRESS + $"/collections/{collectionId}/values");
            postRequest.SetContent(contentToSet);

            var postResponse = this.httpClient.PostAsync(postRequest).Result;
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            // Retrieve values and verify test item exists.
            var getRequest = new HttpRequest(Constants.STORAGE_ADAPTER_ADDRESS + $"/collections/{collectionId}/values");
            var getResponse = this.httpClient.GetAsync(getRequest).Result;
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var valueList = JsonConvert.DeserializeObject<ValueListApiModel>(getResponse.Content);

            // Assert on the test data of retrieved item.
            Assert.NotNull(valueList);
            Assert.Single(valueList.Items);

            var returnedValue = valueList.Items.First();
            this.collectionAndKeysToCleanup.Add((collectionId, returnedValue.Key));

            Assert.Equal($"testData:{itemSuffix}", returnedValue.Data);
            Assert.False(String.IsNullOrEmpty(returnedValue.ETag));
            Assert.False(String.IsNullOrEmpty(returnedValue.Key));
        }
        
        /// <summary>
        /// Integration test verifying an item can be put with a particular key and
        /// subsequently retrieved with that same key.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void CanPutAndRetrieveValuesByKey()
        {
            // Arrange
            var itemSuffix = Guid.NewGuid().ToString();
            var itemKey = "putKey";
            var collectionId = "testCollection" + itemSuffix;

            var contentToSet = new ValueApiModel()
            {
                Data = $"testData:{itemSuffix}"
            };

            // Act
            this.PutItemByKey(Constants.STORAGE_ADAPTER_ADDRESS + $"/collections/{collectionId}/values/{itemKey}", contentToSet);
            this.collectionAndKeysToCleanup.Add((collectionId, itemKey));

            // Assert on the test data of retrieved item.
            var returnedValue = this.GetItemByKey(collectionId, itemKey);
            Assert.Equal($"testData:{itemSuffix}", returnedValue.Data);
            Assert.False(String.IsNullOrEmpty(returnedValue.ETag));
            Assert.Equal(itemKey, returnedValue.Key);
        }

        /// <summary>
        /// Integration test verifying an item can be put with a particular key and
        /// then updated by the returned ETag.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void CanUpdateValueWithETag()
        {
            // Arrange
            var itemSuffix = Guid.NewGuid().ToString();
            var itemKey = "updateKey";
            var collectionId = "testCollection" + itemSuffix;
            var requestUri = Constants.STORAGE_ADAPTER_ADDRESS + $"/collections/{collectionId}/values/{itemKey}";

            var contentToSet = new ValueApiModel()
            {
                Data = $"testData:{itemSuffix}"
            };

            // Act place item with initial data
            this.PutItemByKey(requestUri, contentToSet);
            this.collectionAndKeysToCleanup.Add((collectionId, itemKey));

            var returnedValue = GetItemByKey(collectionId, itemKey);
            Assert.Equal($"testData:{itemSuffix}", returnedValue.Data);

            // Set item with updated data
            contentToSet.ETag = returnedValue.ETag;
            contentToSet.Data = "updatedData";
            this.PutItemByKey(requestUri, contentToSet);

            // Assert that the value for this key was updated.
            returnedValue = GetItemByKey(collectionId, itemKey);
            Assert.Equal($"updatedData", returnedValue.Data);
        }

        /// <summary>
        /// Integration test verifying an item can be added and then deleted by key.
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void CanDeleteItemByKey()
        {
            var itemSuffix = Guid.NewGuid().ToString();
            var itemKey = "deleteKey";
            var collectionId = "testCollection" + itemSuffix;
            var requestUri = Constants.STORAGE_ADAPTER_ADDRESS + $"/collections/{collectionId}/values/{itemKey}";

            var contentToSet = new ValueApiModel()
            {
                Data = $"testData:{itemSuffix}"
            };

            this.PutItemByKey(requestUri, contentToSet);

            // Delete the item using the same key.
            this.DeleteItemByKey(collectionId, itemKey);

            // Retrieve value by key and verify test item does not exist.
            var getRequest = new HttpRequest(Constants.STORAGE_ADAPTER_ADDRESS + $"/collections/{collectionId}/values/{itemKey}");
            var getResponse = this.httpClient.GetAsync(getRequest).Result;
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
        

        /// <summary>
        /// Integration test verifying an empty item leads to a bad request i.e. Status Code 400
        /// </summary>
        [Fact, Trait(Constants.TEST, Constants.INTEGRATION_TEST)]
        public void ReturnsBadRequestForEmptyContent()
        {
            var emptyPutRequest = new HttpRequest(Constants.STORAGE_ADAPTER_ADDRESS + "/collections/emptyCollection/values");
            emptyPutRequest.SetContent("");
            emptyPutRequest.Options.Timeout = 10000;
            var putResponse = this.httpClient.PostAsync(emptyPutRequest).Result;
            Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
        }

        private void PutItemByKey(string path, ValueApiModel content)
        {
            var postRequest = new HttpRequest(path);
            postRequest.SetContent(content);
            var putResponse = this.httpClient.PutAsync(postRequest).Result;
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
        }

        private ValueApiModel GetItemByKey(string collectionId, string itemKey)
        {
            var getRequest = new HttpRequest(Constants.STORAGE_ADAPTER_ADDRESS + $"/collections/{collectionId}/values/{itemKey}");
            var getResponse = this.httpClient.GetAsync(getRequest).Result;
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            return JsonConvert.DeserializeObject<ValueApiModel>(getResponse.Content);
        }

        private void DeleteItemByKey(string collectionId, string itemKey)
        {
            var deleteRequest = new HttpRequest(Constants.STORAGE_ADAPTER_ADDRESS + $"/collections/{collectionId}/values/{itemKey}");
            var deleteResponse = this.httpClient.DeleteAsync(deleteRequest).Result;
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        public void Dispose()
        {
            foreach (var collectionAndKeyToDelete in this.collectionAndKeysToCleanup)
            {
                this.DeleteItemByKey(collectionAndKeyToDelete.Item1, collectionAndKeyToDelete.Item2);
            }
        }
    }
}
