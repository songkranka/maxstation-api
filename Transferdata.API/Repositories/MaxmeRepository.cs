using MaxStation.Entities.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;

namespace Transferdata.API.Repositories
{
    public class MaxmeRepository : SqlDataAccessHelper, IMaxmeRepository
    {
        public MaxmeRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<MaxmeResponse> GetMaxPosSumAsync(MaxmeQuery query)
        {
            MaxmeResponse response = new MaxmeResponse();

            var formula = this.context.DopFormulas.FirstOrDefault(x => x.SourceName == "MaxPosSum");
            if (formula != null && formula.SourceValue != "")
            {
                //var tomorrow = query.docDate.Date.AddDays(1);
                //var startTime = new DateTime(docDate.Year, docDate.Month, docDate.Day, 05, 00, 00, 000);
                //var endTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 04, 59, 59, 999);

                var request = new MaxPosSumQuery()
                {
                    token = formula.SourceKey,
                    starttime = query.starttime,
                    endtime = query.endtime,
                    shopid = query.shopid
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(formula.SourceValue);
                //client.BaseAddress = new Uri("https://api.uat.maxcard.tech/api/inquiry/CS/api/maxpossum");
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var resmaxme = await client.PostAsync(formula.SourceValue, content);
                //var response = await client.PostAsync("https://api.uat.maxcard.tech/api/inquiry/CS/api/maxpossum", content);


                try
                {
                    if (resmaxme.IsSuccessStatusCode)
                    {
                        resmaxme.Content.ReadAsStringAsync().Wait();
                        var jsonString = resmaxme.Content.ReadAsStringAsync().Result;
                        var responseData = JsonConvert.DeserializeObject<MaxmeResponse>(jsonString);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            return response;

        }
    }
}
