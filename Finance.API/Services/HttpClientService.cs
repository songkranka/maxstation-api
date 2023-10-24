using Finance.API.Domain.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Finance.API.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _clientFactory;
        public HttpClientService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<TResult> PostAsync<TRequest, TResult>(string jwt, string url, TRequest t)
        {
            var result = default(TResult);
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromMinutes(3);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var json = JsonConvert.SerializeObject(t);
            HttpContent httpContent = new StringContent(json, Encoding.UTF8, "text/plain");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url, httpContent);
            
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
            {
                if (x.IsFaulted)
                    throw x.Exception;

                result = JsonConvert.DeserializeObject<TResult>(x.Result);
            });

            return result;
        }

        //public async Task<TResult> PostAsync<TRequest, TResult>(string jwt, string url, TRequest t)
        //{
        //    var result = default(TResult);
        //    var client = _clientFactory.CreateClient();
        //    client.BaseAddress = new Uri(url);
        //    client.Timeout = TimeSpan.FromMinutes(3);
        //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
        //    var json = JsonConvert.SerializeObject(t);
        //    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "text/plain");
        //    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var response = await client.PostAsync(url, httpContent);

        //    response.EnsureSuccessStatusCode();

        //    await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
        //    {
        //        if (x.IsFaulted)
        //            throw x.Exception;

        //        result = JsonConvert.DeserializeObject<TResult>(x.Result);
        //    });

        //    return result;
        //}
    }
}
