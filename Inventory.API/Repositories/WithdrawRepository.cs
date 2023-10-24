using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class WithdrawRepository : SqlDataAccessHelper, IWithdrawRepository
    {
        private const string _strActive = "Active";
        private const string _strWithdraw = "Withdraw";
        
        public WithdrawRepository(PTMaxstationContext context) : base(context)
        {
        }

        public int GetRunNumber(string compCode, string brnCode, string pattern)
        {
            int runNumber = 0;
            InvWithdrawHd resp = new InvWithdrawHd();
            resp = this.context.InvWithdrawHds.Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.DocPattern == pattern).OrderByDescending(x => x.RunNumber).FirstOrDefault();
            
            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }
            return runNumber;
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

        public string GenDocNo(InvWithdrawHd withdrawHd, string pattern, int runNumber)
        {
            string docno = "";

            var date = withdrawHd.DocDate.Value;
            var Brn = withdrawHd.BrnCode.ToUpper();

            var patterns = (from hd in this.context.MasDocPatterns
                            join dt in this.context.MasDocPatternDts on hd.DocId equals dt.DocId
                            where hd.DocType == "Withdraw"
                            select dt).ToList();

            docno = pattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("Brn", Brn);
            docno = docno.Replace("yy", date.ToString("yy"));
            docno = docno.Replace("MM", date.ToString("MM"));
            docno = docno.Replace("#", "") + runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }

        public async Task<bool> IsDupplicateDocNo(InvWithdrawHd pHeader, string pStrDocNo)
        {
            if (pHeader == null)
            {
                return false;
            }
            var qryWithDraw = context.InvWithdrawHds.Where(
                x => x.DocNo == pStrDocNo
                && x.CompCode == pHeader.CompCode
                && x.BrnCode == pHeader.BrnCode
                && x.LocCode == pHeader.LocCode
            ).AsNoTracking();
            var result = await qryWithDraw.AnyAsync();
            return result;

        }

        public async Task<QueryResult<InvWithdrawHd>> ListAsync(WithdrawQuery query)
        {

            try
            {
                var queryable = context.InvWithdrawHds
                 .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.LocCode == query.LocCode)
                 .AsNoTracking();

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword));
                }
                if (query.FromDate != null && query.ToDate != null)
                {
                    queryable = queryable.Where(p => p.DocDate >= query.FromDate && p.DocDate <= query.ToDate);
                }

                int totalItems = await queryable.CountAsync();
                var resp = await queryable.OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo).Skip((query.Page - 1) * query.ItemsPerPage)
                                               .Take(query.ItemsPerPage)
                                               .ToListAsync();

                return new QueryResult<InvWithdrawHd>
                {
                    Items = resp,
                    TotalItems = totalItems,
                    ItemsPerPage = query.ItemsPerPage
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task AddHdAsync(InvWithdrawHd withdrawHd)
        {
            var costCenter = context.MasCostCenters.FirstOrDefault(x => x.BrnCode == withdrawHd.UseBrnCode && x.CompCode == withdrawHd.CompCode);

            if (costCenter != null)
            {
                withdrawHd.UseBrnName = costCenter.BrnName;
            }

            await context.InvWithdrawHds.AddAsync(withdrawHd);
        }

        public async Task AddDtAsync(List<InvWithdrawDt> withdrawDts)
        {
            await context.InvWithdrawDts.AddRangeAsync(withdrawDts);
        }

        public async Task<InvWithdrawHd> FindByIdAsync(Guid guid, string compcode, string brnCode, string locCode)
        {
            var withdraw = await context.InvWithdrawHds.FirstOrDefaultAsync(x => x.Guid == guid && x.CompCode == compcode && x.BrnCode == brnCode && x.LocCode == locCode);
            //withdraw.InvWithdrawDt = await context.InvWithdrawDts.Where(x => x.DocNo == withdraw.DocNo).ToListAsync();
            var withdrawQuery = await (from wd in context.InvWithdrawDts
                                       join pd in context.MasProducts on wd.PdId equals pd.PdId
                                       where wd.DocNo == withdraw.DocNo
                                       && wd.CompCode == compcode
                                       && wd.BrnCode == brnCode
                                       && wd.LocCode == locCode
                                       select new { wd, pd })
                              .AsNoTracking()
                              .ToListAsync();

            var withdrawDts = new List<InvWithdrawDt>();
            withdrawQuery.ForEach(x =>
            {
                var withdrawDt = new InvWithdrawDt
                {
                    CompCode = x.wd.CompCode,
                    BrnCode = x.wd.BrnCode,
                    LocCode = x.wd.LocCode,
                    DocNo = x.wd.DocNo,
                    SeqNo = x.wd.SeqNo,
                    PdId = x.wd.PdId,
                    PdName = x.wd.PdName,
                    UnitId = x.wd.UnitId,
                    UnitBarcode = x.wd.UnitBarcode,
                    UnitName = x.wd.UnitName,
                    ItemQty = x.wd.ItemQty,
                    StockQty = x.wd.StockQty,
                    GroupId = x.pd.GroupId
                };
                withdrawDts.Add(withdrawDt);
            });
            withdraw.InvWithdrawDt = withdrawDts;
            return withdraw;
        }

        public async Task<List<InvWithdrawDt>> GetDetailAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            List<InvWithdrawDt> items = new List<InvWithdrawDt>();
            items = await this.context.InvWithdrawDts.Where(x => x.CompCode == compCode
                                                                && x.BrnCode == brnCode
                                                                && x.LocCode == locCode
                                                                && x.DocNo == docNo).ToListAsync();
            return items;
        }


        public async Task<List<InvWithdrawDt>> GetDetailByDocNoAsync(string docNo)
        {
            List<InvWithdrawDt> items = new List<InvWithdrawDt>();
            items = await this.context.InvWithdrawDts.Where(x => x.DocNo == docNo).ToListAsync();
            return items;
        }


        public void UpdateAsync(InvWithdrawHd withdrawHd)
        {
            var costCenter = context.MasCostCenters.FirstOrDefault(x => x.BrnCode == withdrawHd.UseBrnCode  && x.CompCode == withdrawHd.CompCode);

            if (costCenter != null)
            {
                withdrawHd.UseBrnName = costCenter.BrnName;
            }
            context.InvWithdrawHds.Update(withdrawHd);
            context.SaveChanges();
        }

        public void AddDetailAsync(IEnumerable<InvWithdrawDt> withdrawDts)
        {
            context.InvWithdrawDts.AddRange(withdrawDts);
            context.SaveChanges();
        }

        public void RemoveDetailAsync(IEnumerable<InvWithdrawDt> withdrawDts)
        {
            context.InvWithdrawDts.RemoveRange(withdrawDts);
            context.SaveChanges();
        }

        public async Task<MasReason[]> GetReasons()
        {
            IQueryable<MasReason> qryReason = null;
            qryReason = context.MasReasons.AsNoTracking().Where(
                x => _strActive.Equals(x.ReasonStatus)
                && _strWithdraw.Equals(x.ReasonGroup)
            );
            MasReason[] result = null;
            result = await qryReason.ToArrayAsync();
            return result;
        }

        /*        
        public async Task<MasProduct[]> GetProducts(string pStrReasonId)
        {
            pStrReasonId = (pStrReasonId ?? string.Empty).Trim();
            if(0.Equals(pStrReasonId))
            {
                return null;
            }
            IQueryable<MasReasonGroup> qryReasonGroup = null;
            qryReasonGroup = context.MasReasonGroups
                .AsNoTracking()
                .Where(x=> pStrReasonId.Equals(x.ReasonId));
            IQueryable<MasProduct> qryProduct = null;
            qryProduct = context.MasProducts
                .AsNoTracking()
                .Where(x => qryReasonGroup.Any(y => y.GroupId == x.GroupId));
            string strQry = qryProduct.ToQueryString();
            MasProduct[] result = null;
            result = await qryProduct.ToArrayAsync();
            return result;
        }*/

        public async Task<MasReasonGroup[]> GetReasonGroups(string pStrReasonId)
        {
            pStrReasonId = (pStrReasonId ?? string.Empty).Trim();
            if (0.Equals(pStrReasonId))
            {
                return null;
            }
            IQueryable<MasReasonGroup> qryReasonGroup = null;
            qryReasonGroup = context.MasReasonGroups.AsNoTracking().Where(
                x => pStrReasonId.Equals(x.ReasonId)
                && _strWithdraw.Equals(x.ReasonGroup)
            );
            MasReasonGroup[] result = null;
            result = await qryReasonGroup.ToArrayAsync();
            return result;
        }

        private static T convertObject<T>(object pObjInput)
        {
            if (pObjInput == null)
            {
                return default(T);
            }
            var serialized = JsonConvert.SerializeObject(pObjInput);
            var result = JsonConvert.DeserializeObject<T>(serialized);
            return result;
        }

        public async Task<bool> CheckPOSWater(InvWithdrawHd withdraw)
        {
            var Water = await this.context.DopPosConfigs.FirstOrDefaultAsync(x => x.DocType == "Withdraw" && x.DocDesc == "Water");
            var config = await this.context.MasBranchConfigs.FirstOrDefaultAsync(x=>x.CompCode == withdraw.CompCode && x.BrnCode == withdraw.BrnCode);
            if(config != null && Water != null && config.IsPos == "Y")
            {                                  
                var result =  withdraw.InvWithdrawDt.Any(x => x.PdId.Contains(Water.PdId));  // ถ้าเป็นสาขา pos และ มีเลือกรหัสน้ำ  ไม่ให้บันทึก
                return result;
            }
            return false;  //ไม่เจอ  
        }

        public async Task CancelAsync(InvWithdrawHd withdrawHd)
        {
            withdrawHd.DocStatus = "Cancel";
            withdrawHd.UpdatedDate = DateTime.Now;
            context.InvWithdrawHds.Update(withdrawHd);
        }
    }
}
