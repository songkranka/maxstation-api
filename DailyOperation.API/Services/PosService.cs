using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Models.Request;
using DailyOperation.API.Domain.Models.Response;
using DailyOperation.API.Domain.Repositories;
using DailyOperation.API.Domain.Services;
using DailyOperation.API.Domain.Services.Communication;
using DailyOperation.API.Extensions;
using DailyOperation.API.Resources.POS;
using MaxStation.Entities.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DailyOperation.API.Resources.POS.SaveCreditSaleResource;
using System.Data.Entity;

namespace DailyOperation.API.Services
{
    public class PosService : IPosService
    {
        private readonly IPosRepository _posRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PosService(
            IPosRepository posRepository,
            IUnitOfWork unitOfWork
            )
        {
            _posRepository = posRepository;
            _unitOfWork = unitOfWork;
           
        }

        public string GetPosConnection()
        {
            return _posRepository.GetConn();
        }


        public QueryResult<POSCash> CashList(CashQueryResource query)
        {
            return  _posRepository.CashList(query);
        }

        public QueryResult<POSCredit> CreditList(CreditQueryResource query)
        {
            return _posRepository.CreditList(query);
        }

        public QueryResult<POSWithdraw> WithdrawList(WithdrawQueryResource query)
        {
            var withdraws = _posRepository.WithdrawList(query);
            return withdraws;
        }

        public QueryResult<POSReceive> ReceiveList(ReceiveQueryResource query)
        {
            return _posRepository.ReceiveList(query);
        }

        public async Task<CashsaleResponse> SaveCashSaleAsync(SaveCashSaleResource request)
        {
            try
            {
                var cashSaleCount = request._Cashsale.Count;
                var totalItem = request.TotalItem;

                if (cashSaleCount != totalItem)
                {
                    return new CashsaleResponse($"จำนวนรายการไม่ถูกต้อง กรุณากดประมวลผลอีกครั้ง");
                }

                var cashSaleMasProduct = request._Cashsale.Select(x => x.PluNumber);
                var masProductIsValid = ValidateMasProduct(cashSaleMasProduct);
                var masProductUnitIsValid =  ValidateMasProductUnit(cashSaleMasProduct);

                if (masProductIsValid.Any())
                {
                    var masProductResponse = masProductIsValid.Distinct().ToList();
                    string masProductIsValidResponse = string.Join(",", masProductResponse.Select(x => string.Format("'{0}'", x)));
                    var masProductRequest = request._Cashsale.Where(x => !masProductResponse.Contains(x.PluNumber)).ToList();

                    if (masProductUnitIsValid.Any())
                    {
                        var masProductUnitResponse = masProductUnitIsValid.Distinct().ToList();
                        string masProductUnitIsValidResponse = string.Join(",", masProductUnitResponse.Select(x => string.Format("'{0}'", x)));
                        var masProductUnitRequest = request._Cashsale.Where(x => !masProductUnitResponse.Contains(x.PluNumber)).ToList();
                        
                        request._Cashsale = masProductUnitRequest;
                        await _posRepository.AddCashSaleListAsync(request);
                        await _unitOfWork.CompleteAsync();
                        return new CashsaleResponse($@"ไม่พบหน่วยสินค้า {masProductUnitIsValidResponse} กรุณาตรวจสอบอีกครั้ง");
                    }
                    else
                    {
                        request._Cashsale = masProductRequest;
                        await _posRepository.AddCashSaleListAsync(request);
                        await _unitOfWork.CompleteAsync();
                        return new CashsaleResponse($@"ข้อมูลสินค้า {masProductIsValidResponse} ไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง");
                    }
                }
                else if (masProductUnitIsValid.Any())
                {
                    var masProductUnitResponse = masProductUnitIsValid.Distinct().ToList();
                    string masProductUnitIsValidResponse = string.Join(",", masProductUnitResponse.Select(x => string.Format("'{0}'", x)));
                    var masProductUnitRequest = request._Cashsale.Where(x => !masProductUnitResponse.Contains(x.PluNumber)).ToList();
                    
                    request._Cashsale = masProductUnitRequest;
                    await _posRepository.AddCashSaleListAsync(request);
                    await _unitOfWork.CompleteAsync();
                    return new CashsaleResponse($@"ไม่พบหน่วยสินค้า {masProductUnitIsValidResponse} กรุณาตรวจสอบอีกครั้ง");
                }
                else
                {
                    await _posRepository.AddCashSaleListAsync(request);
                    await _unitOfWork.CompleteAsync();
                    return new CashsaleResponse(request);
                }
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new CashsaleResponse($"An error occurred when saving the cashSale: {strMessage}");
            }
        }

