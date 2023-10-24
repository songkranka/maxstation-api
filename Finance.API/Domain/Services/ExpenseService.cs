using Finance.API.Domain.Models;
using Finance.API.Domain.Models.Queries;
using Finance.API.Domain.Models.Request;
using Finance.API.Domain.Models.Response;
using Finance.API.Domain.Repositories;
using Finance.API.Helpers;
using Finance.API.Models;
using Finance.API.Resources.Expense;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Finance.API.Domain.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IAppUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IHttpClientService _httpClientService;
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(
            IAppUnitOfWork unitOfWork,
            IJwtService jwtService,
            IHttpClientService httpClientService,
            IExpenseRepository expenseRepository)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _httpClientService = httpClientService;
            _expenseRepository = expenseRepository;
        }

        public async Task<QueryResult<FinExpenseHd>> ListAsync(ExpenseQuery query)
        {
            return await _expenseRepository.ListAsync(query);
        }

        public async Task<string> GetMasDocPattern(string compCode, string brnCode, string docType, string docDate)
        {
            var docPatternRequest = new DocPatternRequest
            {
                CompCode = compCode,
                BrnCode = brnCode,
                DocType = docType,
                DocDate = Convert.ToDateTime(docDate)
            };

            var jwt = _jwtService.GenerateJwt();
            var result = await _httpClientService.PostAsync<Object, DocPatternResponse>(jwt, "https://uat-maxstation.pt.co.th/masterdata/api/DocPattern/GetDocPattern", docPatternRequest);
            var docPattern = result.Data.Resource.Pattern;
            return docPattern;
        }

        public async Task<List<ExpenseTable>> GetMasExpenseTableAsync(string status, string compCode, string brnCode, string locCode, string docNo)
        {
            return  await _expenseRepository.GetMasExpenseTableAsync(status, compCode, brnCode, locCode, docNo);
        }

        public async Task<FinExpenseHd> GetExpenseHdAsync(string compCode, string brnCode, string locCode, Guid guId)
        {
            return await _expenseRepository.GetExpenseHdAsync(compCode, brnCode, locCode, guId);
        }

        public async Task<List<ExpenseEssTable>> GetExpenseEssTableAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            return await _expenseRepository.GetExpenseEssTableAsync(compCode, brnCode, locCode, docNo);
        }

        public async Task<SaveExpenseResponse> SaveExpenseHdAsync(SaveExpenseRequest request)
        {
            try
            {
                var finExpenseHd = await _expenseRepository.GetExpenseHdByDocNoAsync(request.FinExpenseHd.CompCode, request.FinExpenseHd.BrnCode, request.FinExpenseHd.LocCode, request.FinExpenseHd.DocNo);
                
                if (finExpenseHd == null)
                {
                    var duplicateData = await _expenseRepository.GetExpenseHdByDocDateAsync(request.FinExpenseHd.CompCode, request.FinExpenseHd.BrnCode, request.FinExpenseHd.LocCode, request.FinExpenseHd.DocDate);

                    if (duplicateData != null)
                    {
                        return new SaveExpenseResponse($"ข้ำมูลในระบบซ้ำ");
                    }

                    int intLastRunning = 0;
                    var maxRunNumber = await _expenseRepository.GetExpenseHdMaxRunNumber(request.FinExpenseHd.CompCode, request.FinExpenseHd.BrnCode, request.FinExpenseHd.LocCode, request.FinExpenseHd.DocDate);
                    var countExpenseHd = await _expenseRepository.CountExpenseHd(request.FinExpenseHd.CompCode, request.FinExpenseHd.BrnCode, request.FinExpenseHd.LocCode, request.FinExpenseHd.DocDate);


                    if (maxRunNumber.HasValue)
                    {
                        intLastRunning = Math.Max((int)maxRunNumber, countExpenseHd);
                    }

                    intLastRunning++;

                    int counter = 0;
                    int startIndex = -1;

                    while ((startIndex = (request.FinExpenseHd.DocPattern.IndexOf("#", startIndex + 1))) != -1)
                    {
                        counter++;
                    }
                    var docPattern = request.FinExpenseHd.DocPattern.Substring(0, request.FinExpenseHd.DocPattern.Length - counter);
                    request.FinExpenseHd.DocNo = docPattern + intLastRunning.ToString().PadLeft(counter, '0');
                    request.FinExpenseHd.RunNumber = intLastRunning;
                    request.FinExpenseHd.Guid = Guid.NewGuid();
                    
                    if(request.FinExpenseHd.WorkType == "D")
                    {
                        request.FinExpenseHd.WorkStart = "00:00";
                        request.FinExpenseHd.WorkFinish = "23:59";
                    }

                    await _expenseRepository.AddExpenseHdAsync(request.FinExpenseHd);

                    var finExpenseDts = new List<FinExpenseDt>();
                    var seqNo = 1;

                    foreach (var expenseTable in request.ExpenseTables)
                    {
                        var finExpenseDtParent = new FinExpenseDt();
                        finExpenseDtParent.CompCode = request.FinExpenseHd.CompCode;
                        finExpenseDtParent.BrnCode = request.FinExpenseHd.BrnCode;
                        finExpenseDtParent.LocCode = request.FinExpenseHd.LocCode;
                        finExpenseDtParent.DocNo = request.FinExpenseHd.DocNo;
                        finExpenseDtParent.SeqNo = seqNo++;
                        finExpenseDtParent.ExpenseNo = expenseTable.Id;
                        finExpenseDtParent.CateName = expenseTable.Header;
                        finExpenseDts.Add(finExpenseDtParent);

                        foreach (var body in expenseTable.Body)
                        {
                            var finExpenseDt = new FinExpenseDt();
                            finExpenseDt.CompCode = request.FinExpenseHd.CompCode;
                            finExpenseDt.BrnCode = request.FinExpenseHd.BrnCode;
                            finExpenseDt.LocCode = request.FinExpenseHd.LocCode;
                            finExpenseDt.DocNo = request.FinExpenseHd.DocNo;
                            finExpenseDt.SeqNo = seqNo++;
                            finExpenseDt.ExpenseNo = body.CategoryId;
                            finExpenseDt.Parent = expenseTable.Id;
                            finExpenseDt.CateName = expenseTable.Header;
                            finExpenseDt.BaseName = body.Title;
                            finExpenseDt.BaseQty = body.Qty;
                            finExpenseDt.BaseUnit = body.Unit;
                            finExpenseDt.ItemName = body.Data;
                            finExpenseDt.ItemQty = body.Number;
                            finExpenseDts.Add(finExpenseDt);
                        }
                    }
                    await _expenseRepository.AddExpenseDtRangeAsync(finExpenseDts);

                    var seqNoEss = 1;
                    var finExpenseEssList = new List<FinExpenseEss>();

                    foreach (var expenseEssTable in request.ExpenseEssTables)
                    {
                        var finExpenseEss = new FinExpenseEss();
                        finExpenseEss.CompCode = request.FinExpenseHd.CompCode;
                        finExpenseEss.BrnCode = request.FinExpenseHd.BrnCode;
                        finExpenseEss.LocCode = request.FinExpenseHd.LocCode;
                        finExpenseEss.DocNo = request.FinExpenseHd.DocNo;
                        finExpenseEss.SeqNo = seqNoEss++;
                        finExpenseEss.EssNo = expenseEssTable.EssNumber;
                        finExpenseEss.EssDesc = expenseEssTable.EssDetail;
                        finExpenseEssList.Add(finExpenseEss);
                    }
                    await _expenseRepository.AddExpenseEssRangeAsync(finExpenseEssList);

                    await _unitOfWork.CompleteAsync();
                }
                else
                {
                    request.FinExpenseHd.Guid = (Guid)finExpenseHd.Guid;
                    finExpenseHd.UpdatedBy = request.FinExpenseHd.CreatedBy;
                    finExpenseHd.UpdatedDate = DateTime.Now;
                    finExpenseHd.WorkType = request.FinExpenseHd.WorkType;
                    finExpenseHd.Remark = request.FinExpenseHd.Remark;

                    if (request.FinExpenseHd.WorkType == "D")
                    {
                        finExpenseHd.WorkStart = "00:00";
                        finExpenseHd.WorkFinish = "23:59";
                    }
                    else
                    {
                        finExpenseHd.WorkStart = request.FinExpenseHd.WorkStart;
                        finExpenseHd.WorkFinish = request.FinExpenseHd.WorkFinish;
                    }

                    _expenseRepository.UpdateExpenseHd(finExpenseHd);
                    await _unitOfWork.CompleteAsync();

                    var finExpenseDtRemove = await _expenseRepository.GetExpenseDtByDocNoAsync(request.FinExpenseHd.CompCode, request.FinExpenseHd.BrnCode, request.FinExpenseHd.LocCode, request.FinExpenseHd.DocNo);

                    if(finExpenseDtRemove.Count > 0)
                    {
                        _expenseRepository.RemoveRangeExpenseDt(finExpenseDtRemove);
                        await _unitOfWork.CompleteAsync();
                    }

                    var finExpenseDts = new List<FinExpenseDt>();
                    var seqNo = 1;

                    foreach (var expenseTable in request.ExpenseTables)
                    {
                        var finExpenseDtParent = new FinExpenseDt();
                        finExpenseDtParent.CompCode = request.FinExpenseHd.CompCode;
                        finExpenseDtParent.BrnCode = request.FinExpenseHd.BrnCode;
                        finExpenseDtParent.LocCode = request.FinExpenseHd.LocCode;
                        finExpenseDtParent.DocNo = request.FinExpenseHd.DocNo;
                        finExpenseDtParent.SeqNo = seqNo++;
                        finExpenseDtParent.ExpenseNo = expenseTable.Id;
                        finExpenseDtParent.CateName = expenseTable.Header;
                        finExpenseDts.Add(finExpenseDtParent);

                        foreach (var body in expenseTable.Body)
                        {
                            var finExpenseDt = new FinExpenseDt();
                            finExpenseDt.CompCode = request.FinExpenseHd.CompCode;
                            finExpenseDt.BrnCode = request.FinExpenseHd.BrnCode;
                            finExpenseDt.LocCode = request.FinExpenseHd.LocCode;
                            finExpenseDt.DocNo = request.FinExpenseHd.DocNo;
                            finExpenseDt.SeqNo = seqNo++;
                            finExpenseDt.ExpenseNo = body.CategoryId;
                            finExpenseDt.Parent = expenseTable.Id;
                            finExpenseDt.CateName = expenseTable.Header;
                            finExpenseDt.BaseName = body.Title;
                            finExpenseDt.BaseQty = body.Qty;
                            finExpenseDt.BaseUnit = body.Unit;
                            finExpenseDt.ItemName = body.Data;
                            finExpenseDt.ItemQty = body.Number;
                            finExpenseDts.Add(finExpenseDt);
                        }
                    }
                    await _expenseRepository.AddExpenseDtRangeAsync(finExpenseDts);
                    await _unitOfWork.CompleteAsync();

                    var finExpenseEssRemove = await _expenseRepository.GetExpenseEssByDocNoAsync(request.FinExpenseHd.CompCode, request.FinExpenseHd.BrnCode, request.FinExpenseHd.LocCode, request.FinExpenseHd.DocNo);
                    
                    if (finExpenseEssRemove.Count > 0)
                    {
                        _expenseRepository.RemoveRangeExpenseEss(finExpenseEssRemove);
                        await _unitOfWork.CompleteAsync();
                    }

                    var seqNoEss = 1;
                    var finExpenseEssList = new List<FinExpenseEss>();

                    foreach (var expenseEssTable in request.ExpenseEssTables)
                    {
                        var finExpenseEss = new FinExpenseEss();
                        finExpenseEss.CompCode = request.FinExpenseHd.CompCode;
                        finExpenseEss.BrnCode = request.FinExpenseHd.BrnCode;
                        finExpenseEss.LocCode = request.FinExpenseHd.LocCode;
                        finExpenseEss.DocNo = request.FinExpenseHd.DocNo;
                        finExpenseEss.SeqNo = seqNoEss++;
                        finExpenseEss.EssNo = expenseEssTable.EssNumber;
                        finExpenseEss.EssDesc = expenseEssTable.EssDetail;
                        finExpenseEssList.Add(finExpenseEss);
                    }
                    await _expenseRepository.AddExpenseEssRangeAsync(finExpenseEssList);

                    await _unitOfWork.CompleteAsync();

                    var expenseTables = await _expenseRepository.GetMasExpenseTableAsync("Edit", request.FinExpenseHd.CompCode, request.FinExpenseHd.BrnCode, request.FinExpenseHd.LocCode, request.FinExpenseHd.DocNo);
                    request.ExpenseTables = expenseTables;
                }

                
                return new SaveExpenseResponse(request);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new SaveExpenseResponse($"An error occurred when saving the expenseHd: {strMessage}");
            }
        }

        public async Task<UpdateStatusExpenseResponse> UpdateExpenseHdStatusAsync(FinExpenseHd request)
        {
            var expenseHdResult = await _expenseRepository.GetExpenseHdByDocNoAsync(request.CompCode, request.BrnCode, request.LocCode, request.DocNo);
            if(expenseHdResult != null)
            {
                expenseHdResult.UpdatedBy = request.UpdatedBy;
                expenseHdResult.UpdatedDate = DateTime.Now;
                expenseHdResult.DocStatus = request.DocStatus;
                _expenseRepository.UpdateStatus(expenseHdResult);
                await _unitOfWork.CompleteAsync();
            }
            return new UpdateStatusExpenseResponse(request);
        }
    }
}
