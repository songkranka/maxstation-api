using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Json;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Repositories;
using Sale.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sale.API.Services
{
    public class CreditSaleService : ICreditSaleService
    {
        private readonly ICreditSaleRepository _creditSaleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private PTMaxstationContext _context;
        readonly string urlMasterAPI = @"https://maxstation-masterdata-api.pt.co.th";
        //readonly string urlMasterAPI = @"https://localhost:44309";
        public CreditSaleService(
            ICreditSaleRepository creditSaleRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context
        )
        {
            _creditSaleRepository = creditSaleRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<List<SalCreditsaleDt>> CalculateStock(SalCreditsaleHd obj)
        {
            List<SalCreditsaleDt> resp = new List<SalCreditsaleDt>();
            try
            {
                //สร้างตัวแปรสำหรับเรียกข้อมูล Service : MasterData
                string pdBarcodeStr = "";
                for (int i = 0; i < obj.SalCreditsaleDt.Count; i++)
                {
                    pdBarcodeStr += obj.SalCreditsaleDt[i].UnitBarcode;
                    if (i == obj.SalCreditsaleDt.Count - 1)
                    {
                        pdBarcodeStr += "";
                    }
                    else
                    {
                        pdBarcodeStr += ",";
                    }
                }
                RequestData req = new RequestData
                {
                    CompCode = obj.CompCode,
                    BrnCode = obj.BrnCode,
                    LocCode = obj.LocCode,
                    DocDate = DateTime.Now,
                    PDBarcodeList = pdBarcodeStr
                };

                //เรียกข้อมูลจาก Service : MasterData
                List<MasProductUnit> pdUnit = new List<MasProductUnit>();
                ResponseData<List<MasProductUnit>> response = new ResponseData<List<MasProductUnit>>();
                string url = this.urlMasterAPI + @"/api/ProductUnit/GetProductUnitList";
                var httpWebRequest = WebRequest.CreateHttp(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(req);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string responseText = streamReader.ReadToEnd();
                    response = new JavaScriptSerializer().Deserialize<ResponseData<List<MasProductUnit>>>(responseText);
                    pdUnit = response.Data;
                }

                //คำนวณ Stock
                foreach (SalCreditsaleDt row in obj.SalCreditsaleDt)
                {
                    var dt = pdUnit.Find(x => x.UnitBarcode == row.UnitBarcode);
                    if (dt != null)
                    {
                        row.StockQty = (row?.ItemQty??decimal.Zero) * dt.UnitStock;
                        //row.StockRemain = row.StockQty;
                    }
                }
                resp = obj.SalCreditsaleDt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resp;
        }


        public async Task<QueryResult<SalCreditsaleHd>> ListAsync(RequestData req)
        {
            return await _creditSaleRepository.ListAsync(req);
        }

        public async Task<SalCreditsaleHd> FindByIdAsync(RequestData req)
        {
            return await _creditSaleRepository.FindByIdAsync(Guid.Parse(req.Guid));
        }

        public async Task<ResponseService<SalCreditsaleHd>> SaveAsync(SalCreditsaleHd obj, IServiceScopeFactory serviceScopeFactory)
        {
            ResponseService<SalCreditsaleHd> resp = new ResponseService<SalCreditsaleHd>();
            obj.Guid = Guid.NewGuid();
            try
            {
                var header = new SalCreditsaleHd
                {
                    CompCode = obj.CompCode,
                    BrnCode = obj.BrnCode,
                    LocCode = obj.LocCode,
                    DocNo = obj.DocNo,
                    DocType = "CreditSale",
                    DocStatus = "Active",
                    DocDate = obj.DocDate,
                    Period = obj.Period,
                    RefNo = obj.RefNo,
                    QtNo = obj.QtNo,
                    CustCode = obj.CustCode,
                    CustName = obj.CustName,
                    CustAddr1 = obj.CustAddr1,
                    CustAddr2 = obj.CustAddr2,
                    ItemCount = obj.ItemCount,
                    Remark = obj.Remark,
                    Currency = obj.Currency,
                    CurRate = obj.CurRate,
                    SubAmt = obj.SubAmt,
                    SubAmtCur = obj.SubAmtCur,
                    DiscRate = obj.DiscRate,
                    DiscAmt = obj.DiscAmt??0,
                    DiscAmtCur = obj.DiscAmtCur??0,
                    NetAmt = obj.NetAmt,
                    NetAmtCur = obj.NetAmtCur,
                    VatRate = obj.VatRate,
                    VatAmt = obj.VatAmt,
                    VatAmtCur = obj.VatAmtCur,
                    TaxBaseAmt = obj.TaxBaseAmt,
                    TaxBaseAmtCur = obj.TaxBaseAmtCur,
                    TotalAmt = obj.TotalAmt,
                    TotalAmtCur = obj.TotalAmtCur,
                    TxNo = obj.TxNo,
                    Post = obj.Post,
                    RunNumber = obj.RunNumber,
                    DocPattern = obj.DocPattern,
                    Guid = obj.Guid,
                    CreatedDate = DateTime.Now,
                    CreatedBy = obj.CreatedBy,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = obj.CreatedBy,
                    CitizenId = obj.CitizenId,
                    EmpCode = obj.EmpCode,
                    EmpName = obj.EmpName
                };

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    int runNumber = _creditSaleRepository.GetRunNumber(header);
                    header.RunNumber = runNumber;
                    var docNo = header.DocNo.Replace("#", "0").Substring(0, header.DocNo.Length - runNumber.ToString().Length) + header.RunNumber.ToString();
                    header.DocNo = docNo;

                    await this.CalculateStock(obj);
                    await _creditSaleRepository.UpdateRemainQuotation(obj);
                    await _creditSaleRepository.AddHdAsync(header);

                    int index = 0;
                    foreach (var objDt in obj.SalCreditsaleDt)
                    {
                        index++;
                        var detail = new SalCreditsaleDt
                        {
                            CompCode = obj.CompCode,
                            BrnCode = obj.BrnCode,
                            LocCode = obj.LocCode,
                            DocType = obj.DocType,
                            DocNo = header.DocNo,
                            SeqNo = index,
                            PoNo = objDt.PoNo,
                            LicensePlate = objDt.LicensePlate,
                            Mile = objDt.Mile,
                            PdId = objDt.PdId,
                            PdName = objDt.PdName,
                            IsFree = objDt.IsFree,
                            UnitId = objDt.UnitId,
                            UnitBarcode = objDt.UnitBarcode,
                            UnitName = objDt.UnitName,
                            MeterStart = objDt.MeterStart??0,
                            MeterFinish = objDt.MeterFinish??0,
                            ItemQty = objDt.ItemQty??0,
                            StockQty = _creditSaleRepository.CalculateStockQty(objDt.PdId, objDt.ItemQty.Value),
                            UnitPrice = objDt.UnitPrice,
                            UnitPriceCur = objDt.UnitPriceCur,
                            RefPrice = objDt.RefPrice,
                            RefPriceCur = objDt.RefPriceCur,
                            DiscAmt = objDt.DiscAmt,
                            DiscAmtCur = objDt.DiscAmtCur,
                            DiscHdAmt = objDt.DiscHdAmt,
                            DiscHdAmtCur = objDt.DiscHdAmtCur,
                            SumItemAmt = objDt.SumItemAmt,
                            SumItemAmtCur = objDt.SumItemAmtCur,
                            SubAmt = objDt.SubAmt,
                            SubAmtCur = objDt.SubAmtCur,
                            VatType = objDt.VatType,
                            VatRate = objDt.VatRate,
                            VatAmt = objDt.VatAmt,
                            VatAmtCur = objDt.VatAmtCur,
                            TaxBaseAmt = objDt.TaxBaseAmt,
                            TaxBaseAmtCur = objDt.TaxBaseAmtCur,
                            TotalAmt = objDt.TotalAmt,
                            TotalAmtCur = objDt.TotalAmtCur,
                        };

                        await _creditSaleRepository.AddDtAsync(detail);
                    }
                }

                List<SalQuotationDt> checkRemain = await _creditSaleRepository.CheckRemainQuotation(obj);
                var checkRemainStr = "ไม่อนุญาตให้กรอกจำนวนสินค้าเกินจำนวนคงเหลือ";
                if (checkRemain.Count > 0)
                {
                    foreach (SalQuotationDt dt in checkRemain)
                    {
                        checkRemainStr += "<br>รหัส " + dt.UnitBarcode + " : " + dt.PdName + " คงเหลือ " + dt.StockRemain + " " + dt.UnitName;
                    }
                    resp.IsSuccess = false;
                    resp.Message = checkRemainStr;
                    return resp;
                }

                await _unitOfWork.CompleteAsync();
                resp.IsSuccess = true;
                resp.Message = "บันทึกข้อมูลสำเร็จ";
                resp.Data = obj;
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }

        public async Task<ResponseService<SalCreditsaleHd>> UpdateAsync(SalCreditsaleHd obj, IServiceScopeFactory serviceScopeFactory)
        {
            ResponseService<SalCreditsaleHd> resp = new ResponseService<SalCreditsaleHd>();
            try
            {
                
                resp.IsSuccess = false;
                SalCreditsaleHd header = await _creditSaleRepository.FindByIdAsync(new Guid(obj.Guid.ToString()));

                if (header == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "CreditSaleHD Not Found Data!!!.";
                    return resp;
                }

                List<SalCreditsaleDt> detail = header.SalCreditsaleDt
                    .Where(x => x.CompCode == header.CompCode && x.BrnCode == header.BrnCode
                    && x.LocCode == header.LocCode && x.DocNo == header.DocNo).ToList();

                if (detail == null)
                {
                    resp.IsSuccess = false;
                    resp.Message = "CreditSaleDT Not Found Data!!!.";
                    return resp;
                }

                int seq = 1;
                foreach (SalCreditsaleDt row in obj.SalCreditsaleDt) {
                    row.SeqNo = seq;
                    seq++;
                }

                if (obj.DocStatus == "Active")
                {   //เฉพาะ Active
                    var dtBackup = obj.SalCreditsaleDt.ToList();

                    //คืนค่าเดิมก่อน
                    obj.SalCreditsaleDt = detail;
                    await _creditSaleRepository.ReturnRemainQuotation(obj);

                    //Update ค่าใหม่
                    obj.SalCreditsaleDt = dtBackup;
                    await this.CalculateStock(obj);
                    await _creditSaleRepository.UpdateRemainQuotation(obj);

                    header.CompCode = obj.CompCode;
                    header.BrnCode = obj.BrnCode;
                    header.LocCode = obj.LocCode;
                    header.DocNo = obj.DocNo;
                    header.DocType = "CreditSale";
                    header.DocStatus = obj.DocStatus;
                    header.DocDate = obj.DocDate;
                    header.Period = obj.Period;
                    header.RefNo = obj.RefNo;
                    header.QtNo = obj.QtNo;
                    header.CustCode = obj.CustCode;
                    header.CustName = obj.CustName;
                    header.CustAddr1 = obj.CustAddr1;
                    header.CustAddr2 = obj.CustAddr2;
                    header.ItemCount = obj.ItemCount;
                    header.Remark = obj.Remark;
                    header.Currency = obj.Currency;
                    header.CurRate = obj.CurRate;
                    header.SubAmt = obj.SubAmt;
                    header.SubAmtCur = obj.SubAmtCur;
                    header.DiscRate = obj.DiscRate;
                    header.DiscAmt = obj.DiscAmt;
                    header.DiscAmtCur = obj.DiscAmtCur;
                    header.NetAmt = obj.NetAmt;
                    header.NetAmtCur = obj.NetAmtCur;
                    header.VatRate = obj.VatRate;
                    header.VatAmt = obj.VatAmt;
                    header.VatAmtCur = obj.VatAmtCur;
                    header.TaxBaseAmt = obj.TaxBaseAmt;
                    header.TaxBaseAmtCur = obj.TaxBaseAmtCur;
                    header.TotalAmt = obj.TotalAmt;
                    header.TotalAmtCur = obj.TotalAmtCur;
                    header.TxNo = obj.TxNo;
                    header.Post = obj.Post;
                    header.RunNumber = obj.RunNumber;
                    header.DocPattern = obj.DocPattern;
                    header.Guid = obj.Guid;
                    //header.CreatedDate = obj.CreatedDate;
                    //header.CreatedBy = obj.CreatedBy;
                    header.UpdatedDate = DateTime.Now;
                    header.UpdatedBy = obj.UpdatedBy;
                    header.CitizenId = obj.CitizenId;
                    header.EmpCode = obj.EmpCode;
                    header.EmpName = obj.EmpName;
                    header.SalCreditsaleDt = obj.SalCreditsaleDt.Select(x => new SalCreditsaleDt
                    {
                        CompCode = header.CompCode,
                        BrnCode = header.BrnCode,
                        LocCode = header.LocCode,
                        DocType = header.DocType,
                        DocNo = header.DocNo,
                        SeqNo = x.SeqNo,
                        PoNo = x.PoNo,
                        LicensePlate = x.LicensePlate,
                        Mile = x.Mile,
                        PdId = x.PdId,
                        PdName = x.PdName,
                        IsFree = x.IsFree,
                        UnitId = x.UnitId,
                        UnitBarcode = x.UnitBarcode,
                        UnitName = x.UnitName,
                        MeterStart = x.MeterStart??0,
                        MeterFinish = x.MeterFinish??0,
                        ItemQty = x.ItemQty??0,
                        StockQty = _creditSaleRepository.CalculateStockQty(x.PdId, x.ItemQty.Value),
                        UnitPrice = x.UnitPrice,
                        UnitPriceCur = x.UnitPriceCur,
                        RefPrice = x.RefPrice,
                        RefPriceCur = x.RefPriceCur,
                        DiscAmt = x.DiscAmt,
                        DiscAmtCur = x.DiscAmtCur,
                        DiscHdAmt = x.DiscHdAmt,
                        DiscHdAmtCur = x.DiscHdAmtCur,
                        SumItemAmt = x.SumItemAmt,
                        SumItemAmtCur = x.SumItemAmtCur,
                        SubAmt = x.SubAmt,
                        SubAmtCur = x.SubAmtCur,
                        VatType = x.VatType,
                        VatRate = x.VatRate,
                        VatAmt = x.VatAmt,
                        VatAmtCur = x.VatAmtCur,
                        TaxBaseAmt = x.TaxBaseAmt,
                        TaxBaseAmtCur = x.TaxBaseAmtCur,
                        TotalAmt = x.TotalAmt,
                        TotalAmtCur = x.TotalAmtCur,
                    }).ToList();
                }
                else if (obj.DocStatus == "Cancel")
                {
                    //Get SalCreditsaleDts ของเดิมมาเพื่อคืนค่า จะไม่ดึงจาก Obj ที่ส่งมา เนื่องจากอาจมีการเปลี่ยนแปลงค่าบนหน้าจอ
                    obj.SalCreditsaleDt = detail;
                    await _creditSaleRepository.ReturnRemainQuotation(obj);

                    header.DocStatus = obj.DocStatus;
                    header.UpdatedBy = obj.UpdatedBy;
                    header.UpdatedDate = DateTime.Now;
                }
                else {
                    header.DocStatus = obj.DocStatus;
                    header.UpdatedBy = obj.UpdatedBy;
                    header.UpdatedDate = DateTime.Now;
                }

                List<SalQuotationDt> checkRemain = await _creditSaleRepository.CheckRemainQuotation(obj);
                var checkRemainStr = "ไม่อนุญาตให้กรอกจำนวนสินค้าเกินจำนวนคงเหลือ";
                if (checkRemain.Count > 0)
                {
                    foreach (SalQuotationDt dt in checkRemain) {
                        checkRemainStr += "<br>รหัส " + dt.UnitBarcode + " : " + dt.PdName + " คงเหลือ " + dt.StockRemain + " " + dt.UnitName;
                    }
                    resp.IsSuccess = false;
                    resp.Message = checkRemainStr;
                    return resp;
                }

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    if (obj.DocStatus == "Active")
                    {
                        //เฉพาะ Active
                        _creditSaleRepository.UpdateAsync(header);
                        _creditSaleRepository.RemoveDtAsync(detail);
                        _creditSaleRepository.AddDtListAsync(header.SalCreditsaleDt);
                    }
                    else
                    {
                        //เฉพาะ Wait, Ready, Cancel
                        _creditSaleRepository.UpdateAsync(header);
                    }
                    await _unitOfWork.CompleteAsync();
                }
                resp.IsSuccess = true;
                resp.Data = header;
                return resp;
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
                return resp;
            }
        }

        public async Task<ResponseService<List<SalCreditsaleHd>>> SaveListAsync(List<SalCreditsaleHd> creditSaleList, IServiceScopeFactory serviceScopeFactory)
        {
            ResponseService<List<SalCreditsaleHd>> resp = new ResponseService<List<SalCreditsaleHd>>();
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    int runNumber = _creditSaleRepository.GetRunNumber(creditSaleList[0]); //เฉพาะ [0] เพราะทุกตัวต้องมี Pattern เดียวกันในแต่ละรอบ
                    foreach (SalCreditsaleHd hd in creditSaleList)
                    {
                        hd.RunNumber = runNumber;
                        var docNo = hd.DocNo.Replace("#", "0").Substring(0, hd.DocNo.Length - runNumber.ToString().Length) + hd.RunNumber.ToString();
                        hd.DocNo = docNo;
                        hd.CreatedDate = DateTime.Now;
                        hd.UpdatedDate = DateTime.Now;

                        //Reject ใบซ้ำ
                        var duplicateDoc = await _creditSaleRepository.CheckDataDuplicate(hd);
                        if (duplicateDoc == 0)
                        {
                            await _creditSaleRepository.AddHdAsync(hd);
                            int seq = 1;
                            foreach (SalCreditsaleDt dt in hd.SalCreditsaleDt)
                            {
                                dt.DocNo = docNo;
                                dt.SeqNo = seq;
                                seq++;
                                await _creditSaleRepository.AddDtAsync(dt);
                            }
                            runNumber++;
                        }
                        else
                        {
                            //Not Insert Duplicate Doc From POS
                        }
                    }
                    await _unitOfWork.CompleteAsync();
                }
                resp.IsSuccess = true;
                resp.Data = creditSaleList;
                return resp;
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = ex.Message;
                return resp;
            }
        }

        public async Task<List<MasCustomerCar>> GetCustomerCar(string pStrCusCode)
        {
            return await _creditSaleRepository.GetCustomerCar(pStrCusCode);
        }

        public async Task<MasCompanyCar[]> GetCompCar(string pStrCustcode)
        {
            pStrCustcode = (pStrCustcode ?? string.Empty).Trim();
            if(pStrCustcode.Length == 0)
            {
                return null;
            }
            var qryComp = _context.MasCompanies
                .Where(x => x.CustCode == pStrCustcode)
                .AsNoTracking();


            var qryCar = _context.MasCompanyCars
                .Where(x => qryComp.Any(y=> y.CompCode == x.CompCode))
                .AsNoTracking();
            var result = await qryCar.ToArrayAsync();
            return result;
        }

        public List<Products> GetProductListWithOutMaterialCode(ProductRequest req)
        {
            return  _creditSaleRepository.GetProductListWithOutMaterialCode(req);
        }
    }
}