        public async Task<CreditsaleResponse> SaveCreditSaleAsync(SaveCreditSaleResource request)
        {
            try
            {

                var cashSaleCount = request._Creditsale.Count;
                var totalItem = request.TotalItem;

                if (cashSaleCount != totalItem)
                {
                    return new CreditsaleResponse($"จำนวนรายการไม่ถูกต้อง กรุณากดประมวลผลอีกครั้ง");
                }

                var customerIsValid = _posRepository.ValidateCustomer(request);
                var creditSaleMasProduct = request._Creditsale.Select(x => x.PluNumber);
                var masProductIsValid = ValidateMasProduct(creditSaleMasProduct);
                var masProductUnitIsValid = ValidateMasProductUnit(creditSaleMasProduct);

                if (customerIsValid.Any())
                {
                    var customerResponse = customerIsValid.Distinct().ToList();
                    string customerIsValidResponse = string.Join(",", customerResponse.Select(x => string.Format("'{0}'", x)));
                    return new CreditsaleResponse($@"ข้อมูลลูกค้า {customerIsValidResponse} ไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง");
                }

                if (masProductIsValid.Any())
                {
                    var masProductResponse = masProductIsValid.Distinct().ToList();
                    string masProductIsValidResponse = string.Join(",", masProductResponse.Select(x => string.Format("'{0}'", x)));
                    var masProductRequest = request._Creditsale.Where(x => !masProductResponse.Contains(x.PluNumber)).ToList();

                    if (masProductUnitIsValid.Any())
                    {
                        var masProductUnitResponse = masProductUnitIsValid.Distinct().ToList();
                        string masProductUnitIsValidResponse = string.Join(",", masProductUnitResponse.Select(x => string.Format("'{0}'", x)));
                        var masProductUnitRequest = request._Creditsale.Where(x => !masProductUnitResponse.Contains(x.PluNumber)).ToList();

                        request._Creditsale = masProductUnitRequest;
                        await _posRepository.AddCreditSaleListAsync(request);
                        await _unitOfWork.CompleteAsync();
                        return new CreditsaleResponse($@"ไม่พบหน่วยสินค้า {masProductUnitIsValidResponse} กรุณาตรวจสอบอีกครั้ง");
                    }
                    else
                    {
                        request._Creditsale = masProductRequest;
                        await _posRepository.AddCreditSaleListAsync(request);
                        await _unitOfWork.CompleteAsync();
                        return new CreditsaleResponse($@"ข้อมูลสินค้า {masProductIsValidResponse} ไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง");
                    }
                }
                else if (masProductUnitIsValid.Any())
                {
                    var masProductUnitResponse = masProductUnitIsValid.Distinct().ToList();
                    string masProductUnitIsValidResponse = string.Join(",", masProductUnitResponse.Select(x => string.Format("'{0}'", x)));
                    var masProductUnitRequest = request._Creditsale.Where(x => !masProductUnitResponse.Contains(x.PluNumber)).ToList();

                    request._Creditsale = masProductUnitRequest;
                    await _posRepository.AddCreditSaleListAsync(request);
                    await _unitOfWork.CompleteAsync();
                    return new CreditsaleResponse($@"ไม่พบหน่วยสินค้า {masProductUnitIsValidResponse} กรุณาตรวจสอบอีกครั้ง");
                }
                else
                {
                    await _posRepository.AddCreditSaleListAsync(request);
                    await _unitOfWork.CompleteAsync();
                    return new CreditsaleResponse(request);
                }
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                // Do some logging stuff
                return new CreditsaleResponse($"An error occurred when saving the creditSale: {strMessage}");
            }
        }

