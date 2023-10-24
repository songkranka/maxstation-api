using MaxStation.Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Repositories;
using Sale.API.Helpers;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Repositories
{
    public class CreditSaleRepository : SqlDataAccessHelper, ICreditSaleRepository
    {
        public CreditSaleRepository(PTMaxstationContext context) : base(context)
        {

        }

        public int GetRunNumber(SalCreditsaleHd creditSaleHd)
        {
            int runNumber = 1;
            SalCreditsaleHd resp = new SalCreditsaleHd();
            resp = this.context.SalCreditsaleHds.OrderByDescending(y => y.RunNumber).FirstOrDefault(
                x => (x.DocPattern == creditSaleHd.DocPattern || creditSaleHd.DocPattern == "" || creditSaleHd.DocPattern == null)
                && x.DocType == "CreditSale"
            );

            if (resp != null)
            {
                runNumber = (int)resp.RunNumber + 1;
            }
            else
            {
                runNumber = 1;
            }
            return runNumber;
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

        public async Task<QueryResult<SalCreditsaleHd>> ListAsync(RequestData query)
        {
            if (query == null)
            {
                return null;
            }
            string strIsoLevel = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            string strSelect = @"select doc_no , doc_date , Cust_Code , Cust_Name , net_Amt , doc_status , guid from SAL_CREDITSALE_HD(nolock)";
            string strCount = @"select COUNT(*) from SAL_CREDITSALE_HD(nolock)";
            string strWhere = @" where DOC_TYPE = 'CreditSale' ";
            string strOrderBy = @" order by COMP_CODE, BRN_CODE, DOC_NO desc ";
            string strComCode = DefaultService.EncodeSqlString(query.CompCode);
            if (!0.Equals(strComCode.Length))
            {
                strWhere += $" and COMP_CODE = '{strComCode}'";
            }
            string strBrnCode = DefaultService.EncodeSqlString(query.BrnCode);
            if (!0.Equals(strBrnCode.Length))
            {
                strWhere += $" and BRN_CODE = '{strBrnCode}'";
            }
            if (query.FromDate.HasValue && query.ToDate.HasValue)
            {
                strWhere += $" and DOC_DATE between '{query.FromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}' and '{query.ToDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}'";
            }
            string strKeyWord = DefaultService.EncodeSqlString(query.Keyword);
            if (!0.Equals(strKeyWord.Length))
            {
                strWhere += $" and ( DOC_NO like '%{strKeyWord}%' or DOC_STATUS like '%{strKeyWord}%' )";
            }
            string strCon = context.Database.GetConnectionString();
            string strPage = string.Empty;
            if (query.Skip > 0 && query.Take > 0)
            {
                strPage = $" OFFSET {(query.Skip - 1) * query.Take} row fetch next {query.Take} row only";
            }
            int intTotal = await DefaultService.ExecuteScalar<int>(strCon, strIsoLevel + strCount + strWhere);
            var listCreditSale = await DefaultService.GetEntityFromSql<List<SalCreditsaleHd>>(
                context , strIsoLevel + strSelect + strWhere + strOrderBy + strPage
            );
            return new QueryResult<SalCreditsaleHd>
            {
                Items = listCreditSale ?? new List<SalCreditsaleHd>(),
                TotalItems = intTotal,
                ItemsPerPage = query.Take
            };
        }
        public async Task<QueryResult<SalCreditsaleHd>> ListAsyncOld(RequestData query)
        {
            //var queryable = context.SalCreditsaleHds
            //    .Where(
            //        x => (x.CompCode == req.CompCode || req.CompCode == null || req.CompCode == "") 
            //        && (x.BrnCode == req.BrnCode || req.BrnCode == null || req.BrnCode == "")
            //        && (x.LocCode == req.LocCode || req.LocCode == null || req.LocCode == "")
            //        && x.DocType == "CreditSale"
            //        && ((x.DocDate >= req.FromDate && x.DocDate <= req.ToDate) || req.FromDate == null || req.ToDate == null)
            //    )
            //    .OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
            //    //.OrderByDescending(y => y.DocDate).ThenByDescending(z => z.DocNo)
            //    .AsNoTracking();

            //if (!string.IsNullOrEmpty(req.Keyword))
            //{
            //    queryable = queryable.Where(p => p.DocNo.Contains(req.Keyword)
            //        || p.DocStatus.Contains(req.Keyword));
            //}

            //int totalItems = await queryable.CountAsync();
            ////List<SalCreditsaleHd> resp = await queryable.Skip((req.Skip - 1) * req.Take)
            ////                               .Take(req.Take)
            ////                               .ToListAsync();

            //if(req.Skip >= 1 && req.Take > 0)
            //{
            //    queryable = queryable.Skip((req.Skip - 1) * req.Take).Take(req.Take);
            //}
            //List<SalCreditsaleHd> resp = await queryable.ToListAsync();
            //return new QueryResult<SalCreditsaleHd>
            //{
            //    Items = resp,
            //    TotalItems = totalItems,
            //};

            var resp = new List<SalCreditsaleHd>();
            int totalItems = 0;

            if (query.FromDate != null && query.FromDate != DateTime.MinValue && query.ToDate != null && query.ToDate != DateTime.MinValue)
            {
                var queryable = context.SalCreditsaleHds
                    .Where(
                        x => (x.CompCode == query.CompCode || query.CompCode == null || query.CompCode == "")
                        && (x.BrnCode == query.BrnCode || query.BrnCode == null || query.BrnCode == "")
                        && (x.LocCode == query.LocCode || query.LocCode == null || query.LocCode == "")
                        && x.DocType == "CreditSale"
                        && ((x.DocDate >= query.FromDate && x.DocDate <= query.ToDate) || query.FromDate == null || query.ToDate == null)
                    )
                    .OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
                    .AsNoTracking();

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                        || p.DocStatus.Contains(query.Keyword));
                }

                resp = await queryable.Skip((query.Skip - 1) * query.Take)
                                           .Take(query.Take)
                                           .ToListAsync();

                totalItems = await queryable.CountAsync();
            }
            else
            {
                var queryable = context.SalCreditsaleHds
                    .Where(
                        x => (x.CompCode == query.CompCode || query.CompCode == null || query.CompCode == "")
                        && (x.BrnCode == query.BrnCode || query.BrnCode == null || query.BrnCode == "")
                        && (x.LocCode == query.LocCode || query.LocCode == null || query.LocCode == "")
                        && x.DocType == "CreditSale"
                        && (x.DocDate >= query.SysDate.AddDays(-30))
                    )
                    .OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
                    .AsNoTracking();

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                        || p.DocStatus.Contains(query.Keyword));
                }

                resp = await queryable.Skip((query.Skip - 1) * query.Take)
                                           .Take(query.Take)
                                           .ToListAsync();

                totalItems = await queryable.CountAsync();
            }

            return new QueryResult<SalCreditsaleHd>
            {
                Items = resp,
                TotalItems = totalItems,
            };
        }

        public async Task<SalCreditsaleHd> FindByIdAsync(Guid guid)
        {
            var response = new SalCreditsaleHd();
            var header = await context.SalCreditsaleHds.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == guid && x.DocType == "CreditSale");

            if (header != null)
            {
                var detail = context.SalCreditsaleDts.AsNoTracking().Where(x => x.CompCode == header.CompCode && x.BrnCode == header.BrnCode && x.DocNo == header.DocNo).OrderBy(y => y.SeqNo).ToList();
                response.CompCode = header.CompCode;
                response.BrnCode = header.BrnCode;
                response.LocCode = header.LocCode;
                response.DocNo = header.DocNo;
                response.DocType = header.DocType;
                response.DocStatus = header.DocStatus;
                response.DocDate = header.DocDate;
                response.Period = header.Period;
                response.RefNo = header.RefNo;
                response.QtNo = header.QtNo;
                response.CustCode = header.CustCode;
                response.CustName = header.CustName;
                response.CustAddr1 = header.CustAddr1;
                response.CustAddr2 = header.CustAddr2;
                response.ItemCount = header.ItemCount;
                response.Remark = header.Remark;
                response.Currency = header.Currency;
                response.CurRate = header.CurRate;
                response.SubAmt = header.SubAmt;
                response.SubAmtCur = header.SubAmtCur;
                response.DiscRate = header.DiscRate;
                response.DiscAmt = header.DiscAmt;
                response.DiscAmtCur = header.DiscAmtCur;
                response.NetAmt = header.NetAmt;
                response.NetAmtCur = header.NetAmtCur;
                response.VatRate = header.VatRate;
                response.VatAmt = header.VatAmt;
                response.VatAmtCur = header.VatAmtCur;
                response.TotalAmt = header.TotalAmt;
                response.TotalAmtCur = header.TotalAmtCur;
                response.TaxBaseAmt = header.TaxBaseAmt;
                response.TaxBaseAmtCur = header.TaxBaseAmtCur;
                response.TxNo = header.TxNo;
                response.Post = header.Post;
                response.RunNumber = header.RunNumber;
                response.Guid = header.Guid;
                response.CreatedDate = header.CreatedDate;
                response.CreatedBy = header.CreatedBy;
                response.UpdatedDate = header.UpdatedDate;
                response.UpdatedBy = header.UpdatedBy;
                response.DocPattern = header.DocPattern;
                response.CitizenId = header.CitizenId;
                response.SalCreditsaleDt = detail;
                response.EmpCode = header.EmpCode;
                response.EmpName = header.EmpName;
            }
            return response;
        }

        public async Task AddHdAsync(SalCreditsaleHd creditSaleHd)
        {
            await context.SalCreditsaleHds.AddAsync(creditSaleHd);
        }

        public async Task AddDtAsync(SalCreditsaleDt creditSaleDt)
        {
            await context.SalCreditsaleDts.AddAsync(creditSaleDt);
        }

        public async Task<int> CheckDataDuplicate(SalCreditsaleHd obj)
        {
            return await context.SalCreditsaleHds.AsNoTracking().Where(
                x => x.CompCode == obj.CompCode
                && x.BrnCode == obj.BrnCode
                && x.LocCode == obj.LocCode
                && x.PosNo == obj.PosNo).CountAsync();
        }

        //public void UpdateAsync(SalCreditsaleHd creditSaleHd)
        //{
        //    context.SalCreditsaleHds.Update(creditSaleHd);
        //}

        public void UpdateAsync(SalCreditsaleHd creditSaleHd)
        {
            EntityEntry<SalCreditsaleHd> entCreditSale = null;
            entCreditSale = context.SalCreditsaleHds.Update(creditSaleHd);
            string[] arrNotUpdate = { "DocDate", "RunNumber", "DocPattern", "Guid", "CreatedDate", "CreatedBy"};
            foreach (var item in arrNotUpdate)
            {
                entCreditSale.Property(item).IsModified = false;
            }
        }

        public void RemoveDtAsync(IEnumerable<SalCreditsaleDt> creditSaleDt)
        {
            context.SalCreditsaleDts.RemoveRange(creditSaleDt);
        }

        public void AddDtListAsync(IEnumerable<SalCreditsaleDt> creditSaleDt)
        {
            context.SalCreditsaleDts.AddRange(creditSaleDt);
        }

        public async Task UpdateRemainQuotation(SalCreditsaleHd obj)
        {
            var hd = context.SalQuotationHds.FirstOrDefault(x => x.CompCode == obj.CompCode && x.BrnCode == obj.BrnCode && x.LocCode == obj.LocCode && x.DocNo == obj.RefNo);
            if (hd != null)
            {
                var dt = context.SalQuotationDts.Where(x => x.CompCode == obj.CompCode && x.BrnCode == obj.BrnCode && x.LocCode == obj.LocCode && x.DocNo == obj.RefNo).ToList();
                if (dt != null)
                {
                    foreach (SalQuotationDt row in dt) {
                        var find = obj.SalCreditsaleDt.FirstOrDefault(y => y.UnitBarcode == row.UnitBarcode); //ต้องไป 1 แถวเสมอ เพราะในเอกสารเดียวกัน จะมี Barcode สินค้าเดียวกันหลายบรรทัดไม่ได้
                        if (obj.DocStatus == "Active")
                        {
                            //row.StockRemain -= (find != null ? find.StockQty : 0);
                            row.StockRemain -= find?.StockQty ?? decimal.Zero;
                        }
                    }

                    hd.DocStatus = "Reference";
                    context.SalQuotationHds.Update(hd);
                    context.SalQuotationDts.UpdateRange(dt);
                }
            }
        }

        public async Task ReturnRemainQuotation(SalCreditsaleHd obj)
        {
            var hd = context.SalQuotationHds.FirstOrDefault(x => x.CompCode == obj.CompCode && x.BrnCode == obj.BrnCode && x.LocCode == obj.LocCode && x.DocNo == obj.RefNo);
            if (hd != null)
            {
                var dt = context.SalQuotationDts.Where(x => x.CompCode == obj.CompCode && x.BrnCode == obj.BrnCode && x.LocCode == obj.LocCode && x.DocNo == obj.RefNo).ToList();
                if (dt != null)
                {
                    foreach (SalQuotationDt row in dt)
                    {
                        var find = obj.SalCreditsaleDt.FirstOrDefault(y => y.UnitBarcode == row.UnitBarcode); //ต้องไป 1 แถวเสมอ เพราะในเอกสารเดียวกัน จะมี Barcode สินค้าเดียวกันหลายบรรทัดไม่ได้
                                                                                                              //row.StockRemain += (find != null ? find.StockQty : 0);
                        row.StockRemain += find?.StockQty ?? decimal.Zero;
                    }

                    //Change Status เมื่อ StockRemain เท่ากับ Stock ทุกรายการ
                    var chk = dt.Where(x => x.StockQty != x.StockRemain).ToList();
                    if (chk.Count() > 0)
                    {
                        hd.DocStatus = "Reference";
                    }
                    else
                    {
                        hd.DocStatus = "Ready";
                    }

                    context.SalQuotationHds.Update(hd);
                    context.SalQuotationDts.UpdateRange(dt);
                }
            }
        }

        public async Task<List<SalQuotationDt>> CheckRemainQuotation(SalCreditsaleHd obj)
        {
            List<SalQuotationDt> resp = new List<SalQuotationDt>();
            var hd = context.SalQuotationHds.FirstOrDefault(x => x.CompCode == obj.CompCode && x.BrnCode == obj.BrnCode && x.LocCode == obj.LocCode && x.DocNo == obj.RefNo);
            if (hd != null)
            {
                var dt = context.SalQuotationDts.Where(x => x.CompCode == obj.CompCode && x.BrnCode == obj.BrnCode && x.LocCode == obj.LocCode && x.DocNo == obj.RefNo).ToList();
                if (dt != null)
                {
                    foreach (SalQuotationDt row in dt)
                    {
                        if (row.StockRemain < 0) {
                            var stockQty = obj.SalCreditsaleDt.FirstOrDefault(x => x.PdId == row.PdId && x.UnitBarcode == row.UnitBarcode).StockQty;
                            if (stockQty != null && stockQty > 0)
                            {
                                row.StockRemain += stockQty;
                            }
                            else {
                                row.StockRemain += 0;
                            }
                            resp.Add(row);
                        }
                    }
                }
            }
            return resp.ToList();
        }

        public async Task<List<MasCustomerCar>> GetCustomerCar(string pStrCusCode)
        {
            pStrCusCode = pStrCusCode?.Trim() ?? string.Empty;
            if (0.Equals(pStrCusCode.Length)){
                return null;
            }
            bool checkCompany = await context.MasCompanies.AnyAsync(x => pStrCusCode.Equals(x.CustCode));
            if (!checkCompany)
            {
                return null;
            }
            List<MasCustomerCar> result = await context.MasCustomerCars.Where(
                x => pStrCusCode.Equals( x.CustCode)
                && "Active".Equals(x.CarStatus)
            ).ToListAsync();
            return result;
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
                            //&& pt.DocTypeId != "003"
                            //&& pd.GroupId != "8400"
                            //&& pd.GroupId != "0000"
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
    }
}
