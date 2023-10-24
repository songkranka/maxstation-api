using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MaxStation.Utility.Helpers.Token
{
    public class TokenService : ITokenService
    {
        private readonly GenerateTokenModel _generateToken;

        public TokenService(IOptions<GenerateTokenModel> generateToken)
        {
            _generateToken = generateToken.Value;
        }

        public async Task<string> GenerateTokenAsync()
        {
            try
            {
                
                var request = "";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add(_generateToken.KeyName, _generateToken.KeyValue);
                client.BaseAddress = new Uri(_generateToken.UrlEndpoint);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_generateToken.UrlEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    response.Content.ReadAsStringAsync().Wait();
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var jsonDeserializeObject = JsonConvert.DeserializeObject(jsonString).ToString();
                    return jsonDeserializeObject;

                    
                    //var jsonObject = JObject.Parse(jsonDeserializeObject);
                    //var responseData = jsonObject.ToObject<POSPeriodCountResponse>();
                    //var posPeriod = responseData.Data.CountPeriod;

                    //if (maxPeriod == posPeriod)
                    //{
                    //    result = "Yes";
                    //}
                    //else
                    //{
                    //    result = "No";
                    //}
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
