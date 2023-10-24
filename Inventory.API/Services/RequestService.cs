using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;

        readonly string urlMasterAPI = @"https://maxstation-masterdata-api.pt.co.th";

        public RequestService(
            IRequestRepository requestRepository,
            IUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }

        public List<InvRequestHds> GetRequestHDList(RequestQueryResource query)
        {
            return  _requestRepository.GetRequestHDList(query);
        }

        public async Task<List<InvRequestHds>> GetRequestHDListNew(RequestData req)
        {
            return await _requestRepository.GetRequestHdListNew(req);
        }

        public async Task<int> GetRequestHDCount(RequestData req)
        {
            return await _requestRepository.GetRequestHdCount(req);
        }

        public InvRequest GetRequest(RequestQueryResource req)
        {
            return _requestRepository.GetRequest(req);
        }

        public async Task<InvRequest> CreateRequest(InvRequest obj)
        {
            //this.CalculateStock(obj.InvRequestDt);
           
            try
            {
                int runNumber = _requestRepository.GetRunNumber(obj);
                obj.RunNumber = runNumber;
                obj.DocNo = obj.DocNo.Replace("#", "0").Substring(0, obj.DocNo.Length - runNumber.ToString().Length) + obj.RunNumber.ToString();
                int seqNo = 1;
                obj.Guid = Guid.NewGuid();

                foreach (InvRequestDt row in obj.InvRequestDt)
                {
                    row.DocNo = obj.DocNo;
                    row.SeqNo = seqNo;
                    row.StockQty = _requestRepository.CalculateStockQty(row.PdId, row.UnitId, row.ItemQty.Value);
                    row.StockRemain = row.StockQty;
                    seqNo++;
                }

                await _requestRepository.CreateRequest(obj);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return obj;
        }

        public async Task<InvRequest> UpdateRequest(InvRequest obj)
        {
            if (obj.DocStatus != "Reference")
            { 
                //เฉพาะ Reference ที่ไม่ต้อง CalculateStock
                //this.CalculateStock(obj.InvRequestDt);
                obj.InvRequestDt.ForEach(x => { x.StockQty = _requestRepository.CalculateStockQty(x.PdId, x.UnitId, x.ItemQty.Value); x.StockRemain = x.StockQty; });
            }

            try
            {
                await _requestRepository.UpdateRequest(obj);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return obj;
        }

        public List<InvRequestDt> CalculateStock(List<InvRequestDt> obj)
        {
            List<InvRequestDt> resp = new List<InvRequestDt>();
            try
            {
                //สร้างตัวแปรสำหรับเรียกข้อมูล Service : MasterData
                string pdIdStr = "";
                for (int i = 0; i < obj.Count; i++)
                {
                    pdIdStr += obj[i].PdId;
                    if (i == obj.Count - 1)
                    {
                        pdIdStr += "";
                    }
                    else
                    {
                        pdIdStr += ",";
                    }
                }
                RequestQueryResource req = new RequestQueryResource();
                req.PDListID = pdIdStr;

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
                foreach (InvRequestDt row in obj)
                {
                    var dt = pdUnit.Find(x => x.PdId == row.PdId);
                    if (dt != null)
                    {
                        row.StockQty = row.ItemQty * dt.UnitRatio;
                        row.StockRemain = row.StockQty;
                    }
                }
                resp = obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resp;
        }
    }
}
