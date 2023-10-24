using Common.API.Domain.Models.Queries;
using Common.API.Domain.Repositories;
using Common.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Repositories
{
    public class CashSaleRepository : SqlDataAccessHelper, ICashSaleRepository
    {
        public CashSaleRepository(PTMaxstationContext context) : base(context)
        {

        }

        public async Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query)
        {
            var resp = new List<SalCashsaleHd>();
            int totalItems = 0;

            if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                var queryable = context.SalCashsaleHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate >= query.FromDate && x.DocDate <= query.ToDate)
                .OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
                .AsNoTracking();

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                        || p.VatAmtCur.ToString().Contains(query.Keyword)
                        || p.DocStatus.Contains(query.Keyword));
                }

                resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();
                totalItems = await queryable.CountAsync();
            }
            else
            {
                var queryable = context.SalCashsaleHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate >= query.SysDate.AddDays(-30))
                .OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
                .Skip((query.Page - 1) * query.ItemsPerPage)
                .Take(query.ItemsPerPage)
                .AsNoTracking();

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                        || p.VatAmtCur.ToString().Contains(query.Keyword)
                        || p.DocStatus.Contains(query.Keyword));
                }

                resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();
                totalItems = await queryable.CountAsync();
            }
            context.Dispose();            
            return new QueryResult<SalCashsaleHd>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }
    }
}