        public async Task<WithdrawResponse> SaveWithdrawAsync(SaveWithdrawResource request)
        {
            try
            {
                var strMessage = string.Empty;

                foreach (var withdraw in request._Withdraw)
                {
                    if(withdraw.EmpCode == "" || withdraw.EmpCode == null)
                    {
                        strMessage = "กรุณากรอกรหัสพนักงาน";
                        return new WithdrawResponse($"An error occurred when saving the withdraw: {strMessage}");
                    }

                    if (withdraw.UserBrnCode == "" || withdraw.UserBrnCode == null)
                    {
                        strMessage = "กรุณากรอกส่วนงาน";
                        return new WithdrawResponse($"An error occurred when saving the withdraw: {strMessage}");
                    }
                    if (withdraw.ReasonId == "" || withdraw.ReasonId == null)
                    {
                        strMessage = "กรุณากรอกเหตุผลที่เบิก";
                        return new WithdrawResponse($"An error occurred when saving the withdraw: {strMessage}");
                    }
                }

                await _posRepository.AddWithdrawListAsync(request);
                await _unitOfWork.CompleteAsync();

                return new WithdrawResponse(request);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                // Do some logging stuff
                return new WithdrawResponse($"An error occurred when saving the withdraw: {strMessage}");
            }
        }

        public async Task<ReceiveResponse> SaveReceiveAsync(SaveReceiveResource request)
        {
            try
            {
                var strMessage = string.Empty;

                //foreach (var receive in request._POSReceives)
                //{
                //    if (receive.EmpCode == "" || withdraw.EmpCode == null)
                //    {
                //        strMessage = "กรุณากรอกรหัสพนักงาน";
                //        return new WithdrawResponse($"An error occurred when saving the withdraw: {strMessage}");
                //    }
                //}

                await _posRepository.AddReceiveListAsync(request);
                await _unitOfWork.CompleteAsync();

                return new ReceiveResponse(request);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                // Do some logging stuff
                return new ReceiveResponse($"An error occurred when saving the receive: {strMessage}");
            }
        }

        public DopPosConfig GetWithdrawStatus(WithdrawStatusRequest req)
        {
            return _posRepository.GetWithdrawStatus(req);
        }

        public string GetConn()
        {            
            return _posRepository.GetConn();
        }

        private List<string> ValidateMasProduct(IEnumerable<string> productIds)
        {
            var masProducts = _posRepository.ValidateMasProduct(productIds);

            return masProducts;
        }

        private List<string> ValidateMasProductUnit(IEnumerable<string> productIds)
        {
            var masProductUnits = _posRepository.ValidateMasProductUnit(productIds);

            return masProductUnits;
        }

        public CreditSummaryResponse GetCreditSummaryByBranch(CreditSummaryRequest query)
        {
            return _posRepository.GetCreditSummaryByBranch(query);
        }

        public async Task<PeriodCountResponse> GetPeriodCount(PeriodCountRequest query)
        {
            return await _posRepository.GetPeriodCount(query);
        }

        public async Task<DopPosConfig[]> GetDopPosConfig(GetDopPosConfigParam param)
        {
            return await _posRepository.GetDopPosConfig(param);
        }

        public async Task<CheckPeriodWaterResponse> CheckPeriodWater(PeriodCountRequest query)
        {
            var response = new CheckPeriodWaterResponse();
            int cntPos =  await _posRepository.GetPeriodWaterPOS(query);
            int cntMax =  await _posRepository.GetPeriodWaterMAX(query);
            response.result = (cntMax == cntPos);
            return response;
        }
    }
}
