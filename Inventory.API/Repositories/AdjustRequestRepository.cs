using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using Inventory.API.Resources.AdjustRequest;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class AdjustRequestRepository : SqlDataAccessHelper, IAdjustRequestRepository
    {
        public AdjustRequestRepository(PTMaxstationContext context) : base(context)
        {

        }

        public int GetRunNumber(InvAdjustRequestHd AdjustRequestHd)
        {
            int runNumber = 1;
            InvAdjustRequestHd resp = new InvAdjustRequestHd();
            resp = this.context.InvAdjustRequestHds.Where(x => x.DocPattern == AdjustRequestHd.DocPattern).OrderByDescending(x => x.RunNumber).FirstOrDefault();
            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0) + 1;
            }
            return runNumber;
        }

        public string GenDocNo(InvAdjustRequestHd AdjustRequestHd, int runNumber)
        {
            string docno = "";

            var date = AdjustRequestHd.DocDate.Value;
            var Brn = AdjustRequestHd.BrnCode;

            var patterns = (from hd in this.context.MasDocPatterns
                            join dt in this.context.MasDocPatternDts on hd.DocId equals dt.DocId
                            where hd.DocType == "AdjustRequest"
                            select dt).ToList();

            docno = AdjustRequestHd.DocPattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("Brn", Brn);
            docno = docno.Replace("yy", date.ToString("yy"));
            docno = docno.Replace("MM", date.ToString("MM"));
            docno = docno.Replace("#", "") + runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }

        public async Task AddHdAsync(InvAdjustRequestHd AdjustRequestHd)
        {
            await context.InvAdjustRequestHds.AddAsync(AdjustRequestHd);
        }

        public decimal CalculateStockQty(string pdId, string unitId, decimal itemQty)
        {
            decimal stockQty = 0m;
            MasProductUnit productUnit = this.context.MasProductUnits.FirstOrDefault(x => x.PdId == pdId && x.UnitId == unitId);
            if (productUnit != null)
            {
                stockQty = (itemQty * (productUnit.UnitStock / productUnit.UnitRatio)).Value;
            }
            return stockQty;
        }

        public async Task AddDtAsync(List<InvAdjustRequestDt> AdjustRequestDt)
        {
            await context.InvAdjustRequestDts.AddRangeAsync(AdjustRequestDt);
        }

        public async Task<InvAdjustRequestHd> FindByIdAsync(Guid guid)
        {
            var AdjustRequestHd = await context.InvAdjustRequestHds.FirstOrDefaultAsync(x => x.Guid == guid);
            AdjustRequestHd.InvAdjustRequestDt = await context.InvAdjustRequestDts.Where(x => x.DocNo == AdjustRequestHd.DocNo).ToListAsync();
            return AdjustRequestHd;
        }

        public async Task<QueryResult<InvAdjustRequestHd>> GetAdjustRequestHDList(AdjustRequestQueryResource req)
        {
            try
            {
                var queryable = context.InvAdjustRequestHds
                                 .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode)
                                 .AsNoTracking();

                if (!string.IsNullOrEmpty(req.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(req.Keyword));
                }
                else if (req.FromDate != null && req.ToDate != null)
                {
                    queryable = queryable.Where(p => p.DocDate >= req.FromDate && p.DocDate <= req.ToDate);
                }

                if (req.onlyReadyStatus.HasValue) 
                {
                    if (req.onlyReadyStatus.Value) 
                    {
                        queryable = queryable.Where(x => x.DocStatus == "Ready");
                    }
                }

                int totalItems = await queryable.CountAsync();
                var resp = await queryable.OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo).Skip((req.Page - 1) * req.ItemsPerPage)
                                               .Take(req.ItemsPerPage)
                                               .ToListAsync();

                return new QueryResult<InvAdjustRequestHd>
                {
                    Items = resp,
                    TotalItems = totalItems,
                    ItemsPerPage = req.ItemsPerPage
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<InvAdjustRequestDt>> GetDetailAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            List<InvAdjustRequestDt> items = new List<InvAdjustRequestDt>();
            items = await this.context.InvAdjustRequestDts.Where(x => x.CompCode == compCode
                                                                && x.BrnCode == brnCode
                                                                && x.LocCode == locCode
                                                                && x.DocNo == docNo).ToListAsync();
            return items;
        }

        public void UpdateAsync(InvAdjustRequestHd AdjustRequestHd)
        {
            context.InvAdjustRequestHds.Update(AdjustRequestHd);
        }

        public void AddDetailAsync(IEnumerable<InvAdjustRequestDt> AdjustRequestDts)
        {
            context.InvAdjustRequestDts.AddRange(AdjustRequestDts);
        }

        public void RemoveDetailAsync(IEnumerable<InvAdjustRequestDt> AdjustRequestDts)
        {
            context.InvAdjustRequestDts.RemoveRange(AdjustRequestDts);
        }
    }
}
