using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
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
    public class CashsaleRepository : SqlDataAccessHelper, ICashsaleRepository
    {
        public CashsaleRepository(PTMaxstationContext context) : base(context)
        {
        }


        public async Task<List<SalCashsaleHd>> ListCashsaleAsync(CashsaleQuery query)
        {
            List<SalCashsaleHd> heads = new List<SalCashsaleHd>();

            heads = await this.context.SalCashsaleHds.Where(x =>  x.CompCode == query.CompCode
                                                       && x.BrnCode == query.BrnCode
                                                       && x.DocDate == query.DocDate
                                                       && x.CreatedBy != "dummy"
                                                        // x.DocStatus != "Cancel"
                                                        //&& x.Post == "P"
                                                        ).ToListAsync();

            heads.ForEach(hd => hd.SalCashsaleDt = this.context.SalCashsaleDts.Where(x => x.CompCode == hd.CompCode
                                                                                 && x.BrnCode == hd.BrnCode
                                                                                 && x.LocCode == hd.LocCode
                                                                                 && x.DocNo == hd.DocNo).ToList()
            );
            return heads;
        }

        public async Task<List<SaleNonOil>> ListCashsaleNonOilAmountAsync(SummaryQuery query)
        {
            List<SaleNonOil> result = new List<SaleNonOil>();
            var resp = (from hd in this.context.SalCashsaleHds
                        join dt in this.context.SalCashsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                        join prod in this.context.MasProducts on dt.PdId equals prod.PdId
                        where hd.DocStatus != "Cancel"
                           && hd.CreatedBy != "dummy"
                           && hd.DocDate == query.DocDate
                           && prod.PdType == "Product"
                           && prod.GroupId != "0000"  // oil-gas
                        select new { hd, dt }
                         ).AsQueryable();

            result = await resp.GroupBy(x => new { x.hd.CompCode, x.hd.BrnCode, x.hd.DocDate})
                            .Select(x => new SaleNonOil
                            {
                                CompCode = x.Key.CompCode,
                                BrnCode = x.Key.BrnCode,
                                DocDate = x.Key.DocDate,
                                NetAmt = x.Sum(s => s.dt.SubAmt)
                            }).ToListAsync();

            return result;
        }

        public async Task<List<CashsaleDisc>> ListCashsaleSummaryDiscAsync(SummaryQuery query)
        {
            List<CashsaleDisc> result = new List<CashsaleDisc>();

            var resp = (from hd in this.context.SalCashsaleHds
                        join dt in this.context.SalCashsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                        join prod in this.context.MasProducts on dt.PdId equals prod.PdId
                        where hd.DocStatus != "Cancel"
                        && hd.CreatedBy != "dummy"
                        && hd.DocDate == query.DocDate
                        && prod.GroupId == "0000"
                        select new { hd, dt }
                             ).AsQueryable();

            result = await resp.GroupBy(x => new { x.hd.CompCode, x.hd.BrnCode, x.hd.DocDate, x.dt.PdId, x.dt.PdName })
                    .Select(x => new CashsaleDisc
                    {
                        CompCode = x.Key.CompCode,
                        BrnCode = x.Key.BrnCode,
                        DocDate = x.Key.DocDate,
                        PdId = x.Key.PdId,
                        PdName = x.Key.PdName,
                        DiscAmt = x.Sum(s => s.dt.DiscAmt + s.dt.DiscHdAmt)
                    }).Where(x => x.DiscAmt > 0).ToListAsync();


            return result;
        }

        public Task SaveListAsync(List<SalCashsaleHd> cashsalelist)
        {
            throw new NotImplementedException();
        }

        public bool CheckExistsPOS(SalCashsaleHd obj)
        {
            return  context.SalCashsaleHds.Any(x => x.CompCode == obj.CompCode
                            && x.BrnCode == obj.BrnCode
                            && x.LocCode == obj.LocCode
                            && x.PosNo == obj.PosNo);
        }

        public async Task AddHdAsync(SalCashsaleHd cashSaleHd)
        {
            await context.SalCashsaleHds.AddAsync(cashSaleHd);
        }

        public async Task AddDtAsync(SalCashsaleDt cashSaleDt)
        {
            await context.SalCashsaleDts.AddAsync(cashSaleDt);
        }

        public async Task AddLogAsync(SalCashsaleLog cashSaleLog)
        {
            await context.SalCashsaleLogs.AddAsync(cashSaleLog);
        }
    }
}
