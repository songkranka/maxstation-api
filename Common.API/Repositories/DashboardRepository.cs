using Common.API.Domain.Models;
using Common.API.Domain.Repositories;
using Common.API.Helpers;
using Common.API.Resource;
using Common.API.Resources;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Common.API.Repositories
{
    public class DashboardRepository : SqlDataAccessHelper, IDashboardRepository
    {
        public DashboardRepository(PTMaxstationContext context) : base(context)
        {
        }

        public async Task<QueryObjectResource<ProductDisplayResponse>> GetProductDisplay(RequestGetRequestList req)
        {
            try
            {

                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var periodDateTime = new DateTime(docDate.Year, docDate.Month, docDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);

                var result = new ProductDisplayResponse();

                var lasEffectiveDate = await context.OilStandardPriceHds.OrderByDescending(x => x.EffectiveDate).Where(x => x.EffectiveDate < docDate).Select(x => x.EffectiveDate).FirstOrDefaultAsync();

                result.LastEffectiveDate = lasEffectiveDate.Value;

                var masBranchtank = await context.MasBranchTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().ToListAsync();
                var groupMasBranchTank = masBranchtank.GroupBy(x => x.PdId).Select(x => new ProductDisplayItem
                {
                    PdId = x.First().PdId,
                    PdName = x.First().PdName,
                    PdImage = "",
                    Unitprice = 0
                }).ToList();

                groupMasBranchTank.ForEach(x => { x.PdImage = $"data:image/png;base64, {context.MasProducts.Where(y => y.PdId == x.PdId).Select(y => y.PdImage).AsNoTracking().FirstOrDefault()}"; });

                if (groupMasBranchTank.Count > 0) 
                {
                    groupMasBranchTank = GetCurrentOilPrice(groupMasBranchTank, req.CompCode, req.BrnCode, periodDateTime);
                }

                result.Items = groupMasBranchTank;

                return new QueryObjectResource<ProductDisplayResponse>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private List<ProductDisplayItem> GetCurrentOilPrice(List<ProductDisplayItem> groupMasBranchTank, string compCode, string brnCode, DateTime docDate)
        {
            var listOilPrice = new List<OilPrice>();

            //var sysDateMidNight = new DateTime(docDate.Year, docDate.Month, docDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
            string sysDateTime = docDate.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

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
                    var oilprice = new OilPrice();
                    oilprice.CompCode = Convert.ToString(reader["comp_code"]);
                    oilprice.BrnCode = Convert.ToString(reader["brn_code"]);
                    oilprice.PdId = Convert.ToString(reader["pd_id"]);
                    oilprice.CurrentPrice = Convert.ToDecimal(reader["current_price"]);

                    listOilPrice.Add(oilprice);
                }
            }

            foreach (var item in groupMasBranchTank)
            {
                item.Unitprice = listOilPrice.Where(x => x.PdId == item.PdId).Select(x => x.CurrentPrice).FirstOrDefault();
            }


            return groupMasBranchTank;
        }

        public async Task<QueryResultResource<InvRequestHd>> GetRequestList(RequestGetRequestList req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var queryable = context.InvRequestHds
                                 .Where(x => x.CompCode == req.CompCode && x.BrnCodeFrom == req.BrnCode && x.DocDate <= docDate && x.DocStatus == "Ready")
                                 .AsNoTracking();

                int totalItems = await queryable.CountAsync();
                var resp = await queryable.OrderByDescending(x => x.CreatedDate).Skip((req.Page - 1) * req.ItemsPerPage)
                                               .Take(req.ItemsPerPage)
                                               .ToListAsync();

                return new QueryResultResource<InvRequestHd>
                {
                    Items = resp,
                    TotalItems = totalItems,
                    ItemsPerPage = req.ItemsPerPage,
                    IsSuccess = true,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<QueryResultResource<InvTranoutHd>> GetTransferOutList(RequestGetRequestList req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var queryable = context.InvTranoutHds
                                 .Where(x => x.CompCode == req.CompCode && x.BrnCodeTo == req.BrnCode && x.DocDate <= docDate && x.DocStatus == "Active")
                                 .AsNoTracking();

                int totalItems = await queryable.CountAsync();
                var resp = await queryable.OrderByDescending(x => x.CreatedDate).Skip((req.Page - 1) * req.ItemsPerPage)
                                               .Take(req.ItemsPerPage)
                                               .ToListAsync();

                //find brn name
                var brnName = context.MasBranches.Where(x => x.BrnCode == resp.First().BrnCode).Select(x => x.BrnName).FirstOrDefault();
                resp.ForEach(x => { x.BrnName = brnName; });

                return new QueryResultResource<InvTranoutHd>
                {
                    Items = resp,
                    TotalItems = totalItems,
                    ItemsPerPage = req.ItemsPerPage,
                    IsSuccess = true,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
