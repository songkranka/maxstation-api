using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Repositories;
using Sale.API.Domain.Services;
using Sale.API.Resources;
using Sale.API.Resources.Quotation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sale.API.Services
{
    public class QuotationService : IQuotationService
    {
        private const string _strActive = "Active";

        readonly IQuotationRepository quotationRepositories;
        readonly string urlMasterAPI = @"https://maxstation-masterdata-api.pt.co.th";
        //readonly string urlMasterAPI = @"https://localhost:44309";
        protected PTMaxstationContext context;
        public QuotationService(IQuotationRepository _quotationRepositories, PTMaxstationContext context)
        {
            this.quotationRepositories = _quotationRepositories;
            this.context = context;
        }

        public async Task<List<SalQuotationDt>> CalculateStock(SalQuotationHd obj)
        {
            List<SalQuotationDt> resp = new List<SalQuotationDt>();
            try
            {
                //สร้างตัวแปรสำหรับเรียกข้อมูล Service : MasterData
                string pdBarcodeStr = "";                
                pdBarcodeStr = string.Join(",", obj.SalQuotationDt.Select(x => x.UnitBarcode).ToList());

                //RequestData req = new RequestData();
                //req.CompCode = obj.CompCode;
                //req.BrnCode = obj.BrnCode;
                //req.LocCode = obj.LocCode;
                //req.DocDate = DateTime.Now;
                //req.PDBarcodeList = pdBarcodeStr;

                ////เรียกข้อมูลจาก Service : MasterData
                //List<MasProductUnit> pdUnit = new List<MasProductUnit>();
                //ResponseData<List<MasProductUnit>> response = new ResponseData<List<MasProductUnit>>();
                //string url = this.urlMasterAPI + @"/api/ProductUnit/GetProductUnitList";
                //var httpWebRequest = WebRequest.CreateHttp(url);
                //httpWebRequest.ContentType = "application/json; charset=utf-8";
                //httpWebRequest.Method = "POST";
                //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                //{
                //    string json = new JavaScriptSerializer().Serialize(req);
                //    streamWriter.Write(json);
                //    streamWriter.Flush();
                //}
                //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    string responseText = streamReader.ReadToEnd();
                //    response = new JavaScriptSerializer().Deserialize<ResponseData<List<MasProductUnit>>>(responseText);
                //    pdUnit = response.Data;
                //}

                var pdUnit = GetProductUnitList(pdBarcodeStr);


                //คำนวณ Stock
                foreach (SalQuotationDt row in obj.SalQuotationDt)
                {
                    if(row == null)
                    {
                        continue;
                    }
                    string strUnitBarcode = (row.UnitBarcode ?? string.Empty).Trim();
                    if (0.Equals(strUnitBarcode))
                    {
                        continue;
                    }
                    MasProductUnit pu = null;
                    pu =  pdUnit?.FirstOrDefault(x => strUnitBarcode.Equals( x.UnitBarcode));
                    row.StockQty = row.ItemQty;
                    if (pu != null)
                    {
                        if(pu.UnitStock != null && pu.UnitStock.HasValue)
                        {
                            row.StockQty *= pu.UnitStock.Value;
                        }
                        if(pu.UnitRatio != null && pu.UnitRatio.HasValue && !0.Equals(pu.UnitRatio.Value))
                        {
                            row.StockQty /= pu.UnitRatio.Value;
                        }
                    }
                    row.StockRemain = row.StockQty;
                }
                //foreach (SalQuotationDt row in obj.SalQuotationDt)
                //{
                //    var dt = pdUnit.Find(x => x.UnitBarcode == row.UnitBarcode);
                //    if (dt != null)
                //    {
                //        row.StockQty = row.ItemQty * dt.UnitStock;
                //        row.StockRemain = row.StockQty;
                //    }
                //}
                resp = obj.SalQuotationDt.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resp;
        }

        private List<MasProductUnit> GetProductUnitList(string barcodeList)
        {
            //ตรวจสอบกรณีมีการส่ง PDBarcodeList มาใช้ IN
            List<string> pdBarcodeList = new List<string>();
            if (barcodeList != null && barcodeList != "")
            {
                pdBarcodeList = barcodeList.Split(',').ToList();
            }

            List<MasProductUnit> resp = new List<MasProductUnit>();
            var sql = this.context.MasProductUnits.Where(
                    x => ((pdBarcodeList.Contains(x.UnitBarcode) || barcodeList == null || barcodeList == "")
                    )
                );
            resp = sql.ToList();
            return resp;
        }

        public async Task< SalQuotationHd> CreateQuotation(SalQuotationHd obj)
        {
            SalQuotationHd resp = new SalQuotationHd();
            int runNumber = quotationRepositories.GetRunNumber(obj);
            obj.RunNumber = runNumber;
            obj.DocNo = obj.DocNo.Replace("#", "0").Substring(0, obj.DocNo.Length - runNumber.ToString().Length) + obj.RunNumber.ToString();
            int seqNo = 0;
            obj.Guid = Guid.NewGuid();
            foreach (SalQuotationDt row in obj.SalQuotationDt)
            {
                seqNo++;
                row.DocNo = obj.DocNo;
                row.SeqNo = seqNo;
            }

            using (var transection = this.context.Database.BeginTransaction())
            {
                try
                {
                    await this.CalculateStock(obj);
                    quotationRepositories.CreateQuotation(obj);
                    await quotationRepositories.CreatApprove(obj);
                    this.context.SaveChanges();
                    transection.Commit();
                }
                catch (Exception ex)
                {
                    transection.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            resp = obj;
            return resp;
        }
        public string GetMaxCardProfile(string pStrMaxcardId)
        {
            return quotationRepositories.GetMaxCardProfile(pStrMaxcardId);
        }
        public async Task<SalQuotationHd> GetQuotation(RequestData req)
        {
            return await quotationRepositories.GetQuotation(req);
        }

        public async Task<QueryResult<QuotationResource>> SearchList(QuotationHdQuery req)
        {
            return await quotationRepositories.SearchList(req);
        }

        public async Task<QueryResult<SalQuotationHd>> GetQuotationHDList(QuotationHdQuery req)
        {
            return await quotationRepositories.GetQuotationHdList(req);
        }

        public int GetQuotationHdCount(RequestData req)
        {
            return  quotationRepositories.GetQuotationHdCount(req);
        }

        public async Task<List<SalQuotationHd>> GetQuotationHdRemainList(RequestData req)
        {
            return await quotationRepositories.GetQuotationHdRemainList(req);
        }

        public async Task<SalQuotationHd> UpdateQuotation(SalQuotationHd obj)
        {
            SalQuotationHd resp = new SalQuotationHd();
            if (obj.DocStatus == "Active")
            {   //เฉพาะ Active
                this.CalculateStock(obj);
            }
            else
            {   //เฉพาะ Wait, Ready, Cancel
            }

            using (var transection = this.context.Database.BeginTransaction())
            {
                try
                {
                    if (obj.DocStatus == "Active")
                    {   //เฉพาะ Active
                        quotationRepositories.UpdateQuotation(obj);
                    }
                    else
                    {   //เฉพาะ Wait, Ready, Cancel
                        quotationRepositories.UpdateDocStatusQuotation(obj);
                        if ("Cancel".Equals(obj.DocStatus))
                        {
                            await quotationRepositories.CancelApprove(obj);
                        }
                    }

                    this.context.SaveChanges();
                    transection.Commit();
                }
                catch (Exception ex)
                {
                    transection.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            resp = obj;
            return resp;
        }
        public async Task<SysApproveStep[]> GetApproveStep(SalQuotationHd param)
        {
            return await quotationRepositories.GetApproveStep(param);
        }
        public async Task<MasPayType[]> GetArrayPayType()
        {
            return await quotationRepositories.GetArrayPayType();
        }

        public async Task<MasEmployee[]> GetArrayEmployee()
        {
            return await quotationRepositories.GetArrayEmployee();
        }

        public async Task<MasEmployee> GetEmployee(string pStrEmpCode)
        {
            return await quotationRepositories.GetEmployee(pStrEmpCode);
        }

    }
}
