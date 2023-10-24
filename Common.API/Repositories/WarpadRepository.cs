using Common.API.Domain.Models;
using Common.API.Domain.Repositories;
using Common.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Common.API.Repositories
{
    public class WarpadRepository : SqlDataAccessHelper, IWarpadRepository
    {
        public WarpadRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<ResponseWarpadTaskList> GetWarpadTaskList(RequestWarpadTaskList req)
        {
            try
            {
                var codeDev = context.MasEmployees.Where(x => x.EmpCode == req.User).Select(x => x.CodeDev).FirstOrDefault();
                if (codeDev == null) 
                {
                    throw new Exception("Not found this employee");
                }

                var response = await CallWarpadTaskList(codeDev);

                //var listData = new List<ResponseWarpadTaskData>();
                //for (int i = 0 ; i <= 20; i++)
                //{
                //    listData.Add(response.Data[0]);
                //}


                //response.Data = listData;

                return response;
            }
            catch (Exception ex) 
            {
                throw new Exception($"Error in GetWarpadTaskList {ex.Message}");
            }
        }

        private async Task<ResponseWarpadTaskList> CallWarpadTaskList(string codeDev) 
        {
            var urlWarpad = context.SysConfigApis.Where(x => x.SystemId == "Warpad" && x.ApiId == "M006").Select(x => x.ApiUrl).AsNoTracking().FirstOrDefault();
            if (urlWarpad != null) 
            {
                var client = new HttpClient();

                urlWarpad = urlWarpad.Replace("{BranchCode}", codeDev);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(urlWarpad);
                var response = await client.GetAsync(urlWarpad);
                if (response.IsSuccessStatusCode)
                {
                    response.Content.ReadAsStringAsync().Wait();
                    var jsonString = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<ResponseWarpadTaskList>(jsonString);
                }
                else
                {
                    throw new Exception("Call warpad task list failed");
                }
            }
            else 
            {
                throw new Exception("Not found warpad url");
            }
        }

        public string GetSysConfigApi(string systemId, string apiId)
        {
            return context.SysConfigApis.Where(x => x.SystemId == systemId && x.ApiId == apiId).Select(x => x.ApiUrl).AsNoTracking().FirstOrDefault();
        }

    }
}
