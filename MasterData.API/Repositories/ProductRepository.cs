using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MasterData.API.Resources.Product;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class ProductRepository : SqlDataAccessHelper, IProductRepository
    {
        public ProductRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<Products> GetProductAllTypeList(ProductRequest req)
        {
            //ตรวจสอบกรณีมีการส่ง PDListID มาใช้ IN
            List<string> pdIdList = new List<string>();
            if (req.PDListID != null && req.PDListID != "")
            {
                pdIdList = req.PDListID.Split(',').ToList();
            }

            //ตรวจสอบกรณีมีการส่ง PDBarcodeList มาใช้ IN
            List<string> pdBarcodeList = new List<string>();
            if (req.PDBarcodeList != null && req.PDBarcodeList != "")
            {
                pdBarcodeList = req.PDBarcodeList.Split(',').ToList();
            }

            var sql = (from pd in this.context.MasProducts
                       join pp in this.context.MasProductPrices on new { pd.PdId } equals new { pp.PdId }
                       join un in this.context.MasUnits on pp.UnitId equals un.UnitId
                       where (pd.PdName.Contains(req.Keyword) || pd.PdId.Contains(req.Keyword) || pd.UnitName.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                            && (pdIdList.Contains(pd.PdId) || req.PDListID == null || req.PDListID == "")
                            && (pdBarcodeList.Contains(pp.UnitBarcode) || req.PDBarcodeList == null || req.PDBarcodeList == "")
                            && (pp.CompCode == req.CompCode || req.CompCode == null || req.CompCode == "")
                            && (pp.LocCode == req.LocCode || req.LocCode == null || req.LocCode == "")
                            && (pp.BrnCode == req.BrnCode || req.BrnCode == null || req.BrnCode == "")
                            && pd.PdStatus == "Active" && pp.PdStatus == "Active"
                            && pd.GroupId != "0000"
                       select new { pd, pp, un }).AsQueryable();

            List<Products> resp = sql
              .GroupBy(x => new
              {
                  x.pd.PdId,
                  x.pd.PdName,
                  x.pd.PdStatus,
                  x.pd.PdDesc,
                  x.un.UnitId,
                  x.un.UnitName,
                  x.pd.GroupId,
                  x.pd.VatType,
                  x.pd.VatRate,
                  x.pp.UnitBarcode,
                  x.pp.Unitprice
              })
              .Select(x => new Products
              {
                  PdId = x.Key.PdId,
                  PdName = x.Key.PdName,
                  PdStatus = x.Key.PdStatus,
                  PdDesc = x.Key.PdDesc,
                  UnitId = x.Key.UnitId,
                  UnitName = x.Key.UnitName,
                  GroupId = x.Key.GroupId,
                  VatType = x.Key.VatType,
                  VatRate = x.Key.VatRate,
                  UnitBarcode = x.Key.UnitBarcode,
                  UnitPrice = decimal.Parse(x.Key.Unitprice.ToString())
              }).ToList();

            var currentOilPrices = new List<Products>();

            if (req.SystemDate != DateTime.MinValue)
            {
                var sysDateMidNight = new DateTime(req.SystemDate.Year, req.SystemDate.Month, req.SystemDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
                string sysDateTime = sysDateMidNight.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                using (var cnn = context.Database.GetDbConnection())
                {
                    var cmm = cnn.CreateCommand();
                    cmm.CommandText = $@"select p.PD_ID, p.PD_NAME ,p.PD_STATUS,p.PD_DESC,p.UNIT_ID,pu.UNIT_NAME,p.GROUP_ID,p.VAT_TYPE,p.VAT_RATE,pu.UNIT_BARCODE,oil.current_price as UnitPrice
                                    --,oil.*
                                    from 
                                    (
                                      select xpri.comp_code, xpri.brn_code, xpri.pd_id
                                        , case when xpro.pro_adj_price is null
                                         then (xpri.before_price + xpri.adjust_price) 
                                         else (xpri.before_price + xpro.pro_adj_price) + xpri.adjust_price end current_price
                                      from 
                                       (
                                       select a.doc_no, a.comp_code, a.brn_code, b.pd_id, b.before_price, b.adjust_price 
                                       from dbo.oil_standard_price_hd a, dbo.oil_standard_price_dt b
                                       where a.brn_code = '{ req.BrnCode }'
                                        and a.comp_code = '{ req.CompCode }' 
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
                                       where a.brn_code = '{ req.BrnCode }'  -- parameter: branch
                                        and a.comp_code = '{ req.CompCode }'   -- parameter: compcode
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
                                      and xpri.comp_code = xpro.comp_code
                                    ) oil
                                    inner join MAS_PRODUCT p  on oil.PD_ID = p.PD_ID
                                    inner join MAS_PRODUCT_UNIT pu on p.PD_ID  = pu.PD_ID and p.UNIT_ID = pu.UNIT_ID";
                    cmm.Connection = cnn;
                    cnn.Open();
                    var reader = cmm.ExecuteReader();

                    while (reader.Read())
                    {
                        var productOilprice = new Products();
                        productOilprice.PdId = Convert.ToString(reader["PD_ID"]);
                        productOilprice.PdName = Convert.ToString(reader["PD_NAME"]);
                        productOilprice.PdStatus = Convert.ToString(reader["PD_STATUS"]);
                        productOilprice.PdDesc = Convert.ToString(reader["PD_DESC"]);
                        productOilprice.UnitId = Convert.ToString(reader["UNIT_ID"]);
                        productOilprice.UnitName = Convert.ToString(reader["UNIT_NAME"]);
                        productOilprice.GroupId = Convert.ToString(reader["GROUP_ID"]);
                        productOilprice.VatType = Convert.ToString(reader["VAT_TYPE"]);
                        productOilprice.VatRate = Convert.ToInt32(reader["VAT_RATE"]);
                        productOilprice.UnitBarcode = Convert.ToString(reader["UNIT_BARCODE"]);
                        productOilprice.UnitPrice = Convert.ToDecimal(reader["UnitPrice"]);

                        currentOilPrices.Add(productOilprice);
                    }
                }

                resp.AddRange(currentOilPrices);
                //                resp.OrderBy(p => p.PdId);
            }

            return resp.OrderBy(p => p.PdId).ToList();
        }
        public List<Products> GetProductList(ProductRequest req)
        {
            //ตรวจสอบกรณีมีการส่ง PDListID มาใช้ IN
            List<string> pdIdList = new List<string>();
            if (req.PDListID != null && req.PDListID != "")
            {
                pdIdList = req.PDListID.Split(',').ToList();
            }

            //ตรวจสอบกรณีมีการส่ง PDBarcodeList มาใช้ IN
            List<string> pdBarcodeList = new List<string>();
            if (req.PDBarcodeList != null && req.PDBarcodeList != "")
            {
                pdBarcodeList = req.PDBarcodeList.Split(',').ToList();
            }

            var sql = (from pd in this.context.MasProducts
                       join pp in this.context.MasProductPrices on new { pd.PdId } equals new { pp.PdId }
                       join pt in this.context.MasProductTypes on pd.GroupId equals pt.GroupId
                       join un in this.context.MasUnits on pp.UnitId equals un.UnitId
                       where (pd.PdName.Contains(req.Keyword) || pd.PdId.Contains(req.Keyword) || pd.UnitName.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                            && (pdIdList.Contains(pd.PdId) || req.PDListID == null || req.PDListID == "")
                            && (pdBarcodeList.Contains(pp.UnitBarcode) || req.PDBarcodeList == null || req.PDBarcodeList == "")
                            && (pt.DocTypeId == req.DocumentTypeID || req.DocumentTypeID == null || req.DocumentTypeID == "")
                            && (pp.CompCode == req.CompCode || req.CompCode == null || req.CompCode == "")
                            //&& (pp.LocCode == req.LocCode || req.LocCode == null || req.LocCode == "")
                            && (pp.BrnCode == req.BrnCode || req.BrnCode == null || req.BrnCode == "")
                            && pd.PdStatus == "Active" && pp.PdStatus == "Active"
                            && pd.GroupId != "0000"
                       select new { pd, pp, un }).AsQueryable();

            List<Products> resp = sql
              .GroupBy(x => new
              {
                  x.pd.PdId,
                  x.pd.PdName,
                  x.pd.PdStatus,
                  x.pd.PdDesc,
                  x.un.UnitId,
                  x.un.UnitName,
                  x.pd.GroupId,
                  x.pd.VatType,
                  x.pd.VatRate,
                  x.pp.UnitBarcode,
                  x.pp.Unitprice
              })
              .Select(x => new Products
              {
                  PdId = x.Key.PdId,
                  PdName = x.Key.PdName,
                  PdStatus = x.Key.PdStatus,
                  PdDesc = x.Key.PdDesc,
                  UnitId = x.Key.UnitId,
                  UnitName = x.Key.UnitName,
                  GroupId = x.Key.GroupId,
                  VatType = x.Key.VatType,
                  VatRate = x.Key.VatRate,
                  UnitBarcode = x.Key.UnitBarcode,
                  UnitPrice = decimal.Parse(x.Key.Unitprice.ToString())
              }).ToList();


            var currentOilPrices = new List<Products>();

            if (req.SystemDate != DateTime.MinValue)
            {
                var sysDateMidNight = new DateTime(req.SystemDate.Year, req.SystemDate.Month, req.SystemDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
                string sysDateTime = sysDateMidNight.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                using (var cnn = context.Database.GetDbConnection())
                {
                    var cmm = cnn.CreateCommand();
                    cmm.CommandText = $@"select p.PD_ID, p.PD_NAME ,p.PD_STATUS,p.PD_DESC,p.UNIT_ID,pu.UNIT_NAME,p.GROUP_ID,p.VAT_TYPE,p.VAT_RATE,pu.UNIT_BARCODE,oil.current_price as UnitPrice
                                    --,oil.*
                                    from 
                                    (
                                      select xpri.comp_code, xpri.brn_code, xpri.pd_id
                                        , case when xpro.pro_adj_price is null
                                         then (xpri.before_price + xpri.adjust_price) 
                                         else (xpri.before_price + xpro.pro_adj_price) + xpri.adjust_price end current_price
                                      from 
                                       (
                                       select a.doc_no, a.comp_code, a.brn_code, b.pd_id, b.before_price, b.adjust_price 
                                       from dbo.oil_standard_price_hd a, dbo.oil_standard_price_dt b
                                       where a.brn_code = '{ req.BrnCode }'
                                        and a.comp_code = '{ req.CompCode }' 
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
                                       where a.brn_code = '{ req.BrnCode }'  -- parameter: branch
                                        and a.comp_code = '{ req.CompCode }'   -- parameter: compcode
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
                                      and xpri.comp_code = xpro.comp_code
                                    ) oil
                                    inner join MAS_PRODUCT p  on oil.PD_ID = p.PD_ID
                                    inner join MAS_PRODUCT_UNIT pu on p.PD_ID  = pu.PD_ID and p.UNIT_ID = pu.UNIT_ID";
                    cmm.Connection = cnn;
                    cnn.Open();
                    var reader = cmm.ExecuteReader();

                    while (reader.Read())
                    {
                        var productOilprice = new Products();
                        productOilprice.PdId = Convert.ToString(reader["PD_ID"]);
                        productOilprice.PdName = Convert.ToString(reader["PD_NAME"]);
                        productOilprice.PdStatus = Convert.ToString(reader["PD_STATUS"]);
                        productOilprice.PdDesc = Convert.ToString(reader["PD_DESC"]);
                        productOilprice.UnitId = Convert.ToString(reader["UNIT_ID"]);
                        productOilprice.UnitName = Convert.ToString(reader["UNIT_NAME"]);
                        productOilprice.GroupId = Convert.ToString(reader["GROUP_ID"]);
                        productOilprice.VatType = Convert.ToString(reader["VAT_TYPE"]);
                        productOilprice.VatRate = Convert.ToInt32(reader["VAT_RATE"]);
                        productOilprice.UnitBarcode = Convert.ToString(reader["UNIT_BARCODE"]);
                        productOilprice.UnitPrice = Convert.ToDecimal(reader["UnitPrice"]);

                        currentOilPrices.Add(productOilprice);
                    }
                }

                var a = JsonConvert.SerializeObject(currentOilPrices);
                resp.AddRange(currentOilPrices);
                //resp.OrderBy(p => p.PdId);
            }
            return resp.OrderBy(x => x.PdId).ToList();
        }

        public List<Products> GetProductListWithDocumentType(ProductRequest req)
        {
            var sql = (from pd in this.context.MasProducts
                       join pt in this.context.MasProductTypes on pd.GroupId equals pt.GroupId
                       join pu in this.context.MasProductUnits on new { pd.PdId, pd.UnitId } equals new { pu.PdId, pu.UnitId }
                       where (pd.PdName.Contains(req.Keyword) || pd.PdId.Contains(req.Keyword) || pd.UnitName.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                            && (pt.DocTypeId == req.DocumentTypeID || req.DocumentTypeID == null || req.DocumentTypeID == "")
                            && pd.PdStatus == "Active"
                       select new { pd, pu }).AsQueryable();

            List<Products> resp = sql
              .GroupBy(x => new
              {
                  x.pd.PdId,
                  x.pd.PdName,
                  x.pd.PdStatus,
                  x.pd.PdDesc,
                  x.pu.UnitId,
                  x.pu.UnitName,
                  x.pu.UnitBarcode,
                  x.pd.GroupId,
                  x.pd.VatType,
                  x.pd.VatRate
              })
              .Select(x => new Products
              {
                  PdId = x.Key.PdId,
                  PdName = x.Key.PdName,
                  PdStatus = x.Key.PdStatus,
                  PdDesc = x.Key.PdDesc,
                  UnitId = x.Key.UnitId,
                  UnitName = x.Key.UnitName,
                  UnitBarcode = x.Key.UnitBarcode,
                  GroupId = x.Key.GroupId,
                  VatType = x.Key.VatType,
                  VatRate = x.Key.VatRate
              }).ToList();
            return resp.OrderBy(x => x.PdId).ToList();
        }

        public List<Products> GetProductListWithOutMaterialCode(ProductRequest req)
        {
            //ตรวจสอบกรณีมีการส่ง PDListID มาใช้ IN
            List<string> pdIdList = new List<string>();
            if (req.PDListID != null && req.PDListID != "")
            {
                pdIdList = req.PDListID.Split(',').ToList();
            }

            //ตรวจสอบกรณีมีการส่ง PDBarcodeList มาใช้ IN
            List<string> pdBarcodeList = new List<string>();
            if (req.PDBarcodeList != null && req.PDBarcodeList != "")
            {
                pdBarcodeList = req.PDBarcodeList.Split(',').ToList();
            }

            var sql = (from pd in this.context.MasProducts
                       join pp in this.context.MasProductPrices on new { pd.PdId } equals new { pp.PdId }
                       join pt in this.context.MasProductTypes on pd.GroupId equals pt.GroupId
                       join un in this.context.MasUnits on pp.UnitId equals un.UnitId
                       where (pd.PdName.Contains(req.Keyword) || pd.PdId.Contains(req.Keyword) || pd.UnitName.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                            && (pdIdList.Contains(pd.PdId) || req.PDListID == null || req.PDListID == "")
                            && (pdBarcodeList.Contains(pp.UnitBarcode) || req.PDBarcodeList == null || req.PDBarcodeList == "")
                            && (pt.DocTypeId == req.DocumentTypeID || req.DocumentTypeID == null || req.DocumentTypeID == "")
                            && (pp.CompCode == req.CompCode || req.CompCode == null || req.CompCode == "")
                            && (pp.LocCode == req.LocCode || req.LocCode == null || req.LocCode == "")
                            && (pp.BrnCode == req.BrnCode || req.BrnCode == null || req.BrnCode == "")
                            && pd.PdStatus == "Active" && pp.PdStatus == "Active"
                            && pt.DocTypeId != "003"
                            && pd.GroupId != "8400"
                            && pd.GroupId != "0000"
                       select new { pd, pp, un }).AsQueryable();

            List<Products> resp = sql
              .GroupBy(x => new
              {
                  x.pd.PdId,
                  x.pd.PdName,
                  x.pd.PdStatus,
                  x.pd.PdDesc,
                  x.un.UnitId,
                  x.un.UnitName,
                  x.pd.GroupId,
                  x.pd.VatType,
                  x.pd.VatRate,
                  x.pp.UnitBarcode,
                  x.pp.Unitprice
              })
              .Select(x => new Products
              {
                  PdId = x.Key.PdId,
                  PdName = x.Key.PdName,
                  PdStatus = x.Key.PdStatus,
                  PdDesc = x.Key.PdDesc,
                  UnitId = x.Key.UnitId,
                  UnitName = x.Key.UnitName,
                  GroupId = x.Key.GroupId,
                  VatType = x.Key.VatType,
                  VatRate = x.Key.VatRate,
                  UnitBarcode = x.Key.UnitBarcode,
                  UnitPrice = decimal.Parse(x.Key.Unitprice.ToString())
              }).ToList();

            var currentOilPrices = new List<Products>();

            if (req.SystemDate != DateTime.MinValue)
            {
                var sysDateMidNight = new DateTime(req.SystemDate.Year, req.SystemDate.Month, req.SystemDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
                string sysDateTime = sysDateMidNight.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                using (var cnn = context.Database.GetDbConnection())
                {
                    var cmm = cnn.CreateCommand();
                    cmm.CommandText = $@"select p.PD_ID, p.PD_NAME ,p.PD_STATUS,p.PD_DESC,p.UNIT_ID,pu.UNIT_NAME,p.GROUP_ID,p.VAT_TYPE,p.VAT_RATE,pu.UNIT_BARCODE,oil.current_price as UnitPrice
                                    --,oil.*
                                    from 
                                    (
                                      select xpri.comp_code, xpri.brn_code, xpri.pd_id
                                        , case when xpro.pro_adj_price is null
                                         then (xpri.before_price + xpri.adjust_price) 
                                         else (xpri.before_price + xpro.pro_adj_price) + xpri.adjust_price end current_price
                                      from 
                                       (
                                       select a.doc_no, a.comp_code, a.brn_code, b.pd_id, b.before_price, b.adjust_price 
                                       from dbo.oil_standard_price_hd a, dbo.oil_standard_price_dt b
                                       where a.brn_code = '{ req.BrnCode }'
                                        and a.comp_code = '{ req.CompCode }' 
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
                                       where a.brn_code = '{ req.BrnCode }'  -- parameter: branch
                                        and a.comp_code = '{ req.CompCode }'   -- parameter: compcode
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
                                      and xpri.comp_code = xpro.comp_code
                                    ) oil
                                    inner join MAS_PRODUCT p  on oil.PD_ID = p.PD_ID
                                    inner join MAS_PRODUCT_UNIT pu on p.PD_ID  = pu.PD_ID and p.UNIT_ID = pu.UNIT_ID";
                    cmm.Connection = cnn;
                    cnn.Open();
                    var reader = cmm.ExecuteReader();

                    while (reader.Read())
                    {
                        var productOilprice = new Products();
                        productOilprice.PdId = Convert.ToString(reader["PD_ID"]);
                        productOilprice.PdName = Convert.ToString(reader["PD_NAME"]);
                        productOilprice.PdStatus = Convert.ToString(reader["PD_STATUS"]);
                        productOilprice.PdDesc = Convert.ToString(reader["PD_DESC"]);
                        productOilprice.UnitId = Convert.ToString(reader["UNIT_ID"]);
                        productOilprice.UnitName = Convert.ToString(reader["UNIT_NAME"]);
                        productOilprice.GroupId = Convert.ToString(reader["GROUP_ID"]);
                        productOilprice.VatType = Convert.ToString(reader["VAT_TYPE"]);
                        productOilprice.VatRate = Convert.ToInt32(reader["VAT_RATE"]);
                        productOilprice.UnitBarcode = Convert.ToString(reader["UNIT_BARCODE"]);
                        productOilprice.UnitPrice = Convert.ToDecimal(reader["UnitPrice"]);

                        currentOilPrices.Add(productOilprice);
                    }
                }

                resp.AddRange(currentOilPrices);
                //resp.OrderBy(p => p.PdId);
            }
            return resp.OrderBy(x => x.PdId).ToList();
        }
        public List<Products> GetProductReasonList(ProductReasonRequest req)
        {
            var sql = (from pd in context.MasProducts
                       join pu in context.MasProductUnits on pd.PdId equals pu.PdId
                       join rg in context.MasReasonGroups on pd.GroupId equals rg.GroupId
                       join pp in context.MasProductPrices on new { pd.PdId, req.CompCode, req.BrnCode, req.LocCode } equals new { pp.PdId, pp.CompCode, pp.BrnCode, pp.LocCode }
                       where (pd.PdName.Contains(req.Keyword) || pd.PdId.Contains(req.Keyword) || pd.UnitName.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                            && rg.ReasonGroup == req.ReasonGroup && rg.ReasonId == req.ReasonId
                            && pd.GroupId != "0000"

                       select new { pd, pu, rg, pp }).AsQueryable();

            List<Products> resp = sql
              .GroupBy(x => new
              {
                  x.pd.PdId,
                  x.pd.PdName,
                  x.pd.PdStatus,
                  x.pd.PdDesc,
                  x.pu.UnitId,
                  x.pu.UnitName,
                  x.pd.GroupId,
                  x.pd.VatType,
                  x.pd.VatRate,
                  x.pp.UnitBarcode,
                  x.pp.Unitprice
              })
              .Select(x => new Products
              {
                  PdId = x.Key.PdId,
                  PdName = x.Key.PdName,
                  PdStatus = x.Key.PdStatus,
                  PdDesc = x.Key.PdDesc,
                  UnitId = x.Key.UnitId,
                  UnitName = x.Key.UnitName,
                  GroupId = x.Key.GroupId,
                  VatType = x.Key.VatType,
                  VatRate = x.Key.VatRate,
                  UnitBarcode = x.Key.UnitBarcode,
                  UnitPrice = decimal.Parse(x.Key.Unitprice.ToString())
              }).ToList();

            var currentOilPrices = new List<Products>();

            if (req.SystemDate != DateTime.MinValue)
            {
                var sysDateMidNight = new DateTime(req.SystemDate.Year, req.SystemDate.Month, req.SystemDate.Day, 0, 0, 0).AddHours(23).AddMinutes(59).AddSeconds(59);
                string sysDateTime = sysDateMidNight.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                using (var cnn = context.Database.GetDbConnection())
                {
                    var cmm = cnn.CreateCommand();
                    cmm.CommandText = $@"select p.PD_ID, p.PD_NAME ,p.PD_STATUS,p.PD_DESC,p.UNIT_ID,pu.UNIT_NAME,p.GROUP_ID,p.VAT_TYPE,p.VAT_RATE,pu.UNIT_BARCODE,oil.current_price as UnitPrice
                                    --,oil.*
                                    from 
                                    (
                                      select xpri.comp_code, xpri.brn_code, xpri.pd_id
                                        , case when xpro.pro_adj_price is null
                                         then (xpri.before_price + xpri.adjust_price) 
                                         else (xpri.before_price + xpro.pro_adj_price) + xpri.adjust_price end current_price
                                      from 
                                       (
                                       select a.doc_no, a.comp_code, a.brn_code, b.pd_id, b.before_price, b.adjust_price 
                                       from dbo.oil_standard_price_hd a, dbo.oil_standard_price_dt b
                                       where a.brn_code = '{ req.BrnCode }'
                                        and a.comp_code = '{ req.CompCode }' 
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
                                       where a.brn_code = '{ req.BrnCode }'  -- parameter: branch
                                        and a.comp_code = '{ req.CompCode }'   -- parameter: compcode
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
                                      and xpri.comp_code = xpro.comp_code
                                    ) oil
                                    inner join MAS_PRODUCT p  on oil.PD_ID = p.PD_ID
                                    inner join MAS_PRODUCT_UNIT pu on p.PD_ID  = pu.PD_ID and p.UNIT_ID = pu.UNIT_ID
                                    inner join MAS_REASON_GROUP rg on p.GROUP_ID = rg.GROUP_ID and rg.REASON_GROUP = 'Withdraw' 
                                    where rg.REASON_ID = '{ req.ReasonId }' ; ";
                    cmm.Connection = cnn;
                    cnn.Open();
                    var reader = cmm.ExecuteReader();

                    while (reader.Read())
                    {
                        var productOilprice = new Products();
                        productOilprice.PdId = Convert.ToString(reader["PD_ID"]);
                        productOilprice.PdName = Convert.ToString(reader["PD_NAME"]);
                        productOilprice.PdStatus = Convert.ToString(reader["PD_STATUS"]);
                        productOilprice.PdDesc = Convert.ToString(reader["PD_DESC"]);
                        productOilprice.UnitId = Convert.ToString(reader["UNIT_ID"]);
                        productOilprice.UnitName = Convert.ToString(reader["UNIT_NAME"]);
                        productOilprice.GroupId = Convert.ToString(reader["GROUP_ID"]);
                        productOilprice.VatType = Convert.ToString(reader["VAT_TYPE"]);
                        productOilprice.VatRate = Convert.ToInt32(reader["VAT_RATE"]);
                        productOilprice.UnitBarcode = Convert.ToString(reader["UNIT_BARCODE"]);
                        productOilprice.UnitPrice = Convert.ToDecimal(reader["UnitPrice"]);

                        currentOilPrices.Add(productOilprice);
                    }
                }

                resp.AddRange(currentOilPrices);
                //resp.OrderBy(p => p.PdId);
            }

            return resp.OrderBy(x => x.PdId).ToList();
        }
        public List<Products> GetProductServiceList(ProductRequest req)
        {
            var sql = (from pd in this.context.MasProducts
                       join pt in this.context.MasProductTypes on pd.GroupId equals pt.GroupId
                       join dt in this.context.MasDocumentTypes on pt.DocTypeId equals dt.DocTypeId
                       where (pd.PdName.Contains(req.Keyword) || pd.PdId.Contains(req.Keyword) || pd.UnitName.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                            && (dt.DocTypeId == req.DocumentTypeID || req.DocumentTypeID == null || req.DocumentTypeID == "")
                            && dt.DocTypeDesc == "ReceivePay"
                            && pd.PdStatus != "Cancel"
                       select new { pd, pt, dt }).AsQueryable();

            List<Products> resp = sql
              .Select(x => new Products
              {
                  PdId = x.pd.PdId,
                  PdName = x.pd.PdName,
                  PdStatus = x.pd.PdStatus,
                  PdDesc = x.pd.PdDesc,
                  UnitId = x.pd.UnitId,
                  UnitName = x.pd.UnitName,
                  GroupId = x.pd.GroupId,
                  VatType = x.pd.VatType,
                  VatRate = x.pd.VatRate,
                  AcctCode = x.pd.AcctCode
              }).ToList();
            return resp;
        }

        public async Task<QueryResult<MasProduct>> FindAllAsync(ProductQuery query)
        {
            var queryable = context.MasProducts
                .OrderBy(x => x.PdId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.PdName.Contains(query.Keyword)
                || p.PdId.Contains(query.Keyword)
                || p.GroupId.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<MasProduct>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<MasProduct> FindByIdAsync(ProductResource query)
        {
            return await context.MasProducts.FirstOrDefaultAsync(x => x.PdId == query.PdId);
        }

        public async Task<MasProduct> FindProductOilTypeAsync(string pdid)
        {
            return await context.MasProducts.FirstOrDefaultAsync(x => x.PdId == pdid && x.GroupId == "0000");
        }

        public async Task<List<MasProduct>> GetProductOilTypeAsync()
        {
            var queryable = context.MasProducts
                .Where(x => x.GroupId == "0000")
               .OrderBy(x => x.PdId)
               .AsNoTracking();
            var resp = await queryable.ToListAsync();
            return resp;
        }
    }
}
