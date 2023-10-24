using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using Inventory.API.Resources.Adjust;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class AdjustRepository : SqlDataAccessHelper, IAdjustRepository
    {
        private const string _strActive = "Active";
        private const string _strAdjust = "Adjust";
        public AdjustRepository(PTMaxstationContext context) : base(context)
        {
        }

        public int GetRunNumber(InvAdjustHd AdjustHd)
        {
            int runNumber = 1;
            InvAdjustHd resp = new InvAdjustHd();
            resp = this.context.InvAdjustHds.Where(x => x.DocPattern == AdjustHd.DocPattern).OrderByDescending(x => x.RunNumber).FirstOrDefault();
            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0) + 1;
            }
            return runNumber;
        }

        public string GenDocNo(InvAdjustHd AdjustHd, int runNumber)
        {
            string docno = "";

            var date = AdjustHd.DocDate.Value;
            var Brn = AdjustHd.BrnCode;

            var patterns = (from hd in this.context.MasDocPatterns
                            join dt in this.context.MasDocPatternDts on hd.DocId equals dt.DocId
                            where hd.DocType == "Adjust"
                            select dt).ToList();

            docno = AdjustHd.DocPattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("Brn", Brn);
            docno = docno.Replace("yy", date.ToString("yy"));
            docno = docno.Replace("MM", date.ToString("MM"));
            docno = docno.Replace("#", "") + runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }

        public void RemainAdjustRequest(InvAdjustHd AdjustHd)
        {
            var adjustRequestHd = context.InvAdjustRequestHds.Where(x => x.DocNo == AdjustHd.RefNo).FirstOrDefault();
            if (adjustRequestHd != null) 
            {
                adjustRequestHd.DocStatus = "Reference";
                adjustRequestHd.UpdatedBy = AdjustHd.UpdatedBy;
                adjustRequestHd.UpdatedDate = DateTime.Now;

                var adjustRequestDt = context.InvAdjustRequestDts.Where(x => x.DocNo == AdjustHd.RefNo).ToList();
                foreach (var item in adjustRequestDt)
                {
                    var adjust = AdjustHd.InvAdjustDt.Where(x => x.PdId == item.PdId).FirstOrDefault();
                    if (adjust != null)
                    {
                        item.StockRemain -= adjust.StockQty;
                    }
                }
                context.InvAdjustRequestDts.UpdateRange(adjustRequestDt);
            }
        }

        public async Task AddHdAsync(InvAdjustHd AdjustHd)
        {
            await context.InvAdjustHds.AddAsync(AdjustHd);
        }

        public async Task AddDtAsync(List<InvAdjustDt> AdjustDt)
        {
            await context.InvAdjustDts.AddRangeAsync(AdjustDt);
        }

        public decimal CalculateStockQty(string pdId, decimal itemQty)
        {
            decimal stockQty = 0m;
            MasProductUnit productUnit = this.context.MasProductUnits.FirstOrDefault(x => x.PdId == pdId);
            if (productUnit != null)
            {
                stockQty = (itemQty * (productUnit.UnitStock / productUnit.UnitRatio)).Value;
            }
            return stockQty;
        }

        public async Task<InvAdjustHd> FindByIdAsync(Guid guid)
        {
            var AdjustHd = await context.InvAdjustHds.FirstOrDefaultAsync(x => x.Guid == guid);
            AdjustHd.InvAdjustDt = await context.InvAdjustDts.Where(x => x.DocNo == AdjustHd.DocNo).ToListAsync();
            return AdjustHd;
        }

        public async Task<QueryResult<InvAdjustHd>> GetAdjustHDList(AdjustQueryResource req)
        {
            try
            {
                var queryable = context.InvAdjustHds
                                 .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode && x.DocType == "Adjust")
                                 .AsNoTracking();

                if (!string.IsNullOrEmpty(req.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(req.Keyword));
                }
                else if (req.FromDate != null && req.ToDate != null)
                {
                    queryable = queryable.Where(p => p.DocDate >= req.FromDate && p.DocDate <= req.ToDate);
                }

                int totalItems = await queryable.CountAsync();
                var resp = await queryable.OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo).Skip((req.Page - 1) * req.ItemsPerPage)
                                               .Take(req.ItemsPerPage)
                                               .ToListAsync();

                return new QueryResult<InvAdjustHd>
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

        public async Task<List<InvAdjustDt>> GetDetailAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            List<InvAdjustDt> items = new List<InvAdjustDt>();
            items = await this.context.InvAdjustDts.Where(x => x.CompCode == compCode
                                                                && x.BrnCode == brnCode
                                                                && x.LocCode == locCode
                                                                && x.DocNo == docNo).ToListAsync();
            return items;
        }

        public void UpdateAsync(InvAdjustHd AdjustHd)
        {
            context.InvAdjustHds.Update(AdjustHd);
        }

        public void AddDetailAsync(IEnumerable<InvAdjustDt> AdjustDts)
        {
            context.InvAdjustDts.AddRange(AdjustDts);
        }

        public void RemoveDetailAsync(IEnumerable<InvAdjustDt> AdjustDts)
        {
            context.InvAdjustDts.RemoveRange(AdjustDts);
        }

        public async Task<string> GetBranchName(string brnCode)
        {
            var branchName = await context.MasBranches.Where(x => x.BrnCode == brnCode).Select(x => x.BrnName).AsNoTracking().FirstOrDefaultAsync();
            return branchName;
        }

        public async Task<MasReason[]> GetReasonAdjusts()
        {
            IQueryable<MasReason> qryReason = null;
            qryReason = context.MasReasons.AsNoTracking().Where(
                x => _strActive.Equals(x.ReasonStatus)
                && _strAdjust.Equals(x.ReasonGroup)
            );
            MasReason[] result = null;
            result = await qryReason.ToArrayAsync();
            return result;
        }
    }
}
