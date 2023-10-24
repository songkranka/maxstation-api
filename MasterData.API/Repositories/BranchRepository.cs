using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Models.Responses.Price;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class BranchRepository : SqlDataAccessHelper, IBranchRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchRepository));

        public BranchRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<MasBranch> GetBranchDropdownList(BranchDropdownRequest req)
        {
            var branchList = context.MasBranches.Where(
                x => (x.CompCode == req.CompCode)
            ).ToList();
            return branchList;
        }
        
        public MasBranch FindBranchByBrnCode(string brnCode)
        {
            var branch = context.MasBranches.FirstOrDefault(
                x => (x.BrnCode == brnCode)
            );
            return branch;
        }

        public List<MasBranch> FindBranchByCompCode(string compCode)
        {
            var branchList = context.MasBranches.Where(
                x => (x.CompCode == compCode)
            ).ToList();
            return branchList;
        }

        public List<MasBranch> FindBranchByAuthCode(string compCode,int? authenCode)
        {
            var branchList = (from m in context.AutBranchRoles
                              join b in context.MasBranches on new { m.CompCode, m.BrnCode } equals new { b.CompCode, b.BrnCode }
                              where (m.AuthCode == authenCode && m.CompCode == compCode)
                              select new MasBranch()
                              {
                                CompCode = b.CompCode,
                                BrnCode = b.BrnCode,
                                MapBrnCode = b.MapBrnCode,
                                LocCode = b.LocCode,
                                BrnName = b.BrnName,
                                BrnStatus = b.BrnStatus,
                                BranchNo = b.BranchNo,
                                Address = b.Address,
                                SubDistrict = b.SubDistrict,
                                District = b.District,
                                Province = b.Province,
                                Postcode = b.Postcode,
                                Phone = b.Phone,
                                Fax = b.Fax,
                                CreatedDate = b.CreatedDate,
                                CreatedBy = b.CreatedBy,
                                UpdatedBy = b.UpdatedBy,
                                UpdatedDate = b.UpdatedDate,
                              }).ToList();
            return branchList;
        }

        public List<MasBranch> GetBranchList(BranchRequest req)
        {
            List<MasBranch> branchList = new List<MasBranch>();
            branchList = this.context.MasBranches.Where(
                x => (x.CompCode == req.CompCode || req.CompCode == null || req.CompCode == "")
            ).ToList();
            return branchList;
        }

        public List<MasBranch> GetAuthBranchList(AuthBranchRequest req)
        {
            List<MasBranch> branchList = new List<MasBranch>();
            var empRole = context.AutEmployeeRoles.FirstOrDefault(x => x.EmpCode == req.Username);

            if (empRole != null)
            {
                var authBranchRoleQuery = (from br in context.AutBranchRoles.AsNoTracking()
                                           join mb in context.MasBranches.AsNoTracking()
                                           on br.BrnCode equals mb.BrnCode
                                           where (br.AuthCode == empRole.AuthCode)
                                              && (br.CompCode == req.CompCode)
                                              && (mb.CompCode == req.CompCode)
                                           select new { mb }).AsQueryable();
                var authBranchRoles = authBranchRoleQuery.ToList();
                branchList = authBranchRoles.Select(x => new MasBranch
                {
                    CompCode = x.mb.CompCode,
                    BrnCode = x.mb.BrnCode,
                    BrnName = x.mb.BrnName,
                    LocCode = x.mb.LocCode
                }).ToList();
            }
            return branchList;
        }
        public async Task<QueryResult<MasBranch>> List(BranchQuery pQuery)
        {
            var qryBranch = context.MasBranches
             .Where(x => x.CompCode == pQuery.CompCode)
             .OrderBy(x => x.BrnCode)
             .AsNoTracking();


            if (!string.IsNullOrEmpty(pQuery.Keyword))
            {
                qryBranch = qryBranch.Where(p => p.BrnCode.Contains(pQuery.Keyword)
                || p.BrnName.Contains(pQuery.Keyword));
            }

            int totalItems = await qryBranch.CountAsync();
            var resp = await qryBranch.Skip((pQuery.Page - 1) * pQuery.ItemsPerPage)
                                           .Take(pQuery.ItemsPerPage)
                                           .ToListAsync();
            foreach(var response in resp)
            {
                var company = await context.MasCompanies.FirstOrDefaultAsync(x => x.CompCode == response.CompCode && x.CompStatus == "Active");
                response.CompanyName = company == null ? string.Empty : company.CompName;
            }
            
            return new QueryResult<MasBranch>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = pQuery.ItemsPerPage
            };

        }

        public List<MasCompany> getCompanyDDL()
        {
            List<MasCompany> companyList = new List<MasCompany>();
            companyList = this.context.MasCompanies.Where(
                x => (x.CompStatus.Equals("Active"))
            ).ToList();
            return companyList;
        }

        public async Task<MasCompany> getCompany(string CompCode)
        {
            if (0.Equals(CompCode.Length))
            {
                return null;
            }

            var qryCompany = await context.MasCompanies.FirstOrDefaultAsync(x => x.CompCode == CompCode);
            return qryCompany;
        }

        public async Task<MasBranch> GetBranchByGuid(Guid guid)
        {
            var qryBranch = await context.MasBranches.FirstOrDefaultAsync(x => x.Guid == guid);
            return qryBranch;
        }

        public async Task<MasBranch> getBranchDetail(string brnCode)
        {
            var qryBranch = await context.MasBranches.FirstOrDefaultAsync(x => x.BrnCode == brnCode);
            var company = await context.MasCompanies.FirstOrDefaultAsync(x => x.CompCode == qryBranch.CompCode && x.CompStatus == "Active");
            qryBranch.CompanyName = company == null ? string.Empty : company.CompName;
            return qryBranch;
        }

        public async Task<MasBranch> GetBranchDetailByCompCodeAndBrnCode(string CompCode, string BrnCode)
        {
            if (0.Equals(CompCode.Length) || 0.Equals(BrnCode.Length))
            {
                return null;
            }

            var qryBranch = await context.MasBranches.FirstOrDefaultAsync(x => x.CompCode == CompCode && x.BrnCode == BrnCode);
            return qryBranch;
        }

        public List<MasBranchTank> getTankByBranch(string CompCode, string BrnCode)
        {
            List<MasBranchTank> branchTank = new List<MasBranchTank>();
            branchTank = this.context.MasBranchTanks.Where(
                x => (x.CompCode == CompCode && x.BrnCode == BrnCode)
            ).ToList();
            return branchTank;
        }

        public List<MasBranchDisp> getDispByBranch(string CompCode, string BrnCode)
        {
            List<MasBranchDisp> branchDisp = new List<MasBranchDisp>();
            branchDisp = this.context.MasBranchDisps.Where(
                x => (x.CompCode == CompCode && x.BrnCode == BrnCode)
                ).ToList();
            return branchDisp;
        }

        public List<MasBranchTax> getTaxByBranch(string CompCode, string BrnCode)
        {
            List<MasBranchTax> branchTax = new List<MasBranchTax>();
            branchTax = this.context.MasBranchTaxes.Where(
                x => (x.CompCode == CompCode && x.BrnCode == BrnCode)
                ).ToList();
            return branchTax;
        }
        
        public List<MasBranch> GetAllBranchByCompCode(string CompCode)
        {
            return context.MasBranches.Where(
                x => (x.CompCode == CompCode)
                ).ToList();
        }

        public void SaveBranch(MasBranch param)
        {
            if (param == null)
            {
                return;
            }

            param.UpdatedDate = DateTime.Now;
            EntityEntry<MasBranch> entBranch = null;
            entBranch = context.Update(param);

            string[] arrNoUpdateField = null;
            arrNoUpdateField = new[]
            {
                "CompCode","BrnCode","MapBrnCode",
                "CreatedDate","CreatedBy"
            };
            foreach (string strField in arrNoUpdateField)
            {
                entBranch.Property(strField).IsModified = false;
            }
        }

        public async Task<QueryResult<MeterResponse>> GetMasBranchDISP(BranchMeterRequest req)
        {
            try
            {
                var _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                //string sysDateTime = req.DocDate("dd/MM/yyyy HH:mm:ss", _curCulture);

                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", _curCulture);
                var periodDateTime = new DateTime(docDate.Year, docDate.Month, docDate.Day, 0, 0, 0);
                if (!string.IsNullOrEmpty(req.PeriodStart))
                {
                    var periodHour = Convert.ToInt32(req.PeriodStart.Split(':')[0]);
                    var periodMinute = Convert.ToInt32(req.PeriodStart.Split(':')[1]);
                    periodDateTime = periodDateTime.AddHours(periodHour).AddMinutes(periodMinute);
                }

                //get disp
                var queryableDisp = context.MasBranchDisps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DispStatus != "Cancel");
                var respDisp = await queryableDisp.OrderBy(x => x.DispId).ToListAsync();

                foreach (var item in respDisp)
                {
                    /*
                    var dopData = context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode
                    && x.BrnCode == req.BrnCode
                    && x.DocDate == docDate
                    && x.PeriodNo == (req.PeriodNo - 1)
                    //&& x.TankId == item.TankId
                    && x.DispId == item.DispId                   
                    //&& x.PdId == item.PdId
                    ).Select(x => x.MeterFinish).FirstOrDefault();
                    */

                    var qryDopMeter = context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode
                    && x.BrnCode == req.BrnCode
                    && x.DocDate == docDate
                    && x.PeriodNo == (req.PeriodNo - 1)
                    && x.DispId == item.DispId
                    );
                    var dopData = await qryDopMeter.Select(x => x.MeterFinish).FirstOrDefaultAsync();
                    if (dopData == null)
                    {
                        var yesterday = docDate.AddDays(-1);
                        var dopDataLYesterday = context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode
                        && x.BrnCode == req.BrnCode
                        && x.DocDate == yesterday
                        //&& x.TankId == item.TankId
                        && x.DispId == item.DispId
                        //&& x.PdId == item.PdId
                        ).OrderByDescending(x => x.PeriodNo).Select(x => x.MeterFinish).FirstOrDefault();

                        item.MeterStart = dopDataLYesterday ?? 0;
                    }
                    else
                    {
                        item.MeterStart = dopData.Value;
                    }
                }

                //get tank
                var queryableTank = context.MasBranchTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.TankStatus != "Cancel");
                var respTank = await queryableTank.OrderBy(x => x.TankId).ToListAsync();

                respTank.ForEach(x => { x.PdImage = $"data:image/png;base64, {context.MasProducts.Where(y => y.PdId == x.PdId).Select(y => y.PdImage).AsNoTracking().FirstOrDefault()}"; });

                foreach (var item in respTank)
                {
                    var dopData = context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode
                    && x.BrnCode == req.BrnCode
                    && x.DocDate == docDate
                    && x.PeriodNo == (req.PeriodNo - 1)
                    && x.TankId == item.TankId
                    ).Select(x => x.RealQty).FirstOrDefault();

                    if (dopData == null)
                    {
                        var yesterday = docDate.AddDays(-1);
                        var dopDataLYesterday = context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode
                        && x.BrnCode == req.BrnCode
                        && x.DocDate == yesterday
                        && x.TankId == item.TankId).OrderByDescending(x => x.PeriodNo).Select(x => x.RealQty).FirstOrDefault();

                        item.BeforeQty = dopDataLYesterday ?? 0;
                    }
                    else
                    {
                        item.BeforeQty = dopData.Value;
                    }
                }

                var periodStart = "";
                var periodFinish = "";
                if (req.PeriodStart == "")
                {
                    var masPeriod = await context.MasBranchPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.PeriodNo == req.PeriodNo).AsNoTracking().FirstOrDefaultAsync();                    

                    if (masPeriod != null)
                    {
                        periodStart = masPeriod.TimeStart.Replace('.', ':');
                        periodFinish = masPeriod.TimeFinish.Replace('.', ':');

                        var periodHour = Convert.ToInt32(periodStart.Split(':')[0]);
                        var periodMinute = Convert.ToInt32(periodStart.Split(':')[1]);
                        periodDateTime = periodDateTime.AddHours(periodHour).AddMinutes(periodMinute);
                    }
                }
               


                //get cash
                var queryableCashDr = context.DopPeriodGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.GlType == "DR" && x.GlStatus == "Active");
                var respCashDr = await queryableCashDr.OrderBy(x => x.GlNo).ToListAsync();

                var queryableCashCr = context.DopPeriodGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.GlType == "CR" && x.GlStatus == "Active");
                var respCashCr = await queryableCashCr.OrderBy(x => x.GlNo).ToListAsync();

                if (respTank.Count > 0)
                {
                    respTank = GetCurrentOilPrice(respTank, periodDateTime);
                }

                var result = new List<MeterResponse>()
                {
                    new MeterResponse()
                    {
                        PeriodStart = periodStart,
                        PeriodFinish = periodFinish,
                        MasBranchDispItems = respDisp,
                        MasBranchTankItems = respTank,
                        MasBranchCashDrItems = respCashDr,
                        MasBranchCashCrItems = respCashCr
                    }
                };

                return new QueryResult<MeterResponse>
                {
                    Items = result
                };
            }
            catch (Exception ex)
            {
                log.Error("Error in get mas branch disp", ex);
                throw new Exception(ex.Message);
            }
        }

        public List<MasBranchTank> GetCurrentOilPrice(List<MasBranchTank> masBranchTanks, DateTime docDate)
        {
            var listOilPrice = new List<Domain.Models.Responses.Price.OilPrice>();

            var brnCode = masBranchTanks.First().BrnCode;
            var compCode = masBranchTanks.First().CompCode;

            //var sysDateMidNight = new DateTime(docDate.Year, docDate.Month, docDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
            var _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            string sysDateTime = docDate.ToString("dd/MM/yyyy HH:mm:ss", _curCulture);

            using (var cnn = context.Database.GetDbConnection())
            {
                var cmm = cnn.CreateCommand();
                cmm.CommandText = $@"select xpri.comp_code, xpri.brn_code, xpri.pd_id
	                                     , case when xpro.pro_adj_price is null
			                                    then (xpri.before_price + xpri.adjust_price) 
			                                    else (xpri.before_price + xpro.pro_adj_price) + xpri.adjust_price end current_price
                                    from (
                                        select a.doc_no, a.comp_code, a.brn_code, b.pd_id, b.before_price, b.adjust_price 
                                        from dbo.oil_standard_price_hd a, dbo.oil_standard_price_dt b
                                        where a.brn_code = '{ brnCode }'	
                                          and a.comp_code = '{ compCode }'		
                                          and convert(varchar(10),a.effective_date,105) = 
                                            (
                                            select convert(varchar(10),max(c.effective_date),105)
                                            from dbo.oil_standard_price_hd c
                                            where c.effective_date <= convert(datetime, '{ sysDateTime }', 103)	
                                              and c.approve_status = 'Y'
                                              and c.brn_code = a.brn_code
                                              and c.doc_type = a.doc_type
                                              and c.comp_code = a.comp_code
                                            )
                                          and a.doc_no = b.doc_no
                                          and a.brn_code = b.brn_code
                                          and a.comp_code = b.comp_code
                                     ) xpri left join
                                     (
                                        select a.comp_code, a.brn_code, b.pd_id, sum(b.adjust_price) as pro_adj_price
                                        from dbo.oil_promotion_price_hd a, dbo.oil_promotion_price_dt b
                                        where a.brn_code = '{ brnCode }'		-- parameter: branch
                                          and a.comp_code = '{ brnCode }'		-- parameter: branch
                                          and a.doc_no in
                                            (
                                            select c.doc_no
                                            from dbo.oil_promotion_price_hd c
                                            where ( c.start_date <= convert(datetime, '{ sysDateTime }', 103) 	
                                              and c.finish_date >= convert(datetime, '{ sysDateTime }', 103) )	
                                              and c.approve_status = 'Y'
                                              and c.brn_code = a.brn_code
                                              and c.comp_code = a.comp_code
                                            )
                                          and a.doc_no = b.doc_no
                                          and a.brn_code = b.brn_code
                                          and a.comp_code = b.comp_code
                                        group by a.comp_code, a.brn_code, b.pd_id
                                     ) xpro 
                                    on xpri.brn_code = xpro.brn_code 
                                    and xpri.pd_id = xpro.pd_id
                                    and xpri.comp_code = xpro.comp_code;";

                cmm.Connection = cnn;
                cnn.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    var oilprice = new Domain.Models.Responses.Price.OilPrice();
                    oilprice.CompCode = Convert.ToString(reader["comp_code"]);
                    oilprice.BrnCode = Convert.ToString(reader["brn_code"]);
                    oilprice.PdId = Convert.ToString(reader["pd_id"]);
                    oilprice.CurrentPrice = Convert.ToDecimal(reader["current_price"]);

                    listOilPrice.Add(oilprice);
                }
            }

            foreach (var item in masBranchTanks)
            {
                item.Unitprice = listOilPrice.Where(x => x.CompCode == item.CompCode && x.BrnCode == item.BrnCode && x.PdId == item.PdId).Select(x => x.CurrentPrice).FirstOrDefault();
            }


            return masBranchTanks;
        }

        public async Task<string> GetQueryOilPrice(BranchMeterRequest req)
        {
            var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var periodDateTime = new DateTime(docDate.Year, docDate.Month, docDate.Day, 0, 0, 0);
            var listOilPrice = new List<Domain.Models.Responses.Price.OilPrice>();

            var brnCode = req.BrnCode;
            var compCode = req.CompCode;
            var masPeriod = await context.MasBranchPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.PeriodNo == req.PeriodNo).AsNoTracking().FirstOrDefaultAsync();
            var periodStart = "";
            var periodFinish = "";
            if (masPeriod != null)
            {
                periodStart = masPeriod.TimeStart.Replace('.', ':');
                periodFinish = masPeriod.TimeFinish.Replace('.', ':');

                var periodHour = Convert.ToInt32(periodStart.Split(':')[0]);
                var periodMinute = Convert.ToInt32(periodStart.Split(':')[1]);
                periodDateTime = periodDateTime.AddHours(periodHour).AddMinutes(periodMinute);
            }
            //var sysDateMidNight = new DateTime(docDate.Year, docDate.Month, docDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
            var _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            string sysDateTime = docDate.ToString("dd/MM/yyyy HH:mm:ss", _curCulture);
            string result = $@"select xpri.comp_code, xpri.brn_code, xpri.pd_id
	                                     , case when xpro.pro_adj_price is null
			                                    then (xpri.before_price + xpri.adjust_price) 
			                                    else (xpri.before_price + xpro.pro_adj_price) + xpri.adjust_price end current_price
                                    from (
                                        select a.doc_no, a.comp_code, a.brn_code, b.pd_id, b.before_price, b.adjust_price 
                                        from dbo.oil_standard_price_hd a, dbo.oil_standard_price_dt b
                                        where a.brn_code = '{ brnCode }'	
                                          and a.comp_code = '{ compCode }'		
                                          and convert(varchar(10),a.effective_date,105) = 
                                            (
                                            select convert(varchar(10),max(c.effective_date),105)
                                            from dbo.oil_standard_price_hd c
                                            where c.effective_date <= convert(datetime, '{ sysDateTime }', 103)	
                                              and c.approve_status = 'Y'
                                              and c.brn_code = a.brn_code
                                              and c.doc_type = a.doc_type
                                              and c.comp_code = a.comp_code
                                            )
                                          and a.doc_no = b.doc_no
                                          and a.brn_code = b.brn_code
                                          and a.comp_code = b.comp_code
                                     ) xpri left join
                                     (
                                        select a.comp_code, a.brn_code, b.pd_id, sum(b.adjust_price) as pro_adj_price
                                        from dbo.oil_promotion_price_hd a, dbo.oil_promotion_price_dt b
                                        where a.brn_code = '{ brnCode }'		-- parameter: branch
                                          and a.comp_code = '{ brnCode }'		-- parameter: branch
                                          and a.doc_no in
                                            (
                                            select c.doc_no
                                            from dbo.oil_promotion_price_hd c
                                            where ( c.start_date <= convert(datetime, '{ sysDateTime }', 103) 	
                                              and c.finish_date >= convert(datetime, '{ sysDateTime }', 103) )	
                                              and c.approve_status = 'Y'
                                              and c.brn_code = a.brn_code
                                              and c.comp_code = a.comp_code
                                            )
                                          and a.doc_no = b.doc_no
                                          and a.brn_code = b.brn_code
                                          and a.comp_code = b.comp_code
                                        group by a.comp_code, a.brn_code, b.pd_id
                                     ) xpro 
                                    on xpri.brn_code = xpro.brn_code 
                                    and xpri.pd_id = xpro.pd_id
                                    and xpri.comp_code = xpro.comp_code;";
            return result;
        }
        public async Task<MasBranchConfig> GetMasBranchConfig(string pStrCompCode , string pStrBrnCode)
        {
            var qryBranchConfig = context.MasBranchConfigs
                .Where(x => x.CompCode == pStrCompCode && x.BrnCode == pStrBrnCode)
                .AsNoTracking();
            var result = await qryBranchConfig.FirstOrDefaultAsync();
            return result;
        }

        public async Task<MasBranch> FindByGuidAsync(Guid? guid)
        {
            return await context.MasBranches.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == guid);
        }

        public async Task AddBranchAsync(MasBranch masBranch)
        {
            await context.MasBranches.AddAsync(masBranch);
        }

        public async Task UpdateBranchAsync(MasBranch masBranch)
        {
            var result = await context.MasBranches.SingleOrDefaultAsync(x => x.Guid == masBranch.Guid);
            
            if (result != null)
            {
                result.MapBrnCode = masBranch.MapBrnCode;
                result.BrnName = masBranch.BrnName;
                result.BrnStatus = masBranch.BrnStatus;
                result.BranchNo = masBranch.BranchNo;
                result.Address = masBranch.Address;
                result.SubDistrict = masBranch.SubDistrict;
                result.District = masBranch.District;
                result.ProvCode = masBranch.ProvCode;
                result.Province = masBranch.Province;
                result.Postcode = masBranch.Postcode;
                result.Phone = masBranch.Phone;
                result.Fax = masBranch.Fax;
                result.PosCount= masBranch.PosCount;
                result.CloseDate = masBranch.CloseDate;
                result.UpdatedDate = DateTime.Now;
                context.SaveChanges();
            }
        }

        public async Task<MasBranchTank> FindByCompCodeAndBrnCodeAndTankIdAsync(string compCode, string brnCode, string tankId)
        {
            return await context.MasBranchTanks.AsNoTracking().FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode && x.TankId == tankId);
        }

        public async Task<MasBranchTank> FindByCompCodeAndBrnCodeAsync(string compCode, string brnCode)
        {
            return await context.MasBranchTanks.AsNoTracking().FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode);
        }

        public async Task AddBranchTankAsync(MasBranchTank masBranchTank)
        {
            await context.MasBranchTanks.AddAsync(masBranchTank);
        }

        public async Task UpdateBranchTankAsync(MasBranchTank masBranchTank)
        {
            var result = await context.MasBranchTanks.SingleOrDefaultAsync(x => x.CompCode == masBranchTank.CompCode && x.BrnCode == masBranchTank.BrnCode && x.TankId == masBranchTank.TankId);

            if (result != null)
            {
                result.TankStatus = masBranchTank.TankStatus;
                result.PdId = masBranchTank.PdId;
                result.PdName = masBranchTank.PdName;
                result.Capacity = masBranchTank.Capacity;
                result.CapacityMin = masBranchTank.CapacityMin;
                result.UpdatedDate = masBranchTank.UpdatedDate;
                result.UpdatedBy = masBranchTank.UpdatedBy;
                context.SaveChanges();
            }
        }

        public async Task<MasBranchDisp> FindByCompCodeAndBrnCodeAndDispIdAsync(string compCode, string brnCode, string dispId)
        {
            return await context.MasBranchDisps.AsNoTracking().FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode && x.DispId == dispId);
        }

        public async Task<MasBranchDisp> FindMasBranchDispByCompCodeAndBrnCodeAsync(string compCode, string brnCode)
        {
            return await context.MasBranchDisps.AsNoTracking().FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode);
        }

        public async Task AddBranchDispAsync(MasBranchDisp masBranchDisp)
        {
            await context.MasBranchDisps.AddAsync(masBranchDisp);
        }

        public async Task UpdateBranchDispAsync(MasBranchDisp masBranchDisp)
        {
            var result = await context.MasBranchDisps.SingleOrDefaultAsync(x => x.CompCode == masBranchDisp.CompCode && x.BrnCode == masBranchDisp.BrnCode && x.DispId == masBranchDisp.DispId);

            if (result != null)
            {
                result.DispStatus = masBranchDisp.DispStatus;
                result.MeterMax = masBranchDisp.MeterMax;
                result.SerialNo = masBranchDisp.SerialNo;
                result.TankId = masBranchDisp.TankId;
                result.PdId = masBranchDisp.PdId;
                result.PdName = masBranchDisp.PdName;
                result.UnitId = masBranchDisp.UnitId;
                result.UnitBarcode = masBranchDisp.UnitBarcode;
                result.HoseId = masBranchDisp.HoseId;
                result.UpdatedDate = masBranchDisp.UpdatedDate;
                result.UpdatedBy = masBranchDisp.UpdatedBy;
                context.SaveChanges();
            }
        }

        public async Task AddBranchConfigAsync(MasBranchConfig masBranchConfig)
        {
            await context.MasBranchConfigs.AddAsync(masBranchConfig);
        }


        public async Task UpdateBranchConfigAsync(MasBranchConfig masBranchConfig)
        {
            var result = await context.MasBranchConfigs.SingleOrDefaultAsync(x => x.CompCode == masBranchConfig.CompCode && x.BrnCode == masBranchConfig.BrnCode);

            if (result != null)
            {
                result.Trader = masBranchConfig.Trader;
                result.TraderPosition = masBranchConfig.TraderPosition;
                result.ReportTaxType = masBranchConfig.ReportTaxType;
                result.UpdatedDate = masBranchConfig.UpdatedDate;
                result.UpdatedBy = masBranchConfig.UpdatedBy;
                context.SaveChanges();
            }
        }

        public async Task<MasBranchTax> FindByCompCodeAndBrnCodeAndTaxIdAsync(string compCode, string brnCode)
        {
            return await context.MasBranchTaxes.AsNoTracking().FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode);
        }

        public async Task AddBranchTaxAsync(MasBranchTax masBranchTax)
        {
            await context.MasBranchTaxes.AddAsync(masBranchTax);
        }

        public async Task UpdateBranchTaxAsync(MasBranchTax masBranchTax)
        {
            var result = await context.MasBranchTaxes.SingleOrDefaultAsync(x => x.CompCode == masBranchTax.CompCode && x.BrnCode == masBranchTax.BrnCode && x.TaxId == masBranchTax.TaxId);

            if (result != null)
            {
                result.TaxName = masBranchTax.TaxName;
                result.TaxAmt = masBranchTax.TaxAmt;
                result.UpdatedDate = masBranchTax.UpdatedDate;
                result.UpdatedBy = masBranchTax.UpdatedBy;
                context.SaveChanges();
            }
        }

        public async Task DeleteBranchTaxAsync(string compCode, string brnCode)
        {
            var result = await context.MasBranchTaxes.Where(x => x.CompCode == compCode && x.BrnCode == brnCode).ToListAsync();

            if (result != null)
            {
                context.MasBranchTaxes.RemoveRange(result);
                context.SaveChanges();
            }
        }

        public async Task DeleteBranchTankAsync(string compCode, string brnCode)
        {
            var result = await context.MasBranchTanks.Where(x => x.CompCode == compCode && x.BrnCode == brnCode).ToListAsync();

            if (result != null)
            {
                context.MasBranchTanks.RemoveRange(result);
                context.SaveChanges();
            }
        }

        public async Task DeleteBranchDispAsync(string compCode, string brnCode)
        {
            var result = await context.MasBranchDisps.Where(x => x.CompCode == compCode && x.BrnCode == brnCode).ToListAsync();

            if (result != null)
            {
                context.MasBranchDisps.RemoveRange(result);
                context.SaveChanges();
            }
        }
    }
}
