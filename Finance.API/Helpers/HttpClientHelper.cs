using Finance.API.Helpers.Setting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Finance.API.Helpers
{
    public class HttpClientHelper
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpClientHelper(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<TOut> ExecuteRequestAsync<TIn, TOut>(TIn request, string url)
        {
            var requestUri = new Uri(url);
            var payload = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = requestUri,
                Content = httpContent
            };
            return await SendUriAsync<TOut>(requestMessage);
        }

        public async Task<T> SendUriAsync<T>(HttpRequestMessage requestMessage)
        {
            var client = _clientFactory.CreateClient();
            var result = await client.SendAsync(requestMessage);
            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(response);
        }

        //public static async Task<HttpResponseMessage> PostAsync(string jwt, string url, string content)
        //{
        //    try
        //    {
        //        var connection = _configuration["connectionString"];
        //        var client = _clientFactory.CreateClient();
        //        client.BaseAddress = new Uri(url);
        //        client.Timeout = TimeSpan.FromMinutes(3);
        //        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
        //        var data = new StringContent(content, Encoding.UTF8, "text/plain");
        //        var response = await client.PostAsync(url, data);
        //        return response;
        //    }

        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error URL PostAsync : {url} And Error Message : {ex}");
        //    }
        //}
    }
}
