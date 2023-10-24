using Finance.API.Domain.Models;
using Finance.API.Domain.Models.MSMQ;
using Finance.API.Domain.Models.Queries;
using Finance.API.Domain.Repositories;
using Finance.API.Domain.Services;
using Finance.API.Resources.Recive;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finance.API.Services
{
    public class ReceiveService : IReceiveService
    {
        private readonly IReceiveRepository _receiveRepository;
        private readonly ITaxInvoiceRepository _taxInvoiceRepository;
        private readonly ICreditSaleRepository _creditSaleRepository;
        private readonly IConfigApiRepository _configApiRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReceiveService));

        public ReceiveService(
            IReceiveRepository receiveRepository,
            ITaxInvoiceRepository taxInvoiceRepository,
            ICreditSaleRepository creditSaleRepository,
            IConfigApiRepository configApiRepository,
            IUnitOfWork unitOfWork,
            IServiceScopeFactory serviceScopeFactory)
        {
            _receiveRepository = receiveRepository;
            _taxInvoiceRepository = taxInvoiceRepository;
            _creditSaleRepository = creditSaleRepository;
            _configApiRepository = configApiRepository;
            _unitOfWork = unitOfWork;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<QueryResult<FinReceiveHd>> ListAsync(ReceiveHdQuery query)
        {
            return await _receiveRepository.ListAsync(query);
        }

        public async Task<FinReceiveHd> FindByIdAsync(ReceiveHdQuery query)
        {
            return await _receiveRepository.FindByIdAsync(query.Guid);
        }

        public async Task<ReceiveHdResponse> SaveAsync(FinReceiveHd obj)
        {
            try
            {
                int runNumber = _receiveRepository.GetRunNumber(obj);
                var docNo = obj.DocNo.Replace("#", "0").Substring(0, obj.DocNo.Length - runNumber.ToString().Length) + runNumber.ToString();
                obj.DocNo = docNo;
                obj.RunNumber = runNumber;
                obj.CreatedDate = DateTime.Now;
                obj.UpdatedDate = DateTime.Now;
                obj.Guid = Guid.NewGuid();
                await _receiveRepository.AddHdAsync(obj);

                int seq = 1;
                foreach (var dt in obj.FinReceiveDt)
                {
                    dt.DocNo = docNo;
                    dt.SeqNo = seq;
                    seq++;
                    await _receiveRepository.AddDtAsync(dt);
                }

                seq = 1;
                foreach (var pay in obj.FinReceivePay)
                {
                    pay.BrnCode = obj.BrnCode;
                    pay.DocNo = docNo;
                    pay.SeqNo = seq;
                    seq++;
                    await _receiveRepository.AddPayAsync(pay);
                }

                if (obj.FinReceivePay.Count > 0) {
                    await _receiveRepository.UpdateRemainFinBalance(obj);
                }
                await _unitOfWork.CompleteAsync();
                await SendMessageQue(obj);

                return new ReceiveHdResponse(obj);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ReceiveHdResponse($"{ex.Message}");
            }
        }

        private async Task SendMessageQue(FinReceiveHd finReceiveHd)
        {
            var siteManagementUrl = _configApiRepository.GetApiConfigBySystemIdAndApiId("SiteManagement", "M001");

            if(siteManagementUrl != null)
            {
                var payments = new List<Payment>();

                if(finReceiveHd.FinReceivePay != null)
                {
                    foreach (var finReceivepay in finReceiveHd.FinReceivePay)
                    {
                        var taxinvoiceHd = _taxInvoiceRepository.GetTaxInvoiceHdByFinReceivePay(finReceivepay, finReceiveHd.CustCode);

                        if (taxinvoiceHd != null)
                        {
                            var taxinvoiceDts = _taxInvoiceRepository.GetTaxInvoiceByDocNo(taxinvoiceHd.DocNo);
                            var creditSale = _creditSaleRepository.GetCreditSaleByTxNo(taxinvoiceHd.DocNo);
                            var payment = new Payment();
                            payment.brnNew = taxinvoiceHd.BrnCode;
                            payment.discountAmt = 0;
                            payment.discountPercent = 0;
                            payment.discountType = "B";
                            payment.inNo = taxinvoiceHd.DocNo;
                            payment.paymentDate = finReceiveHd.DocDate?.ToString("dd/MM/yyyy");
                            var paymentDetails = new List<Payment.PaymentDetail>();

                            foreach (var taxinvoiceDt in taxinvoiceDts)
                            {
                                int seqNo = 1;
                                var paymentDetail = new Payment.PaymentDetail();
                                paymentDetail.debitCreditAmt = 0;
                                paymentDetail.debitCreditType = "N";
                                paymentDetail.debitCreditVatAmt = 0;
                                paymentDetail.discountAmt = taxinvoiceDt.DiscAmt + taxinvoiceDt.DiscHdAmt;
                                paymentDetail.discountPercent = 0;
                                paymentDetail.discountType = "B";
                                paymentDetail.itemCode = taxinvoiceDt.PdId;
                                paymentDetail.itemName = taxinvoiceDt.PdName;
                                paymentDetail.seqNo = seqNo++;
                                paymentDetail.totalPaymentAmt = taxinvoiceDt.SumItemAmt;
                                paymentDetail.totalUnit = (int)taxinvoiceDt.ItemQty;
                                paymentDetail.transactionID = creditSale.RefNo;
                                paymentDetail.unitPrice = (int)taxinvoiceDt.UnitPrice;
                                paymentDetail.vatAmt = taxinvoiceDt.VatAmt;
                                paymentDetails.Add(paymentDetail);
                            }
                            payment.paymentDetailList = paymentDetails;
                            payment.totalPaymentAmt = taxinvoiceHd.SubAmt;
                            payment.transactionID = creditSale.RefNo;
                            payment.transactionType = "01";
                            payment.vatAmt = taxinvoiceHd.VatAmt;
                            payments.Add(payment);
                        }
                    }


                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    var client = new HttpClient(clientHandler);
                    var req = new HttpRequestMessage(HttpMethod.Post, siteManagementUrl.ApiUrl) { Content = new StringContent(JsonConvert.SerializeObject(payments), System.Text.Encoding.UTF8, "application/json") };
                    var httpResponse = await client.SendAsync(req);

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        log.Error($"Cannot retrieve tasks MSMQ");
                        //throw new Exception("Cannot retrieve tasks");
                    }

                    var content = await httpResponse.Content.ReadAsStringAsync();
                    var tasks = JsonConvert.DeserializeObject<Payment>(content);
                }
            }
        }


        public async Task<ReceiveHdResponse> UpdateAsync(FinReceiveHd obj)
        {
            try
            {
                var hd = await _receiveRepository.FindByIdAsync(Guid.Parse(obj.Guid.ToString()));
                if (hd == null)
                    return new ReceiveHdResponse("FinReceiveHd not found.");

                List<FinReceiveDt> dt = hd.FinReceiveDt
                    .Where(x => x.CompCode == hd.CompCode && x.BrnCode == hd.BrnCode
                    && x.LocCode == hd.LocCode && x.DocNo == hd.DocNo).ToList();
                if (dt == null)
                    return new ReceiveHdResponse("Invalid FinReceiveDt.");

                List<FinReceivePay> pay = hd.FinReceivePay
                    .Where(x => x.CompCode == hd.CompCode && x.BrnCode == hd.BrnCode
                    && x.LocCode == hd.LocCode && x.DocNo == hd.DocNo).ToList();
                if (hd.ReceiveType == "รับชำระ" && pay == null)
                {
                    //ถ้าเป็นประเภทรับชำระ ต้องมีการตัดหนี้เสมอ
                    return new ReceiveHdResponse("Invalid FinReceivePay.");
                }
                
                int seq = 1;
                foreach (var row in obj.FinReceiveDt)
                {
                    row.DocNo = obj.DocNo;
                    row.SeqNo = seq;
                    seq++;
                }

                seq = 1;
                foreach (var row in obj.FinReceivePay)
                {
                    row.DocNo = obj.DocNo;
                    row.SeqNo = seq;
                    seq++;
                }

                if (obj.DocStatus == "Active")
                {   //เฉพาะ Active
                    var dtBackup = obj.FinReceivePay.ToList();

                    //คืนค่าเดิมก่อน
                    obj.FinReceivePay = pay;
                    await _receiveRepository.ReturnRemainFinBalance(obj);

                    //Update ค่าใหม่
                    obj.FinReceivePay = dtBackup;
                    await _receiveRepository.UpdateRemainFinBalance(obj);
                    //var taxinvoiceHds = await _receiveRepository.UpdateRemainFinBalance(obj);
                    await SendMessageQue(obj);

                    obj.UpdatedBy = obj.UpdatedBy;
                    obj.UpdatedDate = DateTime.Now;
                }
                else if (obj.DocStatus == "Cancel")
                {
                    //Get SalCreditsaleDts ของเดิมมาเพื่อคืนค่า จะไม่ดึงจาก Obj ที่ส่งมา เนื่องจากอาจมีการเปลี่ยนแปลงค่าบนหน้าจอ
                    obj.FinReceivePay = pay;
                    await _receiveRepository.ReturnRemainFinBalance(obj);

                    obj.DocStatus = obj.DocStatus;
                    obj.UpdatedBy = obj.UpdatedBy;
                    obj.UpdatedDate = DateTime.Now;
                }
                else
                {
                    obj.DocStatus = obj.DocStatus;
                    obj.UpdatedBy = obj.UpdatedBy;
                    obj.UpdatedDate = DateTime.Now;
                }

                //List<SalQuotationDt> checkRemain = await _creditSaleRepository.CheckRemainQuotation(obj);
                //var checkRemainStr = "ไม่อนุญาตให้กรอกจำนวนสินค้าเกินจำนวนคงเหลือ";
                //if (checkRemain.Count > 0)
                //{
                //    foreach (SalQuotationDt dt in checkRemain)
                //    {
                //        checkRemainStr += "<br>รหัส " + dt.UnitBarcode + " : " + dt.PdName + " คงเหลือ " + dt.StockRemain + " " + dt.UnitName;
                //    }
                //    resp.IsSuccess = false;
                //    resp.Message = checkRemainStr;
                //    return resp;
                //}

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    if (obj.DocStatus == "Active")
                    {
                        //เฉพาะ Active
                        _receiveRepository.UpdateAsync(obj);
                        _receiveRepository.RemoveDtAsync(dt);
                        _receiveRepository.AddDtListAsync(obj.FinReceiveDt);
                        _receiveRepository.RemovePayAsync(pay);
                        _receiveRepository.AddPayListAsync(obj.FinReceivePay);
                    }
                    else
                    {
                        //เฉพาะ Wait, Ready, Cancel
                        _receiveRepository.UpdateAsync(obj);
                    }
                    await _unitOfWork.CompleteAsync();
                }
                return new ReceiveHdResponse(obj);
            }
            catch (Exception ex)
            {
                return new ReceiveHdResponse($"An error occurred when updating the FinReceiveHd: {ex.Message}");
            }
        }

        public async Task<List<FinReceivePay>> GetRemainFinBalanceList(ReceiveQuery query)
        {
            return await _receiveRepository.GetRemainFinBalanceList(query);
        }

        public async Task<FinReceivePay[]> GetFinReceivePays(FinReceiveHd pInput)
        {
            return await _receiveRepository.GetFinReceivePays(pInput);
        }

        public string GetErrorMessage(Exception pException)
        {
            if(pException == null)
            {
                return string.Empty;
            }
            string result = pException.StackTrace;
            while(pException.InnerException != null)
            {
                pException = pException.InnerException;
            }
            result = pException.Message + Environment.NewLine + result;
            return result;
        }
        public async Task<MasMapping[]> GetMasMapping()
        {
            return await _receiveRepository.GetMasMapping();
        }
        public async Task<ModelSumRecivePayResult[]> SumReceivePay(string pStrComCode, string pStrBrnCode, DateTime pDatDocDate)
        {
            return await _receiveRepository.SumReceivePay(pStrComCode, pStrBrnCode, pDatDocDate);
        }
    }
}
