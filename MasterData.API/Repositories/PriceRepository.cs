using MasterData.API.Domain.Models.Responses.Price;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class PriceRepository : SqlDataAccessHelper, IPriceRepository
    {
        public PriceRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<OilPrice> GetCurrentOilPriceAsync(string compCode, string brnCode, DateTime systemDate)
        {
            var response = new List<OilPrice>();

            var sysDateMidNight = new DateTime(systemDate.Year, systemDate.Month, systemDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
            string sysDateTime = sysDateMidNight.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

            //var _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            //string sysDateTime = docdate.ToString("dd/MM/yyyy HH:mm:ss", _curCulture);

            using (var cnn = context.Database.GetDbConnection())
            {
                var cmm = cnn.CreateCommand();
                //cmm.CommandType = System.Data.CommandType.StoredProcedure;
                //cmm.CommandText = "[dbo].[sp_GetCurrentOilPrice]";
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
                //cmm.Parameters.AddRange(param);
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

                    response.Add(oilprice);
                }
                //reader.NextResult(); //move the next record set
                //while (reader.Read())
                //{
                //    // result from table
                //}
            }
            

            return response;
        }
    }
}
