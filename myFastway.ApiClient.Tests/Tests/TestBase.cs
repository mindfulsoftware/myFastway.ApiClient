using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using myFastway.ApiClient.Tests.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace myFastway.ApiClient.Tests
{
    public abstract class TestBase
    {
        const string JsonContentType = "application/json";

        protected readonly IConfigurationRoot configurationRoot;
        protected readonly ConfigModel config;

        static readonly HttpClient httpClient = new HttpClient();

        public TestBase()
        {
            configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json")
                .Build();

            config = new ConfigModel(configurationRoot);

        }

        /// <summary>
        /// Returns a token using IdentityServers DiscoveryClient
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetClientCredentialDiscovery() {

            var discoveryClient = new DiscoveryClient(config.OAuth.Authority) {
                Policy = new DiscoveryPolicy { RequireHttps = true }
            };

            var disco = await discoveryClient.GetAsync();

            var tokenClient = new TokenClient(disco.TokenEndpoint, config.OAuth.ClientId, config.OAuth.Secret);
            return (await tokenClient.RequestClientCredentialsAsync(config.OAuth.Scope)).AccessToken;
        }

        /// <summary>
        /// Returns an access token using the HttpClient
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetClientCredentialHttpClient() {

            var content = new StringContent($"grant_type=client_credentials&client_id={config.OAuth.ClientId}&scope={config.OAuth.Scope}&client_secret={config.OAuth.Secret}");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await httpClient.PostAsync($"{config.OAuth.Authority}/connect/token", content);

            if (response.IsSuccessStatusCode) {
                var result = await response.Content.ReadAsStringAsync();
                return JObject.Parse(result)["access_token"].ToString();
            }

            return null;
        }


        protected async Task<T> GetSingle<T>(string url, string apiVersion = "1.0") {

            var response = await CallApi(GetClientCredentialDiscovery, async (client) => await client.GetAsync($"api/{url}"), apiVersion);
            var result = await ParseResponse<T>(response);
            return result;
        }

        protected async Task<byte[]> GetBytes(string url, string apiVersion = "1.0")
        {
            var response = await CallApi(GetClientCredentialDiscovery, async (client) => await client.GetAsync($"api/{url}"), apiVersion);
            var result = await response.Content.ReadAsByteArrayAsync();
            return result;
        }

        protected async Task<HttpResponseMessage> PostSingle(string url, object payload, string apiVersion = "1.0")
        {
            var content = GetPayloadContent(payload);
            var result = await CallApi(GetClientCredentialDiscovery, async (client) => await client.PostAsync($"api/{url}", content), apiVersion);
            return result;
        }

        protected async Task<HttpResponseMessage> PutSingle(string url, object payload, string apiVersion = "1.0")
        {
            var content = GetPayloadContent(payload);
            var result = await CallApi(GetClientCredentialDiscovery, async (client) => await client.PutAsync($"api/{url}", content), apiVersion);
            return result;
        }

        protected async Task<T> PostSingle<T>(string url, object payload, string apiVersion = "1.0") {

            var response = await PostSingle(url, payload, apiVersion);
            var result = await ParseResponse<T>(response);
            return result;
        }

        protected async Task<IEnumerable<T>> GetCollection<T>(string url, string apiVersion = "1.0") {

            var response = await CallApi(GetClientCredentialDiscovery, async (client) => await client.GetAsync($"api/{url}"), apiVersion);
            var result = await ParseResponse<IEnumerable<T>>(response);
            return result;
        }

        protected async Task<HttpResponseMessage> Delete(string url, string apiVersion = "1.0")
        {
            var result = await CallApi(GetClientCredentialDiscovery, async client => await client.DeleteAsync($"api/{url}"), apiVersion);
            return result;
        }

        StringContent GetPayloadContent(object payload)
        {
            var serialised = payload == null ? string.Empty : JsonConvert.SerializeObject(payload);
            var result = new StringContent(serialised, Encoding.UTF8, JsonContentType);
            return result;
        }

        async Task<T> ParseResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response Body:\r\n{responseBody}");
                var jobj = JObject.Parse(responseBody);
                return jobj["data"].ToObject<T>();
            }
            return default(T);
        }


        /// <summary>
        /// Make a call to the api, setting the required headers and bearer token.  In the case where the token has expired, it renews
        /// the token and trys a single time more.
        /// </summary>
        /// <param name="getAccessToken">a function to retrieve an acces token</param>
        /// <param name="callApi">the function making the request</param>
        /// <param name="apiVersion">the version of the api being called.</param>
        /// <returns></returns>
        async Task<HttpResponseMessage> CallApi(Func<Task<string>> getAccessToken, Func<HttpClient, Task<HttpResponseMessage>> callApi, string apiVersion) {
            HttpResponseMessage retVal = null;

            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Accept.Clear();
                var jsonHeader = new MediaTypeWithQualityHeaderValue(JsonContentType);

                if (!string.IsNullOrEmpty(apiVersion))
                    jsonHeader.Parameters.Add(new NameValueHeaderValue("api-version", apiVersion));

                client.DefaultRequestHeaders.Accept.Add(jsonHeader);
                client.BaseAddress = new Uri(config.Api.BaseAddress);
                var accessToken = await getAccessToken();
                client.SetBearerToken(accessToken);

                retVal = await callApi(client);

                // check once to see if the access token has expired (default lifetime: 60 mins)
                if (retVal.StatusCode == HttpStatusCode.Unauthorized) {
                    client.SetBearerToken(await getAccessToken());
                    retVal = await callApi(client);
                }

                return retVal;
            }
        }


        /// <summary>
        /// For a given model, loads a json object with the expected results from disk.  Before a test referencing this function will pass, a 'local' copy
        /// of the file will have to be created by copying <typeparamref name="T"/>.json to <typeparamref name="T"/>.local.json populating the default
        /// object with the expected results.
        /// </summary>
        /// <typeparam name="T">The type of the model to be loaded from file</typeparam>
        /// <param name="filename">The phyiscal file name (excluding the .local extension)</param>
        /// <returns></returns>
        protected async Task<T> LoadModelFromFile<T>(string filename) {

            string localFileName = $"{filename}.local.json";

            if (File.Exists(localFileName)) {
                var serializedObject = await File.ReadAllTextAsync(localFileName);
                return JsonConvert.DeserializeObject<T>(serializedObject);
            }
            
            throw new FileNotFoundException($"Cannot find file {localFileName}.  Please copy {filename}.json to a new file {localFileName} poplulating the model with the expected results ", localFileName);
        }
    }
}
