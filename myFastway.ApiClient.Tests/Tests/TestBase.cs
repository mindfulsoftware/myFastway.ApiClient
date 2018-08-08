using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        protected readonly IConfigurationRoot config;
        protected readonly string authority, clientId, secret, scope;
        protected readonly string baseAddress, apiVersion;

        HttpClient httpClient = new HttpClient();

        public TestBase()
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings-local.json")
                .Build();

            authority = config["oauth:authority"];
            clientId = config["oauth:client_id"];
            secret = config["oauth:secret"];
            scope = config["oauth:scope"];

            baseAddress = config["api:base-address"];
            apiVersion = config["api:version"];
        }

        /// <summary>
        /// Returns a token using IdentityServers DiscoveryClient
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetClientCredentialDiscovery() {

            var discoveryClient = new DiscoveryClient(authority) {
                Policy = new DiscoveryPolicy { RequireHttps = true }
            };

            var disco = await discoveryClient.GetAsync();

            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, secret);
            return (await tokenClient.RequestClientCredentialsAsync(scope)).AccessToken;
        }

        /// <summary>
        /// Returns an access token using the HttpClient
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetClientCredentialHttpClient() {

            var content = new StringContent($"grant_type=client_credentials&client_id={clientId}&scope={scope}&client_secret={secret}");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await httpClient.PostAsync($"{authority}/connect/token", content);

            if (response.IsSuccessStatusCode) {
                var result = await response.Content.ReadAsStringAsync();
                return JObject.Parse(result)["access_token"].ToString();
            }

            return null;
        }


        protected async Task<T> GetSingle<T>(string url, string apiVersion = "1.0") {

            var response = await CallApi(GetClientCredentialDiscovery, async (client) => await client.GetAsync($"api/{url}"), apiVersion);

            if (response.IsSuccessStatusCode) {
                var jobj = JObject.Parse(await response.Content.ReadAsStringAsync());
                return jobj["data"].ToObject<T>();
            }

            return default(T);
        }

        protected async Task<HttpResponseMessage> PostSingle(string url, object payload, string apiVersion = "1.0")
        {
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, JsonContentType);
            var result = await CallApi(GetClientCredentialDiscovery, async (client) => await client.PostAsync($"api/{url}", content), apiVersion);
            return result;
        }

        protected async Task<T> PostSingle<T>(string url, object payload, string apiVersion = "1.0") {

            var response = await PostSingle(url, payload, apiVersion);
            if (response.IsSuccessStatusCode) {
                var jobj = JObject.Parse(await response.Content.ReadAsStringAsync());
                return jobj["data"].ToObject<T>();
            }

            return default(T);
        }

        protected async Task<IEnumerable<T>> GetCollection<T>(string url, string apiVersion = "1.0") {

            var response = await CallApi(GetClientCredentialDiscovery, async (client) => await client.GetAsync($"api/{url}"), apiVersion);

            if (response.IsSuccessStatusCode) {
                var jobj = JObject.Parse(await response.Content.ReadAsStringAsync());
                return jobj["data"].ToObject<IEnumerable<T>>();
            }

            return null;
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
                client.BaseAddress = new Uri(baseAddress);
                client.SetBearerToken(await getAccessToken());

                retVal = await callApi(client);

                // check once to see if the access token has expired (default lifetime: 60 mins)
                if (retVal.StatusCode == HttpStatusCode.Unauthorized) {
                    client.SetBearerToken(await getAccessToken());
                    retVal = await callApi(client);
                }

                return retVal;
            }
        }
    }
}
