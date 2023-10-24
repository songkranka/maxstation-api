using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Helpers;

namespace Transferdata.API.Repositories
{
    public class SummaryRepository : SqlDataAccessHelper, ISummaryRepository
    {
        public SummaryRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<List<SaleEngineOil>> ListCashsaleSummaryDiscAsync(SummaryQuery query)
        {
            List<SaleEngineOil> response = new List<SaleEngineOil>();

            var tempcash =  (from hd in this.context.SalCashsaleHds
                            join dt in this.context.SalCashsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                            join prod in this.context.MasProducts on dt.PdId equals prod.PdId
                            join pt in this.context.MasProductTypes on prod.GroupId equals pt.GroupId
                            where hd.DocStatus != "Cancel"
                            && pt.DocTypeId == "001"  //น้ำมันเครื่อง
                            && hd.DocDate == query.DocDate
                            select new { hd, dt }
                        ).Select(x => new SaleEngineOil
                        {
                            CompCode = x.hd.CompCode,
                            BrnCode = x.hd.BrnCode,
                            DocDate = x.hd.DocDate,
                            DiscAmt = x.dt.DiscAmt + x.dt.DiscHdAmt
                        }).Where(x=>x.DiscAmt>0).AsQueryable();
                

          var  tempcredit = (from hd in this.context.SalCreditsaleHds
                        join dt in this.context.SalCreditsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                        join prod in this.context.MasProducts on dt.PdId equals prod.PdId
                        join pt in this.context.MasProductTypes on prod.GroupId equals pt.GroupId
                        where hd.DocStatus != "Cancel"
                        && pt.DocTypeId == "001"  //น้ำมันเครื่อง                        
                        && hd.DocDate == query.DocDate
                        select new { hd, dt }
                        ).Select(x => new SaleEngineOil
                        {
                            CompCode = x.hd.CompCode,
                            BrnCode = x.hd.BrnCode,
                            DocDate = x.hd.DocDate,
                            DiscAmt = x.dt.DiscAmt + x.dt.DiscHdAmt
                        }).Where(x=>x.DiscAmt>0).AsQueryable();

            var temp = tempcash.Concat(tempcredit);


            response =  temp.GroupBy(x => new { x.CompCode, x.BrnCode, x.DocDate })
                                .Select(x => new SaleEngineOil
                                {
                                    CompCode = x.Key.CompCode,
                                    BrnCode = x.Key.BrnCode,
                                    DocDate = x.Key.DocDate,
                                    DiscAmt = x.Sum(s=>s.DiscAmt)
                                }).OrderBy(x=>x.CompCode).ThenBy(x=>x.BrnCode).ToList();

            return response;
        }
    }
}
