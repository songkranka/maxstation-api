using Inventory.API.Domain.Models;
using MaxStation.Entities.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.API.Helpers
{
    public class SqlDataAccessHelper : DataModelHelper
    {
        protected PTMaxstationContext context;

        public SqlDataAccessHelper(PTMaxstationContext context)
        {
            this.context = context;
        }

		public async Task SendDataToWarpadAsync(RequestWarpadModel req, string url)
		{
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(url);
            var content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
        }
	}
}
