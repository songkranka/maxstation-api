using Common.API.Domain.Models;
using Common.API.Domain.Repositories;
using Common.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Common.API.Repositories
{
    public class CommonRepository : SqlDataAccessHelper, ICommonRepository
    {
        public CommonRepository(PTMaxstationContext context) : base(context)
        {
        }

        public void UpdateCloseDay(RequestData req)
        {
            //InvRequestHds
            var invRequestHds = context.InvRequestHds
                .Where(
                    x => req.CompCode == null || req.CompCode == "" || x.CompCode == req.CompCode
                    && req.BrnCode == null || req.BrnCode == "" || x.BrnCode == req.BrnCode
                    && x.DocDate <= req.CloseDate
                ).ToList();
            invRequestHds.ForEach(x => x.Post = "P");
            context.UpdateRange(invRequestHds);

            //SalQuotationHD
            var salQuotationHds = context.SalQuotationHds
                .Where(
                    x => req.CompCode == null || req.CompCode == "" || x.CompCode == req.CompCode
                    && req.BrnCode == null || req.BrnCode == "" || x.BrnCode == req.BrnCode
                    && x.DocDate <= req.CloseDate
                ).ToList();
            salQuotationHds.ForEach(x => x.Post = "P");
            context.UpdateRange(salQuotationHds);

        }
    }
}
