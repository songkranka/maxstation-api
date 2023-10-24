using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Repositories
{
    public class WarpadRepository : SqlDataAccessHelper, IWarpadRepository
    {
        public WarpadRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task SendDataToWarpadAsync(WarpadCloseDay req, string url)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(url);
                var content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
            }
            catch
            {

            }
        }

        public async Task<WarpadCloseDay> SendCloseDay(TransferDataResource req)
        {
            WarpadCloseDay request = new WarpadCloseDay();
            var codeDev = context.MasOrganizes.Where(x => x.OrgComp == req.CompCode && x.OrgCode == req.BrnCode).Select(x => x.OrgCodedev).AsNoTracking().FirstOrDefault();
            var apiConfig = context.SysConfigApis.Where(x => x.SystemId == "Warpad" && x.ApiId == "M005").AsNoTracking().FirstOrDefault();
            if (apiConfig != null)
            {

                var toppic = apiConfig.Topic.Replace("{doc_date}", req.WDate.Value.ToString("dd/MM/yyyy"));

                request = new WarpadCloseDay()
                {
                    TOPIC = toppic,
                    CREATE_DATE = DateTime.Now.ToString("yyyyMMdd"),
                    CREATE_TIME = DateTime.Now.AddHours(7).ToString("HH:mm"),
                    BRANCH_FROM = codeDev ?? "",
                    LINK = "#",
                    DATA = new List<string>()
                };

                await SendDataToWarpadAsync(request, apiConfig.ApiUrl);                
            }
            return request;
        }
    }
}
