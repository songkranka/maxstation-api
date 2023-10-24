using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Models.Response;
using PostDay.API.Domain.Repositories;
using PostDay.API.Domain.Services;
using PostDay.API.Domain.Services.Communication.PostDay;
using PostDay.API.Resources.PostDay;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Services
{
    public class CloseDayService : ICloseDayService
    {
        private readonly ICloseDayRepository _closedayRepository;
        private readonly IMasControlRepository _masControlRepository;
        private IUnitOfWork _unitOfWork;

        public CloseDayService(
            ICloseDayRepository closedayRepository,
            IMasControlRepository masControlRepository,
            IUnitOfWork unitOfWork)
        {
            _closedayRepository = closedayRepository;
            _masControlRepository = masControlRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCloseDayResponse> GetDocument(GetDocumentRequest req)
        {
            var result = new GetDocumentResponse();

            var systremDate = DateTime.ParseExact(req.SystemDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); //new CultureInfo("en-US")

            if (systremDate == docDate)
            {
                //return _closedayRepository.GetDocument(req); // ตรวจสอบความถูกต้องก่อนปิดสิ้นวัน  
                var dopPostdayHd = await _closedayRepository.GetDopPostDayHd(req, docDate);
                var dopPostdayDts = await _closedayRepository.GetDopPostDayDt(req, docDate);
                var crItems = dopPostdayDts.Where(x => x.DocType == "CR").ToList();
                var drItems = dopPostdayDts.Where(x => x.DocType == "DR").ToList();
                var formulas = await _closedayRepository.GetFormula(req, docDate);
                var sumData = await _closedayRepository.GetSumDate(req, docDate);
                var checkBeforeSaveItems = await _closedayRepository.CheckBeforeSave(req, docDate);   //dop_validate

                result.DopPostdayHd = dopPostdayHd;
                result.CrItems = crItems;
                result.DrItems = drItems;
                result.FormulaItems = formulas;
                result.SumData = sumData;
                result.CheckBeforeSaveItems = checkBeforeSaveItems.checkBeforeSaves;
                result.ListValidatePostPaid = checkBeforeSaveItems.mecPostPaidValidates;
                

                return new GetCloseDayResponse(result);
            }
            else
            {
                return await _closedayRepository.GetTransactionCloseday(req); // เอาผลลัพธ์ที่ได้หลังการบันทึกปิดสิ้นวันมาแสดงที่หน้าจอ
            }
        }

        public async Task<ClosedayResponse> SaveDocument(SaveDocumentRequest request)
        {
            try
            {
                #region Create Postday 
                var docDate = DateTime.ParseExact(request.DopPostdayHd.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                await _closedayRepository.AddDopPostdayAsync(request);

                //if (request.CheckBeforeSaveItems?.Any() ?? false)
                //{
                //    var dopPostdayValidates = request.CheckBeforeSaveItems.Select((x, i) => new DopPostdayValidate()
                //    {
                //        BrnCode = request.DopPostdayHd.BrnCode,
                //        CompCode = request.DopPostdayHd.CompCode,
                //        LocCode = request.DopPostdayHd.LocCode,
                //        DocDate = docDate,
                //        SeqNo = i + 1,
                //        ValidRemark = x.Label,
                //        ValidResult = x.PassValue
                //    }).ToList();

                //    await _closedayRepository.AddDopPostdayValidateAsync(dopPostdayValidates);
                //}
                //await _unitOfWork.CompleteAsync();
                #endregion

                #region update Post Transaction
                var updateClosedayRequest = new PostDayResource()
                {
                    CompCode = request.DopPostdayHd.CompCode,
                    BrnCode = request.DopPostdayHd.BrnCode,
                    WDate = docDate,
                    User = request.DopPostdayHd.User
                };
                await _closedayRepository.UpdatePeriodAsync(updateClosedayRequest);
                await _closedayRepository.UpdateRequestAsync(updateClosedayRequest);
                await _closedayRepository.UpdateReceiveAsync(updateClosedayRequest);
                await _closedayRepository.UpdateTranferOutAsync(updateClosedayRequest);
                await _closedayRepository.UpdateTranferInAsync(updateClosedayRequest);
                await _closedayRepository.UpdateWithdrawAsync(updateClosedayRequest);
                await _closedayRepository.UpdateReturnSupAsync(updateClosedayRequest);
                await _closedayRepository.UpdateAuditAsync(updateClosedayRequest);
                await _closedayRepository.UpdateAdjustAsync(updateClosedayRequest);
                await _closedayRepository.UpdateAdjustRequestAsync(updateClosedayRequest);
                await _closedayRepository.UpdateReturnOilAsync(updateClosedayRequest);
                await _closedayRepository.UpdateUnusableAsync(updateClosedayRequest);
                await _closedayRepository.UpdateQuotationAsync(updateClosedayRequest);
                await _closedayRepository.UpdateCashSaleAsync(updateClosedayRequest);
                await _closedayRepository.UpdateCreditSaleAsync(updateClosedayRequest);
                await _closedayRepository.UpdateTaxinvoiceAsync(updateClosedayRequest);
                await _closedayRepository.UpdateSalCndnAsync(updateClosedayRequest);
                await _closedayRepository.UpdateBillingAsync(updateClosedayRequest);
                await _closedayRepository.UpdateFinanceReceiveAsync(updateClosedayRequest);
                await _closedayRepository.UpdateBranchConfigAsync(updateClosedayRequest);
                await _closedayRepository.UpdateMasControlAsync(updateClosedayRequest);
                //await _unitOfWork.CompleteAsync();
                #endregion

                #region Other
                var warpadData = await _closedayRepository.GetDataToWarpadAsync(updateClosedayRequest);
                await _closedayRepository.SendDataToWarpadAsync(warpadData);
                await _closedayRepository.ExecuteStoredprocedureStockAsnyc(request, docDate);
                #endregion

                #region update mas_control and commit
                await _masControlRepository.UpdateWDateAsync(request.DopPostdayHd.CompCode, request.DopPostdayHd.BrnCode, request.DopPostdayHd.LocCode, "WDATE", "");
                await _unitOfWork.CompleteAsync();
                #endregion

                #region Create TaxInvoice
                var docPattern = _closedayRepository.GetDocPattern("TaxInvoice");
                var pattern = (docPattern == null) ? "" : docPattern.Pattern;
                pattern = pattern.Replace("Brn", updateClosedayRequest.BrnCode);
                pattern = pattern.Replace("yy", updateClosedayRequest.WDate.Value.ToString("yy"));
                pattern = pattern.Replace("MM", updateClosedayRequest.WDate.Value.ToString("MM"));
                var runningNo = _closedayRepository.GetRunningNo(updateClosedayRequest.CompCode, updateClosedayRequest.BrnCode, pattern);
                await _closedayRepository.CreateTaxInvoiceAsync(updateClosedayRequest, pattern, runningNo);
                await _unitOfWork.CompleteAsync();
                #endregion
                return new ClosedayResponse(request);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new ClosedayResponse($"An error occurred when saving the close day: {strMessage}");
            }
        }
    }
}
