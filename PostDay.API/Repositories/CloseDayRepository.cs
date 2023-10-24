using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PostDay.API.Domain.Models;
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Models.Request;
using PostDay.API.Domain.Models.Response;
using PostDay.API.Domain.Models.RestAPI;
using PostDay.API.Domain.Models.Soap;
using PostDay.API.Domain.Repositories;
using PostDay.API.Domain.Services.Communication.PostDay;
using PostDay.API.Helpers;
using PostDay.API.Resources.PostDay;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MaxStation.Utility.Helpers.CRM;
using PostDay.API.Services;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Ocsp;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Reflection.Emit;
using MaxStation.Utility.Helpers.Token;
using Azure.Core;

namespace PostDay.API.Repositories
{
    public class CloseDayRepository : SqlDataAccessHelper, ICloseDayRepository
    {
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(CloseDayRepository));
        private readonly ICrmService _crmService;
        private readonly ITokenService _tokenService;

        public CloseDayRepository(
            PTMaxstationContext context,
            IMapper mapper, 
            ICrmService crmService,
            ITokenService tokenService) : base(context)
        {
            _mapper = mapper;
            _crmService = crmService;
            _tokenService = tokenService;
        }

        public async Task<GetCloseDayResponse> GetTransactionCloseday(GetDocumentRequest req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var dopPostdayHd = await context.DopPostdayHds
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.DocDate == docDate).AsNoTracking().FirstOrDefaultAsync();
                var listDtData = await context.DopPostdayDts
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.DocDate == docDate).AsNoTracking().ToListAsync();
                var listCr = listDtData.Where(x => x.DocType == "CR").ToList();
                var listDr = listDtData.Where(x => x.DocType == "DR").ToList();
                var listResultFormula = new List<Formula>();
                var listFmData = await context.DopPostdaySums
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.DocDate == docDate).AsNoTracking().ToListAsync();

                if (listFmData.Count > 0)
                {
                    listResultFormula = _mapper.Map<List<DopPostdaySum>, List<Formula>>(listFmData);
                    for (int i = 0; i < listResultFormula.Count; i++)
                    {
                        listResultFormula[i].Unit = listFmData[i].UnitName;
                    }
                }

                var sumData = new SumInDay();
                if (dopPostdayHd != null)
                {
                    sumData.SumCashAmt = (decimal)dopPostdayHd.CashAmt;
                    sumData.SumDiffAmt = (decimal)dopPostdayHd.DiffAmt;
                    sumData.SumCashDepositAmt = (decimal)dopPostdayHd.DepositAmt;
                    sumData.SumChequeAmt = (decimal)dopPostdayHd.ChequeAmt;
                }

                var dopPostDayValidates = await context.DopPostdayValidates.Where(x =>
                    x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                    x.DocDate == docDate).ToListAsync();
                var listCheckBeforeSave = new List<CheckBeforeSave>();

                if (dopPostDayValidates.Count > 0)
                {
                    foreach (var dopPostDayValidate in dopPostDayValidates)
                    {
                        listCheckBeforeSave.Add(new CheckBeforeSave()
                        {
                            Label = dopPostDayValidate.ValidRemark,
                            PassValue = dopPostDayValidate.ValidResult,
                            ValidNo = dopPostDayValidate.SeqNo,
                            HaveValidSql = false,
                        });
                    }
                }

                var result = new GetDocumentResponse()
                {
                    DopPostdayHd = dopPostdayHd,
                    CrItems = listCr,
                    DrItems = listDr,
                    FormulaItems = listResultFormula,
                    SumData = sumData,
                    CheckBeforeSaveItems = listCheckBeforeSave
                };

                return new GetCloseDayResponse(result);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new GetCloseDayResponse($"An error occurred when get document postday : {ex.Message}");
            }
        }

        private async Task<ModelMecMaxCardResult> getMecMaxCard(ModelMecMaxCardParam param, string pStrUrl)
        {
            if (param == null)
            {
                return null;
            }
            pStrUrl = (pStrUrl ?? string.Empty).Trim();
            if (0.Equals(pStrUrl.Length))
            {
                return null;
            }
            //param.branch_id = $"52{param.branch_id}";
            ModelMecMaxCardResult result = null;
            result = await DefaultService.CallPostApi<ModelMecMaxCardParam, ModelMecMaxCardResult>(pStrUrl, param);
            return result;
        }

        private async Task saveErrorMecCardLog(
        ModelMecMaxCardParam pMecCard,
        GetDocumentRequest pDocRequest,
        String pStrUrl,
        Exception pException
        )
        {
            LogError log = new LogError();
            if (pException == null)
            {
                log.Message = "Call Api Mec Error";
            }
            else
            {
                log.StackTrace = pException.StackTrace;
                while (pException.InnerException != null)
                {
                    pException = pException.InnerException;
                }
                log.Message = pException.Message;
            }

            log.BrnCode = pDocRequest.BrnCode;
            log.CompCode = pDocRequest.CompCode;
            log.LocCode = pDocRequest.LocCode;
            log.LogStatus = "API Error";
            log.JsonData = JsonConvert.SerializeObject(pMecCard);
            log.Path = pStrUrl;
            //log.Message = "Call Api Mec Error";
            log.CreatedDate = DateTime.Now;
            log.Method = "POST";
            await context.AddAsync(log);
            await context.SaveChangesAsync();
        }


        private List<ModelMecPostPaidValidate> getListPosPaidValidate(ModelMecMaxCardResult pMecMaxCard, List<SalCreditsaleHd> pListCreditSale)
        {
            List<ModelMecPostPaidValidate> result = new List<ModelMecPostPaidValidate>();
            foreach (var posPaid in pMecMaxCard.RESULT.POSTPAID)
            {
                if (posPaid == null)
                {
                    continue;
                }
                var mecPosPaidValidate = new ModelMecPostPaidValidate();
                string strCusCode = (posPaid.cusno ?? string.Empty).Trim();
                mecPosPaidValidate.MecCuscode = strCusCode;
                mecPosPaidValidate.MecTotalBath = posPaid.total_baht;
                result.Add(mecPosPaidValidate);
                if (pListCreditSale == null || 0.Equals(pListCreditSale.Count))
                {
                    continue;
                }
                foreach (var creditsale in pListCreditSale)
                {
                    if (creditsale == null)
                    {
                        continue;
                    }
                    if (strCusCode.Equals(creditsale.CustCode))
                    {
                        mecPosPaidValidate.MaxCusCode = strCusCode;
                        mecPosPaidValidate.MaxCreditSaleSubAmt = creditsale.SubAmt;
                        mecPosPaidValidate.IsValid
                            = mecPosPaidValidate.MaxCreditSaleSubAmt
                            == mecPosPaidValidate.MecTotalBath;
                    }
                }
            }
            return result;
        }

        public async Task<DopPostdayHd> GetDopPostDayHd(GetDocumentRequest req, DateTime docDate)
        {
            var dopPostdayHd = await context.DopPostdayHds
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.DocDate == docDate).AsNoTracking().FirstOrDefaultAsync();
            return dopPostdayHd;
        }

        public async Task<List<DopPostdayDt>> GetDopPostDayDt(GetDocumentRequest req, DateTime docDate)
        {
            var dopPostdayDts = await context.DopPostdayDts
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                    x.DocDate == docDate).AsNoTracking().ToListAsync();
            return dopPostdayDts;

        }

        public async Task<List<Formula>> GetFormula(GetDocumentRequest req, DateTime docDate)
        {
            var listResultFormula = new List<Formula>();
           
            ModelMecMaxCardResult mecMaxCard = null;
            Func<string, ModelMecMaxCardParam, Task> funcCallApiMaxCard = async (url, mecParam) =>
            {
                url = (url ?? string.Empty).Trim();
                try
                {
                    mecMaxCard = await getMecMaxCard(mecParam, url);
                    if (mecMaxCard == null)
                    {
                        mecMaxCard = new ModelMecMaxCardResult();
                        throw new Exception("Cannot Connect MecMaxCard API");
                    }
                    mecMaxCard.CODE = (mecMaxCard.CODE ?? string.Empty).Trim();
                    if (!"200".Equals(mecMaxCard.CODE))
                    {
                        throw new Exception("MexMaxCard Api Error With Code : " + mecMaxCard.CODE);
                    }
                }
                catch (Exception ex)
                {
                    await saveErrorMecCardLog(
                        pMecCard: mecParam,
                        pDocRequest: req,
                        pStrUrl: url,
                        pException: ex
                    );
                }
            };
            string strMecBrachId = string.Empty;
            strMecBrachId = (req?.BrnCode ?? string.Empty).Trim();
            var _sourcenameCredit = "POSCredit";
            string strMecDate = string.Empty;
            strMecDate = req.SystemDate;
            var masBranchConfig =
                    await context.MasBranchConfigs.FirstOrDefaultAsync(x =>
                        x.CompCode == req.CompCode && x.BrnCode == req.BrnCode);
            var masCompany = await context.MasCompanies.FirstOrDefaultAsync(x => x.CompCode == req.CompCode);
            var masProductPriceCardStock = await context.MasProductPrices
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.PdId == "08896" && x.UnitBarcode == "08896").AsNoTracking().FirstOrDefaultAsync();
            var listFmData = await context.DopPostdaySums
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.DocDate == docDate).AsNoTracking().ToListAsync();
            if (listFmData.Count > 0)
            {
                listResultFormula = _mapper.Map<List<DopPostdaySum>, List<Formula>>(listFmData);
                for (int i = 0; i < listResultFormula.Count; i++)
                {
                    listResultFormula[i].Unit = listFmData[i].UnitName;
                }
            }
            else
            {
                var listFm = await (from brn in context.DopFormulaBranches
                                    join sql in context.DopFormulas on brn.FmNo equals sql.FmNo
                                    where brn.CompCode == req.CompCode
                                          && brn.BrnCode == req.BrnCode
                                    select sql).AsNoTracking().ToListAsync();

                if (masBranchConfig != null)
                {
                    if (masBranchConfig.IsLockSlip != "Y" || masBranchConfig.IsPos != "Y")
                    {
                        var posCreditApi = listFm.FirstOrDefault(x => x.SourceName == _sourcenameCredit);
                        listFm.Remove(posCreditApi);
                    }
                }

                var urlCardStock = listFm.Where(x => x.FmName == "Maxplus").Select(x => x.SourceValue)
                    .FirstOrDefault();
                var cardStock = await GetCardStock(urlCardStock, masCompany.CompSname, req.BrnCode, docDate);
                var urlMaxCard = listFm.FirstOrDefault(x => x.FmName == "MEC");
                //var maxCardMec = await GetMaxCard(urlMaxCard?.SourceValue, urlMaxCard?.SourceKey, req.BrnCode, docDate);

                var listResultFm = new List<Formula>();

                Func<DopFormula, Task<ModelMecMaxCardResult>> funcGetMaxCard = async fm =>
                {
                    if (mecMaxCard == null)
                    {
                        var mecParam = new ModelMecMaxCardParam();
                        mecParam.branch_id = $"52{strMecBrachId}";
                        mecParam.date = strMecDate;
                        mecParam.apikey = fm.SourceKey ?? string.Empty;
                        await funcCallApiMaxCard(fm.SourceValue, mecParam);
                    }
                    return mecMaxCard;
                };
                foreach (var fm in listFm)
                {
                    var resultFm = new Formula()
                    {
                        FmNo = fm.FmNo,
                        Remark = fm.Remark,
                        Unit = fm.UnitName
                    };

                    if (fm.SourceType == "SQL")
                    {
                        var sourceQuery = fm.SourceValue
                            .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                            .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                            .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                            .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");

                        resultFm.SourceAmount = GetValueFromQuery(sourceQuery, context);
                    }
                    else if (fm.SourceType == "API")
                    {
                        if (fm.SourceName == "MaxPosSum")
                        {
                            var maxPosSum = await GetMaxPosSum(fm.SourceValue, fm.SourceKey, docDate, req.BrnCode);
                            resultFm.SourceAmount = Math.Round(Convert.ToDecimal(maxPosSum), 2);
                        }
                        else if (fm.SourceName == "PCCA")
                        {
                            var pccaValue = await GetPCCAService(fm.SourceValue, req.CompCode, req.BrnCode,
                                req.DocDate);
                            resultFm.SourceAmount = pccaValue;
                        }
                        else if (fm.SourceName == _sourcenameCredit)
                        {
                            var posCreditValue = await GetPOSCreditAmount(fm.SourceValue, req.BrnCode, docDate);
                            resultFm.SourceAmount = posCreditValue;
                        }
                        else if (fm.FmName == "Maxplus" && cardStock != null)
                        {
                            switch (fm.SourceName)
                            {
                                case "MaxPlusTouchPointBath":
                                    if (masProductPriceCardStock != null)
                                    {
                                        resultFm.SourceAmount = (decimal)(cardStock.TouchPoint * masProductPriceCardStock.Unitprice - cardStock.DiscountBuy);  //DiscountBuyPrice
                                    }
                                    break;
                                case "MaxPlusTouchPointPiece":
                                    resultFm.SourceAmount = cardStock.TouchPoint;
                                    break;
                                case "MaxPlusWithdraw":
                                    resultFm.SourceAmount = cardStock.Online + cardStock.CardLost;
                                    break;
                                default:
                                    resultFm.SourceAmount = cardStock.ExtendCardPrice - cardStock.DiscountExtend; //DiscountExtendPrice
                                    break;
                            }
                        }
                        else if (fm.FmName == "MEC" && fm.SourceName == "Prepaid")
                        {
                            mecMaxCard = await funcGetMaxCard(fm);
                            resultFm.SourceAmount = mecMaxCard?.RESULT?.PREPAID?.total_baht ?? 0;

                        }
                        else if (fm.FmName == "MEC" && fm.SourceName == "Withdraw")
                        {
                            mecMaxCard = await funcGetMaxCard(fm);
                            resultFm.SourceAmount = (decimal)(mecMaxCard?.RESULT?.USECOMP?.total_lite ?? 0);
                        }
                        else
                        {
                            var sourceValue = await GetValueFromApi(fm.SourceValue, fm.SourceKey,
                                masCompany.CompSname, req.BrnCode, req.DocDate);
                            resultFm.SourceAmount = sourceValue;
                        }
                    }

                    if (fm.DestinationType == "SQL")
                    {
                        var destinationQuery = fm.DestinationValue
                            .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                            .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                            .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                            .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");

                        resultFm.DestinationAmount = GetValueFromQuery(destinationQuery, context);
                    }

                    listResultFm.Add(resultFm);
                }

                listResultFormula = listResultFm;
            }

            return listResultFormula;
        }

        public async Task<SumInDay> GetSumDate(GetDocumentRequest req, DateTime docDate)
        {
            var payType = new List<string>()
                {
                    "2",
                    "เช็ค"
                };
            var listCashAmt = new List<decimal>();
            var listDiffAmt = new List<decimal>();
            var meterData = await context.DopPeriodCashSums
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate)
                    .AsNoTracking().ToListAsync();
            meterData.ForEach(x =>
            {
                listCashAmt.Add(x.CashAmt.Value);
                listDiffAmt.Add(x.DiffAmt.Value);
            });

            var receiveData = await (from hd in context.FinReceiveHds
                                     join dt in context.FinReceiveDts on hd.DocNo equals dt.DocNo
                                     where hd.CompCode == req.CompCode && hd.BrnCode == req.BrnCode && hd.LocCode == req.LocCode &&
                                           hd.DocDate == docDate && payType.Contains(hd.PayType)
                                     select dt.ItemAmt.Value).ToListAsync();

            var sumData = new SumInDay()
            {
                SumCashAmt = listCashAmt.Sum(),
                SumDiffAmt = listDiffAmt.Sum(),
                SumCashDepositAmt = listCashAmt.Sum() + listDiffAmt.Sum(),
                SumChequeAmt = receiveData.Sum()
            };

            return sumData;
        }
        public async Task<CheckBeforeSaveView> CheckBeforeSave(GetDocumentRequest req, DateTime docDate)
        {
            var response = new CheckBeforeSaveView();
            var listCheckBeforeSave = new List<CheckBeforeSave>();
            List<ModelMecPostPaidValidate> listPosPaidValidate = null;
            var checkCoperate = "Yes";
            string strMecBrachId = string.Empty;
            strMecBrachId = (req?.BrnCode ?? string.Empty).Trim();
            string strMecDate = string.Empty;
            strMecDate = req.SystemDate;
            var dopValidates = await context.DopValidates.Where(x => x.ValidStatus == "Active").ToListAsync();
            ModelMecMaxCardResult mecMaxCard = null;
            List<SalCreditsaleHd> listGroupCreditSale = null;

            Func<string, ModelMecMaxCardParam, Task> funcCallApiMaxCard = async (url, mecParam) =>
            {
                url = (url ?? string.Empty).Trim();
                try
                {
                    mecMaxCard = await getMecMaxCard(mecParam, url);
                    if (mecMaxCard == null)
                    {
                        mecMaxCard = new ModelMecMaxCardResult();
                        throw new Exception("Cannot Connect MecMaxCard API");
                    }
                    mecMaxCard.CODE = (mecMaxCard.CODE ?? string.Empty).Trim();
                    if (!"200".Equals(mecMaxCard.CODE))
                    {
                        throw new Exception("MexMaxCard Api Error With Code : " + mecMaxCard.CODE);
                    }
                }
                catch (Exception ex)
                {
                    await saveErrorMecCardLog(
                        pMecCard: mecParam,
                        pDocRequest: req,
                        pStrUrl: url,
                        pException: ex
                    );
                }
            };
            Func<string, string, Task<bool>> funcBranchIsPos = async (compCode, brnCode) =>
            {
                //eturn await this.context.MasBranchConfigs.FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode);
                var masBranchConfig = await this.context.MasBranchConfigs.FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode);

                if (masBranchConfig != null && masBranchConfig.IsPos == "Y" && masBranchConfig.IsLockMeter == "Y")
                {
                    return true; // ต้องตรวจสอบ
                }
                else
                {
                    return false; // ไม่ต้องตรวจสอบ ข้ามข้อนี้ไปเลย
                }
            };
            Func<Task<List<SalCreditsaleHd>>> funcGetListCreditSale = async () =>
            {
                if (listGroupCreditSale == null)
                {
                    listGroupCreditSale = await context.SalCreditsaleHds.AsNoTracking()
                    .Where(
                        x => x.CompCode == req.CompCode &&
                        x.BrnCode == req.BrnCode &&
                        x.DocDate == docDate &&
                        x.DocStatus != "Cancel" &&
                        x.DocType == "CreditSale"
                    ).GroupBy(c => c.CustCode)
                    .Select(cd => new SalCreditsaleHd
                    {
                        CustCode = cd.Key,
                        SubAmt = cd.Sum(x => x.SubAmt)
                    }).ToListAsync();
                }
                return listGroupCreditSale;
            };

            var isPos = await funcBranchIsPos(req.CompCode, req.BrnCode);

            foreach (var item in dopValidates)
            {
                if (item.SourceType == "SOAP")
                {
                    if (item.SourceKey == "Corporate")
                    {
                        var customerSoaps = await GetValueFronSoap(req.BrnCode, docDate, item.SourceValue);

                        if (customerSoaps != null && customerSoaps.Count() > 0)
                        {
                            var creditsales = await funcGetListCreditSale();
                            foreach (var customersoap in customerSoaps)
                            {
                                var creditsale = creditsales.FirstOrDefault(x =>
                                    x.CustCode.Trim() == customersoap.legcustcode.Trim());

                                if (creditsale == null)
                                {
                                    checkCoperate = "No";
                                    break;
                                }
                                else
                                {
                                    if (customersoap.amt != creditsale.SubAmt)
                                    {
                                        checkCoperate = "No";
                                        break;
                                    }
                                }
                            }
                        }

                        listCheckBeforeSave.Add(new CheckBeforeSave()
                        {
                            Label = item.Remark,
                            PassValue = checkCoperate,
                            ValidNo = item.ValidNo,
                            HaveValidSql = false,
                        });
                    }
                }
                else if (item.SourceType == "API" && item.ValidName == "MEC")
                {

                    var brnMEC = await (from f in this.context.DopFormulas
                                        join fm in this.context.DopFormulaBranches on f.FmNo equals fm.FmNo
                                        where f.FmName == "MEC" && f.SourceName == "Prepaid"
                                        select fm
                                  ).Select(x => x.BrnCode).ToListAsync();
                    if (brnMEC != null && brnMEC.Contains(req.BrnCode))
                    {

                        bool isValidPostPaid = true;
                        List<SalCreditsaleHd> creditsales = null;
                        Func<Task<bool>> funcValidateMaxCard = async () =>
                        {
                            if (mecMaxCard == null)
                            {
                                var mecParam = new ModelMecMaxCardParam();
                                mecParam.branch_id = $"52{strMecBrachId}";
                                mecParam.date = strMecDate;
                                mecParam.apikey = (item.SourceKey ?? string.Empty).Trim();
                                await funcCallApiMaxCard(item.SourceValue, mecParam);
                            }
                            var result = mecMaxCard != null
                                && mecMaxCard.RESULT != null
                                && mecMaxCard.RESULT.POSTPAID != null
                                && mecMaxCard.RESULT.POSTPAID.Count > 0;
                            return result;
                        };
                        if (await funcValidateMaxCard())
                        {
                            creditsales = await funcGetListCreditSale();
                            listPosPaidValidate = getListPosPaidValidate(mecMaxCard, creditsales);
                            if (listPosPaidValidate != null && listPosPaidValidate.Count > 0)
                            {
                                isValidPostPaid = listPosPaidValidate.All(x => x.IsValid);
                            }
                        }
                        listCheckBeforeSave.Add(new CheckBeforeSave()
                        {
                            Label = item.Remark,
                            PassValue = isValidPostPaid ? "Yes" : "No",
                            ValidNo = item.ValidNo,
                            HaveValidSql = false,
                        });

                    }

                }
                else if (item.SourceType == "API" && item.ValidName == "DailOperation" && item.SourceName == "GetPeriodCount")
                {
                    //var isPos = GetIsPosFromMasBranchConfig(req.CompCode, req.BrnCode);
                    if (isPos)
                    {
                        var passValue = await ComparePeriod(req.CompCode, req.BrnCode, docDate, item.SourceValue);
                        listCheckBeforeSave.Add(new CheckBeforeSave()
                        {
                            Label = item.Remark,
                            PassValue = passValue,
                            ValidNo = item.ValidNo,
                            HaveValidSql = !string.IsNullOrWhiteSpace(item.ValidSql),
                        });
                    }
                }
                else if (item.SourceType == "API" && item.ValidName == "DailOperation" && item.SourceName == "CheckPeriodWater")
                {
                    if (isPos)
                    {
                        var passValue = "No";
                        var result = await CheckPeriodWater(req.CompCode, req.BrnCode, req.LocCode, docDate, item.SourceValue);
                        if (result)
                        {
                            passValue = "Yes";
                        }
                     
                        listCheckBeforeSave.Add(new CheckBeforeSave()
                        {
                            Label = item.Remark,
                            PassValue = passValue,
                            ValidNo = item.ValidNo,
                            HaveValidSql = !string.IsNullOrWhiteSpace(item.ValidSql),
                        });
                        //var passValue = await ComparePeriod(req.CompCode, req.BrnCode, docDate, item.SourceValue);
                        //listCheckBeforeSave.Add(new CheckBeforeSave()
                        //{
                        //    Label = item.Remark,
                        //    PassValue = passValue,
                        //    ValidNo = item.ValidNo,
                        //    HaveValidSql = !string.IsNullOrWhiteSpace(item.ValidSql),
                        //});
                    }
                }
                else // sql
                {
                    var query = item.SourceValue
                        .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                        .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                        .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                        .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");
                    listCheckBeforeSave.Add(new CheckBeforeSave()
                    {
                        Label = item.Remark,
                        PassValue = GetValueFromQueryValidate(query, context),
                        ValidNo = item.ValidNo,
                        HaveValidSql = !string.IsNullOrWhiteSpace(item.ValidSql),
                    });
                }
            }

            response.checkBeforeSaves = listCheckBeforeSave;
            response.mecPostPaidValidates = listPosPaidValidate;
            return response;
        }


        public async Task<GetCloseDayResponse> GetDocument(GetDocumentRequest req)
        {
            try
            {
                ModelMecMaxCardResult mecMaxCard = null;
                Func<string, ModelMecMaxCardParam, Task> funcCallApiMaxCard = async (url, mecParam) =>
                {
                    url = (url ?? string.Empty).Trim();
                    try
                    {
                        mecMaxCard = await getMecMaxCard(mecParam, url);
                        if (mecMaxCard == null)
                        {
                            mecMaxCard = new ModelMecMaxCardResult();
                            throw new Exception("Cannot Connect MecMaxCard API");
                        }
                        mecMaxCard.CODE = (mecMaxCard.CODE ?? string.Empty).Trim();
                        if (!"200".Equals(mecMaxCard.CODE))
                        {
                            throw new Exception("MexMaxCard Api Error With Code : " + mecMaxCard.CODE);
                        }
                    }
                    catch (Exception ex)
                    {
                        await saveErrorMecCardLog(
                            pMecCard: mecParam,
                            pDocRequest: req,
                            pStrUrl: url,
                            pException: ex
                        );
                    }
                };



                string strMecBrachId = string.Empty;
                //strMecBrachId = "100261";
                strMecBrachId = (req?.BrnCode ?? string.Empty).Trim();
                string strMecDate = string.Empty;
                //strMecDate = "2023-02-22";
                strMecDate = req.SystemDate;
                var checkCoperate = "Yes";
                var _sourcenameCredit = "POSCredit";
                var masCompany = await context.MasCompanies.FirstOrDefaultAsync(x => x.CompCode == req.CompCode);
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                List<ModelMecPostPaidValidate> listPosPaidValidate = null;
                var hdData = await context.DopPostdayHds
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.DocDate == docDate).AsNoTracking().FirstOrDefaultAsync();

                var listDtData = await context.DopPostdayDts
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.DocDate == docDate).AsNoTracking().ToListAsync();
                var listCr = listDtData.Where(x => x.DocType == "CR").ToList();
                var listDr = listDtData.Where(x => x.DocType == "DR").ToList();

                var listFmData = await context.DopPostdaySums
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.DocDate == docDate).AsNoTracking().ToListAsync();
                var masCustomers =
                    context.MasCustomers.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode);
                var masBranchConfig =
                    await context.MasBranchConfigs.FirstOrDefaultAsync(x =>
                        x.CompCode == req.CompCode && x.BrnCode == req.BrnCode);
                var masProductPriceCardStock = await context.MasProductPrices
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.LocCode == req.LocCode &&
                                x.PdId == "08896" && x.UnitBarcode == "08896").AsNoTracking().FirstOrDefaultAsync();
                var listResultFormula = new List<Formula>();

                if (listFmData.Count > 0)
                {
                    listResultFormula = _mapper.Map<List<DopPostdaySum>, List<Formula>>(listFmData);
                    for (int i = 0; i < listResultFormula.Count; i++)
                    {
                        listResultFormula[i].Unit = listFmData[i].UnitName;
                    }
                }
                else
                {
                    var listFm = await (from brn in context.DopFormulaBranches
                                        join sql in context.DopFormulas on brn.FmNo equals sql.FmNo
                                        where brn.CompCode == req.CompCode
                                              && brn.BrnCode == req.BrnCode
                                        select sql).AsNoTracking().ToListAsync();

                    if (masBranchConfig != null)
                    {
                        if (masBranchConfig.IsLockSlip != "Y" || masBranchConfig.IsPos != "Y")
                        {
                            var posCreditApi = listFm.FirstOrDefault(x => x.SourceName == _sourcenameCredit);
                            listFm.Remove(posCreditApi);
                        }
                    }

                    var urlCardStock = listFm.Where(x => x.FmName == "Maxplus").Select(x => x.SourceValue)
                        .FirstOrDefault();
                    var cardStock = await GetCardStock(urlCardStock, masCompany.CompSname, req.BrnCode, docDate);
                    var urlMaxCard = listFm.FirstOrDefault(x => x.FmName == "MEC");
                    //var maxCardMec = await GetMaxCard(urlMaxCard?.SourceValue, urlMaxCard?.SourceKey, req.BrnCode, docDate);

                    var listResultFm = new List<Formula>();

                    Func<DopFormula, Task<ModelMecMaxCardResult>> funcGetMaxCard = async fm =>
                    {
                        if (mecMaxCard == null)
                        {
                            var mecParam = new ModelMecMaxCardParam();
                            mecParam.branch_id = $"52{strMecBrachId}";
                            mecParam.date = strMecDate;
                            mecParam.apikey = fm.SourceKey ?? string.Empty;
                            await funcCallApiMaxCard(fm.SourceValue, mecParam);
                        }
                        return mecMaxCard;
                    };
                    foreach (var fm in listFm)
                    {
                        var resultFm = new Formula()
                        {
                            FmNo = fm.FmNo,
                            Remark = fm.Remark,
                            Unit = fm.UnitName
                        };

                        if (fm.SourceType == "SQL")
                        {
                            var sourceQuery = fm.SourceValue
                                .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                                .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                                .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                                .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");

                            resultFm.SourceAmount = GetValueFromQuery(sourceQuery, context);
                        }
                        else if (fm.SourceType == "API")
                        {
                            if (fm.SourceName == "MaxPosSum")
                            {
                                var maxPosSum = await GetMaxPosSum(fm.SourceValue, fm.SourceKey, docDate, req.BrnCode);
                                resultFm.SourceAmount = Math.Round(Convert.ToDecimal(maxPosSum), 2);
                            }
                            else if (fm.SourceName == "PCCA")
                            {
                                var pccaValue = await GetPCCAService(fm.SourceValue, req.CompCode, req.BrnCode,
                                    req.DocDate);
                                resultFm.SourceAmount = pccaValue;
                            }
                            else if (fm.SourceName == _sourcenameCredit)
                            {
                                var posCreditValue = await GetPOSCreditAmount(fm.SourceValue, req.BrnCode, docDate);
                                resultFm.SourceAmount = posCreditValue;
                            }
                            else if (fm.FmName == "Maxplus" && cardStock != null)
                            {
                                switch (fm.SourceName)
                                {
                                    case "MaxPlusTouchPointBath":
                                        if (masProductPriceCardStock != null)
                                        {
                                            resultFm.SourceAmount = (decimal)(cardStock.TouchPoint * masProductPriceCardStock.Unitprice - cardStock.DiscountBuy);  //DiscountBuyPrice
                                        }
                                        break;
                                    case "MaxPlusTouchPointPiece":
                                        resultFm.SourceAmount = cardStock.TouchPoint;
                                        break;
                                    case "MaxPlusWithdraw":
                                        resultFm.SourceAmount = cardStock.Online + cardStock.CardLost;
                                        break;
                                    default:
                                        resultFm.SourceAmount = cardStock.ExtendCardPrice - cardStock.DiscountExtend;// DiscountExtendPrice
                                        break;
                                }
                            }
                            else if (fm.FmName == "MEC" && fm.SourceName == "Prepaid")
                            {
                                mecMaxCard = await funcGetMaxCard(fm);
                                resultFm.SourceAmount = mecMaxCard?.RESULT?.PREPAID?.total_baht ?? 0;

                            }
                            else if (fm.FmName == "MEC" && fm.SourceName == "Withdraw")
                            {
                                mecMaxCard = await funcGetMaxCard(fm);
                                resultFm.SourceAmount = (decimal)(mecMaxCard?.RESULT?.USECOMP?.total_lite ?? 0);
                            }
                            else
                            {
                                var sourceValue = await GetValueFromApi(fm.SourceValue, fm.SourceKey,
                                    masCompany.CompSname, req.BrnCode, req.DocDate);
                                resultFm.SourceAmount = sourceValue;
                            }
                        }

                        if (fm.DestinationType == "SQL")
                        {
                            var destinationQuery = fm.DestinationValue
                                .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                                .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                                .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                                .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");

                            resultFm.DestinationAmount = GetValueFromQuery(destinationQuery, context);
                        }

                        listResultFm.Add(resultFm);
                    }

                    listResultFormula = listResultFm;
                }

                var listCashAmt = new List<decimal>();
                var listDiffAmt = new List<decimal>();

                var meterData = await context.DopPeriodCashSums
                    .Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate)
                    .AsNoTracking().ToListAsync();
                meterData.ForEach(x =>
                {
                    listCashAmt.Add(x.CashAmt.Value);
                    listDiffAmt.Add(x.DiffAmt.Value);
                });

                var payType = new List<string>()
                {
                    "2",
                    "เช็ค"
                };

                var receiveData = await (from hd in context.FinReceiveHds
                                         join dt in context.FinReceiveDts on hd.DocNo equals dt.DocNo
                                         where hd.CompCode == req.CompCode && hd.BrnCode == req.BrnCode && hd.LocCode == req.LocCode &&
                                               hd.DocDate == docDate && payType.Contains(hd.PayType)
                                         select dt.ItemAmt.Value).ToListAsync();

                var sumData = new SumInDay()
                {
                    SumCashAmt = listCashAmt.Sum(),
                    SumDiffAmt = listDiffAmt.Sum(),
                    SumCashDepositAmt = listCashAmt.Sum() + listDiffAmt.Sum(),
                    SumChequeAmt = receiveData.Sum()
                };

                //การตรวจสอบด้านขวา
                var listCheckBeforeSave = new List<CheckBeforeSave>();
                var dopValidates = await context.DopValidates.Where(x => x.ValidStatus == "Active").ToListAsync();
                //var validateData = dopValidates.Where(x => x.SourceType == "SQL");
                //var validateCoperate = dopValidates.FirstOrDefault(x => x.SourceType == "SOAP");
                List<SalCreditsaleHd> listGroupCreditSale = null;
                Func<Task<List<SalCreditsaleHd>>> funcGetListCreditSale = async () =>
                {
                    if (listGroupCreditSale == null)
                    {
                        listGroupCreditSale = await context.SalCreditsaleHds.AsNoTracking()
                        .Where(
                            x => x.CompCode == req.CompCode &&
                            x.BrnCode == req.BrnCode &&
                            x.DocDate == docDate &&
                            x.DocStatus != "Cancel" &&
                            x.DocType == "CreditSale"
                        ).GroupBy(c => c.CustCode)
                        .Select(cd => new SalCreditsaleHd
                        {
                            CustCode = cd.Key,
                            SubAmt = cd.Sum(x => x.SubAmt)
                        }).ToListAsync();
                    }
                    return listGroupCreditSale;
                };
                foreach (var item in dopValidates)
                {
                    if (item.SourceType == "SOAP")
                    {
                        if (item.SourceKey == "Corporate")
                        {
                            var customerSoaps = await GetValueFronSoap(req.BrnCode, docDate, item.SourceValue);
                            //var creditsales = context.SalCreditsaleHds.Where(x => x.CompCode == req.CompCode &&
                            //        x.BrnCode == req.BrnCode &&
                            //        x.DocDate == docDate && x.DocStatus != "Cancel" && x.DocType == "CreditSale")
                            //    .GroupBy(c => c.CustCode)
                            //    .Select(cd => new SalCreditsaleHd
                            //    {
                            //        CustCode = cd.Key,
                            //        SubAmt = cd.Sum(x => x.SubAmt)
                            //    })
                            //    .ToList();

                            if (customerSoaps != null && customerSoaps.Count() > 0)
                            {
                                var creditsales = await funcGetListCreditSale();
                                foreach (var customersoap in customerSoaps)
                                {
                                    var creditsale = creditsales.FirstOrDefault(x =>
                                        x.CustCode.Trim() == customersoap.legcustcode.Trim());

                                    if (creditsale == null)
                                    {
                                        checkCoperate = "No";
                                        break;
                                    }
                                    else
                                    {
                                        if (customersoap.amt != creditsale.SubAmt)
                                        {
                                            checkCoperate = "No";
                                            break;
                                        }
                                    }
                                }
                            }

                            listCheckBeforeSave.Add(new CheckBeforeSave()
                            {
                                Label = item.Remark,
                                PassValue = checkCoperate,
                                ValidNo = item.ValidNo,
                                HaveValidSql = false,
                            });
                        }
                    }
                    else if (item.SourceType == "API" && item.ValidName == "MEC")
                    {

                        var brnMEC = await (from f in this.context.DopFormulas
                                            join fm in this.context.DopFormulaBranches on f.FmNo equals fm.FmNo
                                            where f.FmName == "MEC" && f.SourceName == "Prepaid"
                                            select fm
                                      ).Select(x => x.BrnCode).ToListAsync();
                        if (brnMEC != null && brnMEC.Contains(req.BrnCode))
                        {

                            bool isValidPostPaid = true;
                            List<SalCreditsaleHd> creditsales = null;
                            Func<Task<bool>> funcValidateMaxCard = async () =>
                            {
                                if (mecMaxCard == null)
                                {
                                    var mecParam = new ModelMecMaxCardParam();
                                    mecParam.branch_id = $"52{strMecBrachId}";
                                    mecParam.date = strMecDate;
                                    mecParam.apikey = (item.SourceKey ?? string.Empty).Trim();
                                    await funcCallApiMaxCard(item.SourceValue, mecParam);
                                }
                                var result = mecMaxCard != null
                                    && mecMaxCard.RESULT != null
                                    && mecMaxCard.RESULT.POSTPAID != null
                                    && mecMaxCard.RESULT.POSTPAID.Count > 0;
                                return result;
                            };
                            if (await funcValidateMaxCard())
                            {
                                creditsales = await funcGetListCreditSale();
                                listPosPaidValidate = getListPosPaidValidate(mecMaxCard, creditsales);
                                if (listPosPaidValidate != null && listPosPaidValidate.Count > 0)
                                {
                                    isValidPostPaid = listPosPaidValidate.All(x => x.IsValid);
                                }
                            }
                            listCheckBeforeSave.Add(new CheckBeforeSave()
                            {
                                Label = item.Remark,
                                PassValue = isValidPostPaid ? "Yes" : "No",
                                ValidNo = item.ValidNo,
                                HaveValidSql = false,
                            });

                        }

                    }
                    else if (item.SourceType == "API" && item.SourceKey == "GetPeriodCount")
                    {
                        var isPos = GetIsPosFromMasBranchConfig(req.CompCode, req.BrnCode);

                        if (isPos)
                        {
                            var passValue = await ComparePeriod(req.CompCode, req.BrnCode, docDate, item.SourceValue);
                            listCheckBeforeSave.Add(new CheckBeforeSave()
                            {
                                Label = item.Remark,
                                PassValue = passValue,
                                ValidNo = item.ValidNo,
                                HaveValidSql = !string.IsNullOrWhiteSpace(item.ValidSql),
                            });
                        }
                    }
                    else // sql
                    {
                        var query = item.SourceValue
                            .Replace("@COMP_CODE", $"'{req.CompCode}'").Replace("@comp_code", $"'{req.CompCode}'")
                            .Replace("@BRN_CODE", $"'{req.BrnCode}'").Replace("@brn_code", $"'{req.BrnCode}'")
                            .Replace("@LOC_CODE", $"'{req.LocCode}'").Replace("@loc_code", $"'{req.LocCode}'")
                            .Replace("@DOC_DATE", $"'{req.DocDate}'").Replace("@doc_date", $"'{req.DocDate}'");
                        listCheckBeforeSave.Add(new CheckBeforeSave()
                        {
                            Label = item.Remark,
                            PassValue = GetValueFromQueryValidate(query, context),
                            ValidNo = item.ValidNo,
                            HaveValidSql = !string.IsNullOrWhiteSpace(item.ValidSql),
                        });
                    }
                }


                //check all meter data is active
                //var listMeterData = await context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                //var checkAllTransIsActive = listMeterData.Where(x => x.PeriodStatus != "Active").ToList();
                //if (listMeterData.Count == 0 || checkAllTransIsActive.Count > 0)
                //{
                //    listCheckBeforeSave.Add(new CheckBeforeSave() { Label = "บันทึกมิเตอร์/วัดถัง/รับจ่าย", PassValue = "No" });
                //}
                //else 
                //{
                //    listCheckBeforeSave.Add(new CheckBeforeSave() { Label = "บันทึกมิเตอร์/วัดถัง/รับจ่าย", PassValue = "Yes" });
                //}

                var result = new GetDocumentResponse()
                {
                    DopPostdayHd = hdData,
                    CrItems = listCr,
                    DrItems = listDr,
                    FormulaItems = listResultFormula,
                    SumData = sumData,
                    CheckBeforeSaveItems = listCheckBeforeSave,
                    ListValidatePostPaid = listPosPaidValidate,
                };

                return new GetCloseDayResponse(result);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new GetCloseDayResponse($"An error occurred when get document postday : {ex.Message}");
            }
        }

        private async Task<ResultModel> GetCardStock(string url, string compSname, string brnCode, DateTime docDate)
        {
            if (string.IsNullOrEmpty(url)) return new ResultModel();
            try
            {
                var tomorrow = docDate.Date.AddDays(1);
                var startTime = new DateTime(docDate.Year, docDate.Month, docDate.Day, 05, 00, 00, 000);
                var endTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 04, 59, 59, 999);

                var request = new CardStockRequest
                {
                    StockDateFrom = startTime,
                    StockDateTo = endTime,
                    OriShopCode = brnCode,
                    CompanyCode = compSname
                };

                //GenerateJwt
                var jwt = _crmService.GenerateJwt();

                //Encryption
                var requestToString = JsonConvert.SerializeObject(request);
                var encryptText = _crmService.Encryption(requestToString);

                var response = await _crmService.SendApiAsync(jwt, url, encryptText);

                if (response.IsSuccessStatusCode)
                {
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var content = await reader.ReadToEndAsync();
                        var decryption = _crmService.Decryption(content);
                        var responseBody = JsonConvert.DeserializeObject<CardStockResponse>(decryption);

                        return responseBody.ResultModel.FirstOrDefault() ?? new ResultModel();
                    }
                }

                throw new Exception($"Call api failed with url : {url}, Status Code : {response.StatusCode} and Reason : {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Call method GetCardStock failed with error message : {ex}");
            }
        }

        private async Task<ResultMaxCard> GetMaxCard(string url, string apiKey, string brnCode, DateTime docDate)
        {
            if (string.IsNullOrEmpty(url)) return new ResultMaxCard();
            try
            {
                string dateString = docDate.ToString("yyyy-MM-dd");

                var request = new MaxCardRequest
                {
                    date = dateString,
                    branch_id = brnCode,
                    apikey = apiKey
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(url);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                    "application/json");
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    response.Content.ReadAsStringAsync().Wait();
                    var jsonString = response.Content.ReadAsStringAsync().Result;

                    var responseData = JsonConvert.DeserializeObject<MaxCardResponse>(jsonString);

                    return responseData.RESULT ?? new ResultMaxCard();
                }

                throw new Exception($"Call api failed with url : {url}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Call method GetMaxCard failed with error message : {ex}");
            }
        }

        private decimal GetValueFromQuery(string query, DbContext dbContext)
        {
            var cnn = dbContext.Database.GetDbConnection();
            var cmm = cnn.CreateCommand();
            cmm.Connection = cnn;
            cmm.CommandText = query;
            cnn.Open();
            var reader = cmm.ExecuteScalar();
            var result = reader != DBNull.Value ? Convert.ToDecimal(reader) : 0;
            cnn.Close();

            return result;
        }

        public async Task<string> GetMaxPosSum(string url, string token, DateTime docDate, string shopId)
        {
            try
            {
                var tomorrow = docDate.Date.AddDays(1);
                var startTime = new DateTime(docDate.Year, docDate.Month, docDate.Day, 05, 00, 00, 000);
                var endTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 04, 59, 59, 999);

                var request = new MaxPosSumRequest()
                {
                    token = token,
                    starttime = startTime,
                    endtime = endTime,
                    shopid = shopId
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(url);
                //client.BaseAddress = new Uri("https://api.uat.maxcard.tech/api/inquiry/CS/api/maxpossum");
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                    "application/json");
                var response = await client.PostAsync(url, content);
                //var response = await client.PostAsync("https://api.uat.maxcard.tech/api/inquiry/CS/api/maxpossum", content);

                if (response.IsSuccessStatusCode)
                {
                    response.Content.ReadAsStringAsync().Wait();
                    var jsonString = response.Content.ReadAsStringAsync().Result;

                    var responseData = JsonConvert.DeserializeObject<MaxPosSumResponse>(jsonString);

                    return responseData.summary;
                }
                else
                {
                    throw new Exception($"Call api failed with url : {url}");
                }
            }
            catch
            {
                return "0";
            }
        }

        private async Task<decimal> GetPCCAService(string url, string compCode, string brnCode, string docDate)
        {
            var request = new PCCARequest()
            {
                CODCOMP_HEAD = compCode,
                CENTER_CODE = brnCode,
                H_TS = docDate
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(url);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
                var jsonString = response.Content.ReadAsStringAsync().Result;

                var responseData = JsonConvert.DeserializeObject<PCCA>(jsonString);

                return responseData.AMOUNT;
            }
            else
            {
                throw new Exception($"Call api failed with url : {url}");
            }
        }

        private async Task<decimal> GetPOSCreditAmount(string url, string brnCode, DateTime docDate)
        {
            //url = "https://prod-maxstation-dailyoperation-web1-asv.azurewebsites.net/api/Pos/GetCreditSummaryByBranch";
            //docDate = DateTime.Now;
            var token = await this._tokenService.GenerateTokenAsync();

            var request = new POSCreditRequest()
            {
                BrnCode = brnCode,
                FromDate = docDate
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(url);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var jsonDeserializeObject = JsonConvert.DeserializeObject(jsonString).ToString();
                var jsonObject = JObject.Parse(jsonDeserializeObject);
                var responseData = jsonObject.ToObject<PostCreditResponse>();

                return responseData.Data.Amount;
            }
            else
            {
                throw new Exception($"Call api failed with url : {url}");
            }
        }

        private async Task<decimal> GetValueFromApi(string url, string reqtKey, string compSname, string brnCode,
            string docDate)
        {
            var req = new RequestFormula()
            {
                ReqtKey = reqtKey,
                CompanyCode = compSname,
                ShopCode = brnCode,
                SystemDate = docDate
            };


            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(url);
            string strJsonReq = JsonConvert.SerializeObject(req);
            var content = new StringContent(strJsonReq, Encoding.UTF8, "application/json");
            //var content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
                var jsonString = response.Content.ReadAsStringAsync().Result;

                var responseData = JsonConvert.DeserializeObject<ResponseFormula>(jsonString);

                return responseData.Detail.Discount;
            }
            else
            {
                //response.re
                //throw new Exception($"Call api failed with url : {url}");
                var result = new ResponseFormula();
                result.Detail = new ResponseFormulaDetail();

                LogError log = new LogError();
                log.LogStatus = "API Error";
                log.BrnCode = brnCode;
                log.CompCode = compSname;
                log.CreatedDate = DateTime.Now;
                log.JsonData = strJsonReq;
                log.LocCode = string.Empty;
                log.Message = (response?.ReasonPhrase ?? string.Empty).Trim();
                log.Method = "POST";
                log.Path = url;
                log.StackTrace = string.Empty;
                await context.AddAsync(log);
                await context.SaveChangesAsync();
                return result.Detail.Discount;
            }
        }

        private async Task saveLogErrorFromApi(string pStrContent)
        {
            LogError log = new LogError();
            log.LogStatus = "API Error";
            log.BrnCode = "";
            log.CompCode = "";
            log.CreatedDate = DateTime.Now;
            log.JsonData = "";
            log.LocCode = "";
            log.Message = "";
            log.Method = "POST";
            log.Path = "";
            log.StackTrace = "";
        }


        private async Task<List<CustomerAmount>> GetValueFronSoap(string brnCode, DateTime docDate, string sourceValue)
        {
            var response = new List<CustomerAmount>();
            try
            {
                var tomorrow = docDate.Date.AddDays(1);
                var startDate =
                    new DateTime(docDate.Year, docDate.Month, docDate.Day, 05, 00, 00, 000).ToString(
                        "yyyy-MM-dd HH:mm:ss.fff");
                var endDate =
                    new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 04, 59, 59, 999).ToString(
                        "yyyy-MM-dd HH:mm:ss.fff");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sourceValue); //New Link 16/10/2020
                request.Headers.Add(@"SOAPAction:http://tempuri.org/Web_station_GetAmt");
                request.ContentType = "text/xml;charset=\"utf-8\"";
                request.Accept = "text/xml";
                request.Method = "POST";
                string jsonText = "{\"W_brn\":{\"brn\":\"" + brnCode + "\"},\"S_date\":{\"strdate\":\"" + startDate +
                                  "\",\"enddate\":\"" + endDate + "\"  }}";

                ServicePointManager.ServerCertificateValidationCallback = delegate (object sender,
                    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

                XmlDocument SOAPReqBody = new XmlDocument();
                SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-   instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">  
             <soap:Body>  
                <Web_station_GetAmt  xmlns=""http://tempuri.org/"">  
                   <strJson>" + jsonText + @"</strJson>
                </Web_station_GetAmt>  
              </soap:Body>  
            </soap:Envelope>");

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    SOAPReqBody.Save(stream);
                }

                using (WebResponse Serviceres = await request.GetResponseAsync())
                {
                    using StreamReader rd = new StreamReader(Serviceres.GetResponseStream());
                    string serviceResult = rd.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(serviceResult);

                    XmlNodeList elemList = doc.GetElementsByTagName("Web_station_GetAmtResult");
                    string jsonString = elemList[0].InnerXml;
                    JObject jObjectData = JObject.Parse(jsonString);
                    string tagData = jObjectData["Data"].ToString();
                    JObject jObjectDetail = JObject.Parse(tagData);
                    string tagDetail = jObjectDetail["Detail"].ToString();
                    response = JsonConvert.DeserializeObject<List<CustomerAmount>>(tagDetail);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("ไม่สามารถเชื่อมต่อกับระบบ Coperate ได้");
            }
        }

        private bool GetIsPosFromMasBranchConfig(string compCode, string brnCode)
        {
            try
            {
                var masBranchConfig =
                    context.MasBranchConfigs.FirstOrDefault(x => x.CompCode == compCode && x.BrnCode == brnCode);

                if (masBranchConfig != null && masBranchConfig.IsPos == "Y" && masBranchConfig.IsLockMeter == "Y")
                {
                    return true; // ต้องตรวจสอบ
                }
                else
                {
                    return false; // ไม่ต้องตรวจสอบ ข้ามข้อนี้ไปเลย
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<string> ComparePeriod(string compCode, string brnCode, DateTime docDate, string sourceValue)
        {
            try
            {
                var token = await this._tokenService.GenerateTokenAsync();

                var result = string.Empty;
                var maxPeriod = context.DopPeriods
                    .Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.DocDate == docDate)
                    .Select(d => d.PeriodNo).Distinct()
                    .Count();


                var request = new POSPeriodCountRequest()
                {
                    BrnCode = brnCode,
                    FromDate = docDate
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.BaseAddress = new Uri(sourceValue);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                    "application/json");
                var response = await client.PostAsync(sourceValue, content);

                if (response.IsSuccessStatusCode)
                {
                    response.Content.ReadAsStringAsync().Wait();
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var jsonDeserializeObject = JsonConvert.DeserializeObject(jsonString).ToString();
                    var jsonObject = JObject.Parse(jsonDeserializeObject);
                    var responseData = jsonObject.ToObject<POSPeriodCountResponse>();
                    var posPeriod = responseData.Data.CountPeriod;

                    if (maxPeriod == posPeriod)
                    {
                        result = "Yes";
                    }
                    else
                    {
                        result = "No";
                    }
                }
                else
                {
                    result = "No";
                    //throw new Exception($"Call api failed with url : {sourceValue}");
                }

                return result;
            }
            catch (Exception ex)
            {
                return "No";
                //throw new Exception($"An error occurred when compare period : {ex.Message}");
            }
        }

        private async Task<bool> CheckPeriodWater(string compCode, string brnCode,string locCode, DateTime docDate,string urlRequest)
        {
            try
            {
                var token = await _tokenService.GenerateTokenAsync();
               // var result = string.Empty;

                var request = new POSPeriodWaterRequest()
                {
                    CompCode = compCode,
                    BrnCode = brnCode,
                    LocCode = locCode,
                    FromDate = docDate
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.BaseAddress = new Uri(urlRequest);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                    "application/json");
                var response = await client.PostAsync(urlRequest, content);

                if (response.IsSuccessStatusCode)
                {
                    response.Content.ReadAsStringAsync().Wait();
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<bool>(jsonString);
                    return result;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
               //throw new Exception($"An error occurred when compare period : {ex.Message}");
            }
        }

        public async Task AddDopPostdayAsync(SaveDocumentRequest request)
        {
            try
            {
                var docDate = DateTime.ParseExact(request.DopPostdayHd.DocDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture);
                var oldHdData = await context.DopPostdayHds
                    .Where(x => x.CompCode == request.DopPostdayHd.CompCode &&
                                x.BrnCode == request.DopPostdayHd.BrnCode &&
                                x.LocCode == request.DopPostdayHd.LocCode && x.DocDate == docDate).AsNoTracking()
                    .FirstOrDefaultAsync();
                var listOldDtData = await context.DopPostdayDts
                    .Where(x => x.CompCode == request.DopPostdayHd.CompCode &&
                                x.BrnCode == request.DopPostdayHd.BrnCode &&
                                x.LocCode == request.DopPostdayHd.LocCode && x.DocDate == docDate).AsNoTracking()
                    .ToListAsync();
                var listOldPostDaySumData = await context.DopPostdaySums
                    .Where(x => x.CompCode == request.DopPostdayHd.CompCode &&
                                x.BrnCode == request.DopPostdayHd.BrnCode &&
                                x.LocCode == request.DopPostdayHd.LocCode && x.DocDate == docDate).AsNoTracking()
                    .ToListAsync();
                var listOldPostDayValidate = await context.DopPostdayValidates.Where(x =>
                    x.CompCode == request.DopPostdayHd.CompCode && x.BrnCode == request.DopPostdayHd.BrnCode &&
                    x.LocCode == request.DopPostdayHd.LocCode && x.DocDate == docDate).AsNoTracking().ToListAsync();
                var newHdData = _mapper.Map<CtDopPostdayHd, DopPostdayHd>(request.DopPostdayHd);
                newHdData.DocDate = docDate;
                newHdData.CreatedDate = DateTime.Now;

                var joinListDt = request.CrItems.Concat(request.DrItems).ToList();
                joinListDt.ForEach(x => { x.DocDate = docDate; });

                var listInsertDopPostdaySum = _mapper.Map<List<Formula>, List<DopPostdaySum>>(request.FormulaItems);
                var seq = 1;
                listInsertDopPostdaySum.ForEach(x =>
                {
                    x.CompCode = request.DopPostdayHd.CompCode;
                    x.BrnCode = request.DopPostdayHd.BrnCode;
                    x.LocCode = request.DopPostdayHd.LocCode;
                    x.DocDate = docDate;
                    x.SeqNo = seq++;
                    x.UnitName = request.FormulaItems.FirstOrDefault(y => x.FmNo == y.FmNo)?.Unit ?? string.Empty;
                });


                if (oldHdData != null)
                {
                    context.DopPostdayHds.Remove(oldHdData);
                }

                context.DopPostdayDts.RemoveRange(listOldDtData);
                context.DopPostdaySums.RemoveRange(listOldPostDaySumData);
                context.DopPostdayValidates.RemoveRange(listOldPostDayValidate);

                await context.DopPostdayHds.AddAsync(newHdData);
                await context.DopPostdayDts.AddRangeAsync(joinListDt);
                await context.DopPostdaySums.AddRangeAsync(listInsertDopPostdaySum);

                var dopPostdayValidates = request.CheckBeforeSaveItems.Select((x, i) => new DopPostdayValidate()
                {
                    BrnCode = request.DopPostdayHd.BrnCode,
                    CompCode = request.DopPostdayHd.CompCode,
                    LocCode = request.DopPostdayHd.LocCode,
                    DocDate = docDate,
                    SeqNo = i + 1,
                    ValidRemark = x.Label,
                    ValidResult = x.PassValue
                }).ToList();

                await context.DopPostdayValidates.AddRangeAsync(dopPostdayValidates);


            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the AddDopPostdayAsync: {strMessage}");
            }
        }

        private string GetValueFromQueryValidate(string query, DbContext dbContext)
        {
            var cnn = dbContext.Database.GetDbConnection();
            var cmm = cnn.CreateCommand();
            cmm.Connection = cnn;
            cmm.CommandText = query;
            cnn.Open();
            var reader = cmm.ExecuteScalar();
            var result = reader != DBNull.Value ? Convert.ToString(reader) : "";
            cnn.Close();

            return result;
        }

        public async Task AddDopPostdayValidateAsync(List<DopPostdayValidate> dopPostdayValidates)
        {
            try
            {
                await context.DopPostdayValidates.AddRangeAsync(dopPostdayValidates);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the AddDopPostdayValidateAsync: {strMessage}");
            }
        }

        public async Task UpdatePeriodAsync(PostDayResource query)
        {
            try
            {
                var dopPeriod = await context.DopPeriods
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                dopPeriod.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(dopPeriod);
                //context.SaveChanges();
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdatePeriodAsync: {strMessage}");
            }
        }

        public async Task UpdateRequestAsync(PostDayResource query)
        {
            try
            {
                var invRequestHds = await context.InvRequestHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invRequestHds.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invRequestHds);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateRequestAsync: {strMessage}");
            }
        }

        public async Task UpdateReceiveAsync(PostDayResource query)
        {
            try
            {
                var invReceiveProd = await context.InvReceiveProdHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invReceiveProd.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invReceiveProd);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateReceiveAsync: {strMessage}");
            }
        }

        public async Task UpdateTranferOutAsync(PostDayResource query)
        {
            try
            {
                var invTransOut = await context.InvTranoutHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invTransOut.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invTransOut);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateTranferOut: {strMessage}");
            }
        }

        public async Task UpdateTranferInAsync(PostDayResource query)
        {
            try
            {
                var invTransIn = await context.InvTraninHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invTransIn.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invTransIn);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateTranferInAsync: {strMessage}");
            }
        }

        public async Task UpdateWithdrawAsync(PostDayResource query)
        {
            try
            {
                var invWithdraw = await context.InvWithdrawHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invWithdraw.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invWithdraw);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateWithdrawAsync: {strMessage}");
            }
        }

        public async Task UpdateReturnSupAsync(PostDayResource query)
        {
            try
            {
                var invReturnSup = await context.InvReturnSupHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invReturnSup.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invReturnSup);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateReturnSupAsync: {strMessage}");
            }
        }

        public async Task UpdateAuditAsync(PostDayResource query)
        {
            try
            {
                var invAudit = await context.InvAuditHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invAudit.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invAudit);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateAuditAsync: {strMessage}");
            }
        }

        public async Task UpdateAdjustAsync(PostDayResource query)
        {
            try
            {
                var invAdjust = await context.InvAdjustHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invAdjust.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invAdjust);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateAdjustAsync: {strMessage}");
            }
        }

        public async Task UpdateReturnOilAsync(PostDayResource query)
        {
            try
            {
                var invReturnOil = await context.InvReturnOilHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invReturnOil.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invReturnOil);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateReturnOilAsync: {strMessage}");
            }
        }

        public async Task UpdateUnusableAsync(PostDayResource query)
        {
            try
            {
                var invUnusable = await context.InvUnuseHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invUnusable.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invUnusable);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateUnusableAsync: {strMessage}");
            }
        }

        public async Task UpdateQuotationAsync(PostDayResource query)
        {
            try
            {
                var salQuotationHds = await context.SalQuotationHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                salQuotationHds.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(salQuotationHds);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateQuotationAsync: {strMessage}");
            }
        }

        public async Task UpdateCashSaleAsync(PostDayResource query)
        {
            try
            {
                var salCashHds = await context.SalCashsaleHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                salCashHds.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(salCashHds);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateCashSaleAsync: {strMessage}");
            }
        }

        public async Task UpdateCreditSaleAsync(PostDayResource query)
        {
            try
            {
                var salCreditHds = await context.SalCreditsaleHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                salCreditHds.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(salCreditHds);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateCreditSaleAsync: {strMessage}");
            }
        }

        public async Task UpdateTaxinvoiceAsync(PostDayResource query)
        {
            try
            {
                var salTaxinvoiceHds = await context.SalTaxinvoiceHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate <= query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                salTaxinvoiceHds.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(salTaxinvoiceHds);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateTaxinvoiceAsync: {strMessage}");
            }
        }

        public async Task UpdateSalCndnAsync(PostDayResource query)
        {
            try
            {
                var salCndnHd = await context.SalCndnHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                salCndnHd.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(salCndnHd);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateSalCndnAsync: {strMessage}");
            }
        }

        public async Task UpdateBillingAsync(PostDayResource query)
        {
            try
            {
                var salBilling = await context.SalBillingHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                salBilling.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(salBilling);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateBillingAsync: {strMessage}");
            }
        }

        public async Task UpdateFinanceReceiveAsync(PostDayResource query)
        {
            try
            {
                var finReceive = await context.FinReceiveHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                finReceive.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(finReceive);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateFinanceReceiveAsync: {strMessage}");
            }
        }

        public async Task UpdateBranchConfigAsync(PostDayResource query)
        {
            try
            {
                var branchConfig = await context.MasBranchConfigs
                    .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.IsPos == "Y")
                    .FirstOrDefaultAsync();

                if (branchConfig != null)
                {
                    var maxseq = this.context.SysBranchConfigs
                        .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode)
                        .OrderBy(x => x.DocDate).ThenBy(x => x.SeqNo).LastOrDefault();
                    if (maxseq != null)
                    {
                        var ismeter = this.context.SysBranchConfigs.FirstOrDefault(x =>
                            x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == maxseq.DocDate &&
                            x.SeqNo == maxseq.SeqNo && x.ConfigId == "IS_LOCK_METER");
                        if (ismeter != null)
                        {
                            branchConfig.IsLockMeter =
                                (ismeter.EndDate <= query.WDate) ? "Y" : branchConfig.IsLockMeter;
                        }
                        else
                        {
                            branchConfig.IsLockMeter = "Y";
                        }

                        var isslip = this.context.SysBranchConfigs.FirstOrDefault(x =>
                            x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == maxseq.DocDate &&
                            x.SeqNo == maxseq.SeqNo && x.ConfigId == "IS_LOCK_SLIP");
                        if (isslip != null)
                        {
                            branchConfig.IsLockSlip = (isslip.EndDate <= query.WDate) ? "Y" : branchConfig.IsLockSlip;
                        }
                        else
                        {
                            branchConfig.IsLockSlip = "Y";
                        }
                    }
                    else
                    {
                        branchConfig.IsLockMeter = "Y"; //initial
                        branchConfig.IsLockSlip = "Y"; //initial
                    }

                    branchConfig.UpdatedBy = query.User;
                    branchConfig.UpdatedDate = DateTime.Now;
                    context.Update(branchConfig);
                }
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateBranchConfigAsync: {strMessage}");
            }
        }

        public async Task UpdateMasControlAsync(PostDayResource query)
        {
            try
            {
                var icControl = await context.MasControls.Where(x => x.CompCode == query.CompCode
                                                                     && x.BrnCode == query.BrnCode
                                                                     && x.CtrlCode == "WDATE").ToListAsync();

                if (icControl == null || icControl.Count() == 0)
                {
                    MasControl ctl = new MasControl
                    {
                        CompCode = query.CompCode,
                        BrnCode = query.BrnCode,
                        LocCode = "000",
                        CtrlCode = "WDATE",
                        CtrlValue = query.WDate.Value.AddDays(1).ToString("dd/MM/yyyy"),
                        Remark = "Work Date",
                        CreatedBy = query.User,
                        CreatedDate = DateTime.Now
                    };
                    context.Add(ctl);
                }
                else
                {
                    icControl.ForEach(x =>
                    {
                        x.CtrlValue = query.WDate.Value.AddDays(1).ToString("dd/MM/yyyy"); //ขยับวันทำงานไป 1 วัน
                        x.UpdatedDate = DateTime.Now;
                        x.UpdatedBy = query.User;
                    });
                    context.UpdateRange(icControl);
                }

                //context.SaveChanges();
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the UpdateMasControlAsync: {strMessage}");
            }
        }

        public async Task CreateCreditSaleAsync(PostDayResource query, string pattern, int runno)
        {
            try
            {
                SalTaxinvoiceHd taxinvoicehd = new SalTaxinvoiceHd();

                #region CreditSale N:1

                //เช็คการรวบบิลตามลูกค้า
                //หาก่อนว่า วันนี้มีลูกค้ากี่ราย
                var creditsales = await context.SalCreditsaleHds.Where(x => x.CompCode == query.CompCode
                                                                            && x.BrnCode == query.BrnCode
                                                                            && x.Post == "P"
                                                                            && x.DocStatus != "Cancel"
                                                                            && x.DocType == "CreditSale"
                                                                            && (x.TxNo == "" || x.TxNo == null))
                    .GroupBy(x => new
                    { x.CompCode, x.BrnCode, x.LocCode, x.DocType, x.DocDate, x.CustCode, x.CustName })
                    .Select(x => new SalCreditsaleHd
                    {
                        CompCode = x.Key.CompCode,
                        BrnCode = x.Key.BrnCode,
                        LocCode = x.Key.LocCode,
                        DocType = x.Key.DocType,
                        DocDate = x.Key.DocDate,
                        CustCode = x.Key.CustCode,
                        CustName = x.Key.CustName,
                    }).ToListAsync();

                foreach (var creditsalehd in creditsales) //list creditsale group by cust
                {
                    var cust = this.context.MasCustomers.Find(creditsalehd.CustCode);
                    if (cust.BillType == "Y")
                    {
                        //รวบบิล => เอา item ทั้งหมด ออกมาแล้วรวมกัน
                        var customer = this.context.MasCustomers.Find(cust.CustCode);

                        List<string> docnos = this.context.SalCreditsaleHds.Where(x => x.DocStatus != "Cancel" &&
                            x.Post == "P"
                            && (x.TxNo == null || x.TxNo == "")
                            && x.CompCode == creditsalehd.CompCode
                            && x.BrnCode == creditsalehd.BrnCode
                            && x.LocCode == creditsalehd.LocCode
                            && x.DocType == creditsalehd.DocType
                            && x.DocDate == creditsalehd.DocDate
                            && x.CustCode == creditsalehd.CustCode
                        ).Select(x => x.DocNo).ToList();

                        var salehd = this.context.SalCreditsaleHds.Where(x => x.CompCode == creditsalehd.CompCode
                                                                              && x.BrnCode == creditsalehd.BrnCode
                                                                              && x.LocCode == creditsalehd.LocCode
                                                                              && x.DocType == creditsalehd.DocType
                                                                              && docnos.Contains(x.DocNo)
                            ).GroupBy(x => new
                            {
                                x.CompCode,
                                x.BrnCode,
                                x.LocCode,
                                x.DocType,
                                x.DocDate,
                                x.CustCode,
                                x.Currency,
                                x.CurRate
                            })
                            .Select(x => new SalCreditsaleHd
                            {
                                CompCode = x.Key.CompCode,
                                BrnCode = x.Key.BrnCode,
                                LocCode = x.Key.LocCode,
                                DocType = x.Key.DocType,
                                DocDate = x.Key.DocDate,
                                CustCode = x.Key.CustCode,
                                Currency = x.Key.Currency,
                                CurRate = x.Key.CurRate,
                                SubAmt = x.Sum(s => Math.Round((decimal)s.SubAmt, 2)),
                                SubAmtCur = x.Sum(s => Math.Round((decimal)s.SubAmtCur, 2)),
                                DiscAmt = x.Sum(s => s.DiscAmt),
                                DiscAmtCur = x.Sum(s => s.DiscAmtCur),
                            }).FirstOrDefault();


                        var items = await this.context.SalCreditsaleDts.Where(d => d.CompCode == creditsalehd.CompCode
                                && d.BrnCode == creditsalehd.BrnCode
                                && d.LocCode == creditsalehd.LocCode
                                && d.DocType == creditsalehd.DocType
                                && docnos.Contains(d.DocNo)
                            ).GroupBy(x => new
                            {
                                x.CompCode,
                                x.BrnCode,
                                x.LocCode,
                                x.DocType,
                                x.PdId,
                                x.PdName,
                                x.UnitId,
                                x.UnitName,
                                x.UnitBarcode,
                                x.IsFree,
                                x.UnitPrice,
                                x.UnitPriceCur,
                                x.VatType,
                                x.VatRate
                            })
                            .Select(x => new SalTaxinvoiceDt
                            {
                                CompCode = x.Key.CompCode,
                                BrnCode = x.Key.BrnCode,
                                LocCode = x.Key.LocCode,
                                PdId = x.Key.PdId,
                                PdName = x.Key.PdName,
                                UnitId = x.Key.UnitId,
                                UnitName = x.Key.UnitName,
                                UnitBarcode = x.Key.UnitBarcode,
                                IsFree = x.Key.IsFree,
                                UnitPrice = x.Key.UnitPrice,
                                UnitPriceCur = x.Key.UnitPriceCur,
                                VatType = x.Key.VatType,
                                VatRate = x.Key.VatRate,
                                ItemQty = x.Sum(s => s.ItemQty),
                                StockQty = x.Sum(s => s.StockQty),
                                SumItemAmt = x.Sum(s => s.SumItemAmt),
                                SumItemAmtCur = x.Sum(s => s.SumItemAmtCur),
                                DiscAmt = x.Sum(s => s.DiscAmt),
                                DiscAmtCur = x.Sum(s => s.DiscAmtCur),
                                DiscHdAmt = x.Sum(s => s.DiscHdAmt),
                                DiscHdAmtCur = x.Sum(s => s.DiscHdAmtCur),
                                TaxBaseAmt = x.Sum(s => s.TaxBaseAmt),
                                TaxBaseAmtCur = x.Sum(s => s.TaxBaseAmtCur),
                                VatAmt = x.Sum(s => s.VatAmt),
                                VatAmtCur = x.Sum(s => s.VatAmtCur),
                                TotalAmt = x.Sum(s => s.TotalAmt),
                                TotalAmtCur = x.Sum(s => s.TotalAmtCur),
                            }).ToListAsync();


                        taxinvoicehd.RunNumber = ++runno;
                        taxinvoicehd.CompCode = salehd.CompCode;
                        taxinvoicehd.BrnCode = salehd.BrnCode;
                        taxinvoicehd.LocCode = salehd.LocCode;
                        taxinvoicehd.DocDate = salehd.DocDate;
                        taxinvoicehd.DocNo = GenDocNo(query, pattern, (int)taxinvoicehd.RunNumber);
                        taxinvoicehd.DocStatus = "Active";
                        taxinvoicehd.DocType = "CreditSale";
                        taxinvoicehd.CustCode = salehd.CustCode;
                        taxinvoicehd.CustName = cust.CustName;
                        taxinvoicehd.CustAddr1 = cust.CustAddr1;
                        taxinvoicehd.CustAddr2 = cust.CustAddr2;
                        taxinvoicehd.CitizenId = cust.CitizenId ?? "";
                        taxinvoicehd.Currency = salehd.Currency;
                        taxinvoicehd.CurRate = salehd.CurRate;
                        taxinvoicehd.DiscRate = "";
                        taxinvoicehd.DiscAmt = Math.Round((decimal)salehd.DiscAmt, 2);
                        taxinvoicehd.DiscAmtCur = Math.Round((decimal)salehd.DiscAmtCur, 2);


                        int seqno = 0;
                        decimal? subamt = 0;
                        decimal? subamtcur = 0;

                        items.ForEach(x =>
                        {
                            x.SubAmt = x.SumItemAmt - x.DiscAmt;
                            x.SubAmtCur = x.SumItemAmtCur - x.DiscAmtCur;
                            subamt += x.SubAmt;
                            subamtcur += x.SubAmtCur;
                        });
                        taxinvoicehd.SubAmt = Math.Round((decimal)subamt, 2);
                        taxinvoicehd.SubAmtCur = Math.Round((decimal)subamtcur, 2);

                        foreach (var item in items)
                        {
                            item.DocNo = taxinvoicehd.DocNo;
                            item.SeqNo = ++seqno;

                            var licenseplates = this.context.SalCreditsaleDts.Where(d =>
                                d.CompCode == creditsalehd.CompCode
                                && d.BrnCode == creditsalehd.BrnCode
                                && d.LocCode == creditsalehd.LocCode
                                && d.DocType == creditsalehd.DocType
                                && d.PdId == item.PdId
                                && d.UnitBarcode == item.UnitBarcode
                                && docnos.Contains(d.DocNo)).Select(x => x.LicensePlate).Distinct().ToList();
                            item.LicensePlate = string.Join(",", licenseplates);
                            this.context.SalTaxinvoiceDts.Add(item);
                        } //items


                        taxinvoicehd.TotalAmt = taxinvoicehd.SubAmt - taxinvoicehd.DiscAmt; //totalamt;
                        taxinvoicehd.TotalAmtCur = taxinvoicehd.SubAmtCur - taxinvoicehd.DiscAmtCur; // totalamtcur;
                        taxinvoicehd.TaxBaseAmt = items.Sum(x => x.TaxBaseAmt);
                        taxinvoicehd.TaxBaseAmtCur = items.Sum(x => x.TaxBaseAmtCur);
                        taxinvoicehd.VatRate = 0;
                        taxinvoicehd.VatAmt = items.Sum(x => x.VatAmt);
                        taxinvoicehd.VatAmtCur = items.Sum(x => x.VatAmtCur);
                        taxinvoicehd.NetAmt = taxinvoicehd.TaxBaseAmt + taxinvoicehd.VatAmt;
                        taxinvoicehd.NetAmtCur = taxinvoicehd.TaxBaseAmtCur + taxinvoicehd.VatAmtCur;

                        taxinvoicehd.ItemCount = items.Count();
                        taxinvoicehd.Post = "P";
                        taxinvoicehd.DocPattern = pattern;
                        taxinvoicehd.Guid = Guid.NewGuid();
                        taxinvoicehd.CreatedBy = query.User;
                        taxinvoicehd.CreatedDate = DateTime.Now;
                        this.context.SalTaxinvoiceHds.Add(taxinvoicehd);

                        //insert balance
                        var balance = new FinBalance
                        {
                            CompCode = taxinvoicehd.CompCode,
                            BrnCode = taxinvoicehd.BrnCode,
                            LocCode = taxinvoicehd.LocCode,
                            DocType = taxinvoicehd.DocType,
                            DocNo = taxinvoicehd.DocNo,
                            DocDate = taxinvoicehd.DocDate,
                            CustCode = taxinvoicehd.CustCode,
                            Currency = taxinvoicehd.Currency,
                            NetAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2),
                            NetAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2),
                            BalanceAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2), //0,
                            BalanceAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2), //0,
                            CreatedBy = query.User,
                            CreatedDate = DateTime.Now
                        };
                        this.context.FinBalances.Add(balance);

                        // update creditsalehd
                        var sales = this.context.SalCreditsaleHds.Where(x => x.CompCode == creditsalehd.CompCode
                                                                             && x.BrnCode == creditsalehd.BrnCode
                                                                             && x.LocCode == creditsalehd.LocCode
                                                                             && x.DocType == creditsalehd.DocType
                                                                             && docnos.Contains(x.DocNo)
                        ).ToList();
                        sales.ForEach(x =>
                        {
                            x.DocStatus = "Reference";
                            x.TxNo = taxinvoicehd.DocNo;
                            x.UpdatedBy = query.User;
                            x.UpdatedDate = DateTime.Now;
                        });
                        this.context.SalCreditsaleHds.UpdateRange(sales);

                        //end รวบบิล  
                    }
                    else
                    {
                        //ไม่ต้องรวบบิล  => ดึงทุกใบของลูกค้านี้ขึ้นมา
                        var salehds = await this.context.SalCreditsaleHds.Where(x => x.DocStatus != "Cancel" &&
                            x.Post == "P"
                            && (x.TxNo == null || x.TxNo == "")
                            && x.CompCode == creditsalehd.CompCode
                            && x.BrnCode == creditsalehd.BrnCode
                            && x.LocCode == creditsalehd.LocCode
                            && x.DocType == creditsalehd.DocType
                            && x.DocDate == creditsalehd.DocDate
                            && x.CustCode == creditsalehd.CustCode
                        ).OrderBy(x => x.DocNo).ToListAsync();

                        foreach (var salehd in salehds)
                        {
                            //insert taxinvoice hd
                            taxinvoicehd = new SalTaxinvoiceHd();
                            taxinvoicehd.RunNumber = ++runno;
                            taxinvoicehd.CompCode = salehd.CompCode;
                            taxinvoicehd.BrnCode = salehd.BrnCode;
                            taxinvoicehd.LocCode = salehd.LocCode;
                            taxinvoicehd.DocNo = GenDocNo(query, pattern, (int)taxinvoicehd.RunNumber);
                            taxinvoicehd.DocStatus = "Active";
                            taxinvoicehd.DocType = "CreditSale";
                            taxinvoicehd.DocDate = salehd.DocDate;
                            taxinvoicehd.CustCode = salehd.CustCode;
                            taxinvoicehd.CustName = salehd.CustName;
                            taxinvoicehd.CitizenId = cust.CitizenId;
                            taxinvoicehd.CustAddr1 = cust.CustAddr1; //salehd.CustAddr1;
                            taxinvoicehd.CustAddr2 = cust.CustAddr2; //salehd.CustAddr2;
                            taxinvoicehd.RefNo = salehd.RefNo;
                            taxinvoicehd.ItemCount = salehd.ItemCount;
                            taxinvoicehd.Currency = salehd.Currency;
                            taxinvoicehd.CurRate = salehd.CurRate;
                            taxinvoicehd.SubAmt = salehd.SubAmt;
                            taxinvoicehd.SubAmtCur = salehd.SubAmtCur;
                            taxinvoicehd.DiscRate = salehd.DiscRate;
                            taxinvoicehd.DiscAmt = salehd.DiscAmt;
                            taxinvoicehd.DiscAmtCur = salehd.DiscAmtCur;
                            taxinvoicehd.TotalAmt = Math.Round((decimal)salehd.TotalAmt, 2);
                            taxinvoicehd.TotalAmtCur = Math.Round((decimal)salehd.TotalAmtCur, 2);
                            taxinvoicehd.TaxBaseAmt = Math.Round((decimal)salehd.TaxBaseAmt, 2);
                            taxinvoicehd.TaxBaseAmtCur = Math.Round((decimal)salehd.TaxBaseAmtCur, 2);
                            taxinvoicehd.VatRate = salehd.VatRate;
                            taxinvoicehd.VatAmt = Math.Round((decimal)salehd.VatAmt, 2);
                            taxinvoicehd.VatAmtCur = Math.Round((decimal)salehd.VatAmtCur, 2);
                            taxinvoicehd.NetAmt = Math.Round((decimal)salehd.NetAmt, 2);
                            taxinvoicehd.NetAmtCur = Math.Round((decimal)salehd.NetAmtCur, 2);
                            taxinvoicehd.Post = "P";
                            taxinvoicehd.DocPattern = pattern;


                            //insert taxinvoice dt
                            var items = this.context.SalCreditsaleDts.Where(x => x.CompCode == salehd.CompCode
                                    && x.BrnCode == salehd.BrnCode
                                    && x.LocCode == salehd.LocCode
                                    && x.DocType == salehd.DocType
                                    && x.DocNo == salehd.DocNo)
                                .Select(x => new SalTaxinvoiceDt
                                {
                                    CompCode = x.CompCode,
                                    BrnCode = x.BrnCode,
                                    LocCode = x.LocCode,
                                    DocNo = taxinvoicehd.DocNo,
                                    SeqNo = x.SeqNo,
                                    LicensePlate = x.LicensePlate,
                                    PdId = x.PdId,
                                    PdName = x.PdName,
                                    IsFree = x.IsFree,
                                    UnitId = x.UnitId,
                                    UnitBarcode = x.UnitBarcode,
                                    UnitName = x.UnitName,
                                    ItemQty = x.ItemQty ?? decimal.Zero,
                                    StockQty = x.StockQty ?? decimal.Zero,
                                    UnitPrice = x.UnitPrice,
                                    UnitPriceCur = x.UnitPriceCur,
                                    SumItemAmt = x.SumItemAmt,
                                    SumItemAmtCur = x.SumItemAmtCur,
                                    DiscAmt = x.DiscAmt,
                                    DiscAmtCur = x.DiscAmtCur,
                                    DiscHdAmt = Math.Round((decimal)x.DiscHdAmt, 2),
                                    DiscHdAmtCur = Math.Round((decimal)x.DiscHdAmtCur, 2),
                                    SubAmt = x.SubAmt,
                                    SubAmtCur = x.SubAmtCur,
                                    VatType = x.VatType,
                                    VatRate = x.VatRate,
                                    VatAmt = Math.Round((decimal)x.VatAmt, 2),
                                    VatAmtCur = Math.Round((decimal)x.VatAmtCur, 2),
                                    TaxBaseAmt = Math.Round((decimal)x.TaxBaseAmt, 2),
                                    TaxBaseAmtCur = Math.Round((decimal)x.TaxBaseAmtCur, 2),
                                    TotalAmt = Math.Round((decimal)x.TotalAmt, 2),
                                    TotalAmtCur = Math.Round((decimal)x.TotalAmtCur, 2)
                                }).ToList();

                            this.context.SalTaxinvoiceDts.AddRange(items);

                            taxinvoicehd.ItemCount = items.Count();
                            taxinvoicehd.Guid = Guid.NewGuid();
                            taxinvoicehd.CreatedBy = query.User;
                            taxinvoicehd.CreatedDate = DateTime.Now;

                            this.context.SalTaxinvoiceHds.Add(taxinvoicehd);

                            //insert balance
                            var balance = new FinBalance
                            {
                                CompCode = taxinvoicehd.CompCode,
                                BrnCode = taxinvoicehd.BrnCode,
                                LocCode = taxinvoicehd.LocCode,
                                DocType = taxinvoicehd.DocType,
                                DocNo = taxinvoicehd.DocNo,
                                DocDate = taxinvoicehd.DocDate,
                                CustCode = taxinvoicehd.CustCode,
                                Currency = taxinvoicehd.Currency,
                                NetAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2),
                                NetAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2),
                                BalanceAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2), // 0,
                                BalanceAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2), // 0,
                                CreatedBy = query.User,
                                CreatedDate = DateTime.Now
                            };
                            this.context.FinBalances.Add(balance);


                            // update creditsalehd

                            salehd.DocStatus = "Reference";
                            salehd.TxNo = taxinvoicehd.DocNo; // reference tax_doc_no
                            salehd.UpdatedBy = query.User;
                            salehd.UpdatedDate = DateTime.Now;
                            this.context.SalCreditsaleHds.Update(salehd);
                        } //foreach salehd
                    } //BillType
                } //foreach

                #endregion

                #region Invoice 1:1

                //ไม่ต้องรวบเอกสาร
                var taxinvs = await (from cre in this.context.SalCreditsaleHds
                                     join cus in this.context.MasCustomers on cre.CustCode equals cus.CustCode
                                     where cre.CompCode == query.CompCode
                                           && cre.BrnCode == query.BrnCode
                                           && cre.Post == "P"
                                           && cre.DocType == "Invoice"
                                           && (cre.TxNo == "" || cre.TxNo == null)
                                     select new { cre, cus }).Select(x => new SalTaxinvoiceHd
                                     {
                                         CompCode = x.cre.CompCode,
                                         BrnCode = x.cre.BrnCode,
                                         LocCode = x.cre.LocCode,
                                         DocType = x.cre.DocType,
                                         DocNo = x.cre.DocNo,
                                         DocDate = x.cre.DocDate,
                                         CustCode = x.cre.CustCode,
                                         CustName = x.cre.CustName,
                                         CustAddr1 = x.cre.CustAddr1,
                                         CustAddr2 = x.cre.CustAddr2,
                                         CitizenId = x.cus.CitizenId,
                                         Currency = x.cre.Currency,
                                         CurRate = x.cre.CurRate,
                                         ItemCount = x.cre.ItemCount,
                                         SubAmt = x.cre.SubAmt,
                                         SubAmtCur = x.cre.SubAmtCur,
                                         DiscRate = x.cre.DiscRate,
                                         DiscAmt = x.cre.DiscAmt,
                                         DiscAmtCur = x.cre.DiscAmtCur,
                                         TotalAmt = x.cre.TotalAmt,
                                         TotalAmtCur = x.cre.TotalAmtCur,
                                         TaxBaseAmt = Math.Round((decimal)x.cre.TaxBaseAmt, 2),
                                         TaxBaseAmtCur = Math.Round((decimal)x.cre.TaxBaseAmtCur, 2),
                                         VatRate = x.cre.VatRate,
                                         VatAmt = Math.Round((decimal)x.cre.VatAmt, 2),
                                         VatAmtCur = Math.Round((decimal)x.cre.VatAmtCur, 2),
                                         NetAmt = Math.Round((decimal)x.cre.NetAmt, 2),
                                         NetAmtCur = Math.Round((decimal)x.cre.NetAmtCur, 2),
                                         Post = x.cre.Post
                                     }).ToListAsync();


                foreach (var taxinvhd in taxinvs)
                {
                    var invno = taxinvhd.DocNo;
                    taxinvhd.RunNumber = ++runno;
                    taxinvhd.DocNo = GenDocNo(query, pattern, (int)taxinvhd.RunNumber);
                    taxinvhd.DocStatus = "Active";
                    taxinvhd.DocPattern = pattern;

                    List<string> pdmeter = new List<string>() { "90575", "90581" };

                    var items = this.context.SalCreditsaleDts.Where(x => x.CompCode == taxinvhd.CompCode
                                                                         && x.BrnCode == taxinvhd.BrnCode
                                                                         && x.LocCode == taxinvhd.LocCode
                                                                         && x.DocType == taxinvhd.DocType
                                                                         && x.DocNo == invno)
                        .Select(x => new SalTaxinvoiceDt
                        {
                            CompCode = x.CompCode,
                            BrnCode = x.BrnCode,
                            LocCode = x.LocCode,
                            DocNo = taxinvhd.DocNo,
                            SeqNo = x.SeqNo,
                            LicensePlate = "",
                            PdId = x.PdId,
                            PdName = x.PdName + (pdmeter.Contains(x.PdId) ? $"({x.MeterStart} - {x.MeterFinish})" : ""),
                            IsFree = x.IsFree,
                            UnitId = x.UnitId,
                            UnitBarcode = x.UnitBarcode,
                            UnitName = x.UnitName,
                            ItemQty = x.ItemQty ?? decimal.Zero,
                            StockQty = x.StockQty ?? decimal.Zero,
                            UnitPrice = x.UnitPrice,
                            UnitPriceCur = x.UnitPriceCur,
                            SumItemAmt = x.SumItemAmt,
                            SumItemAmtCur = x.SumItemAmtCur,
                            DiscAmt = x.DiscAmt,
                            DiscAmtCur = x.DiscAmtCur,
                            DiscHdAmt = Math.Round((decimal)x.DiscHdAmt, 2),
                            DiscHdAmtCur = Math.Round((decimal)x.DiscHdAmtCur, 2),
                            SubAmt = x.SubAmt,
                            SubAmtCur = x.SubAmtCur,
                            VatType = x.VatType,
                            VatRate = x.VatRate,
                            VatAmt = Math.Round((decimal)x.VatAmt, 2),
                            VatAmtCur = Math.Round((decimal)x.VatAmtCur, 2),
                            TaxBaseAmt = Math.Round((decimal)x.TaxBaseAmt, 2),
                            TaxBaseAmtCur = Math.Round((decimal)x.TaxBaseAmtCur, 2),
                            TotalAmt = Math.Round((decimal)x.TotalAmt, 2),
                            TotalAmtCur = Math.Round((decimal)x.TotalAmtCur, 2)
                        }).ToList();

                    this.context.SalTaxinvoiceDts.AddRange(items);

                    taxinvhd.ItemCount = items.Count;
                    taxinvhd.CreatedBy = query.User;
                    taxinvhd.CreatedDate = DateTime.Now;
                    this.context.SalTaxinvoiceHds.Add(taxinvhd);

                    var balance = new FinBalance
                    {
                        CompCode = taxinvhd.CompCode,
                        BrnCode = taxinvhd.BrnCode,
                        LocCode = taxinvhd.LocCode,
                        DocType = taxinvhd.DocType,
                        DocNo = taxinvhd.DocNo,
                        DocDate = taxinvhd.DocDate,
                        CustCode = taxinvhd.CustCode,
                        Currency = taxinvhd.Currency,
                        NetAmt = Math.Round((decimal)taxinvhd.NetAmt, 2),
                        NetAmtCur = Math.Round((decimal)taxinvhd.NetAmtCur, 2),
                        BalanceAmt = Math.Round((decimal)taxinvhd.NetAmt, 2), //0,
                        BalanceAmtCur = Math.Round((decimal)taxinvhd.NetAmtCur, 2), //0,
                        CreatedBy = query.User,
                        CreatedDate = DateTime.Now
                    };
                    this.context.FinBalances.Add(balance);

                    //update salehd
                    var salehds = await this.context.SalCreditsaleHds.Where(x => (x.TxNo == null || x.TxNo == "")
                        && x.Post == taxinvhd.Post
                        && x.CompCode == taxinvhd.CompCode
                        && x.BrnCode == taxinvhd.BrnCode
                        && x.LocCode == taxinvhd.LocCode
                        && x.DocType == taxinvhd.DocType
                        && x.DocNo == invno
                    ).ToListAsync();
                    salehds.ForEach(x =>
                    {
                        x.DocStatus = "Reference";
                        x.TxNo = taxinvhd.DocNo;
                        x.UpdatedBy = query.User;
                        x.UpdatedDate = DateTime.Now;
                    });
                    this.context.SalCreditsaleHds.UpdateRange(salehds);
                } //end foreach

                #endregion
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the CreateCreditSaleAsync: {strMessage}");
            }
        }

        public async Task CreateTaxInvoiceAsync(PostDayResource query, string pattern, int runno)
        {
            try
            {
                SalTaxinvoiceHd taxinvoicehd = new SalTaxinvoiceHd();

                #region CreditSale N:1

                //เช็คการรวบบิลตามลูกค้า
                //หาก่อนว่า วันนี้มีลูกค้ากี่ราย
                var creditsales = await context.SalCreditsaleHds.Where(x => x.CompCode == query.CompCode
                                                                            && x.BrnCode == query.BrnCode
                                                                            && x.Post == "P"
                                                                            && x.DocStatus != "Cancel"
                                                                            && x.DocType == "CreditSale"
                                                                            && (x.TxNo == "" || x.TxNo == null))
                    .GroupBy(x => new
                    { x.CompCode, x.BrnCode, x.LocCode, x.DocType, x.DocDate, x.CustCode, x.CustName })
                    .Select(x => new SalCreditsaleHd
                    {
                        CompCode = x.Key.CompCode,
                        BrnCode = x.Key.BrnCode,
                        LocCode = x.Key.LocCode,
                        DocType = x.Key.DocType,
                        DocDate = x.Key.DocDate,
                        CustCode = x.Key.CustCode,
                        CustName = x.Key.CustName,
                    }).ToListAsync();

                foreach (var creditsalehd in creditsales) //list creditsale group by cust
                {
                    var cust = this.context.MasCustomers.Find(creditsalehd.CustCode);
                    if (cust.BillType == "Y")
                    {
                        //รวบบิล => เอา item ทั้งหมด ออกมาแล้วรวมกัน
                        var customer = this.context.MasCustomers.Find(cust.CustCode);

                        List<string> docnos = this.context.SalCreditsaleHds.Where(x => x.DocStatus != "Cancel" &&
                            x.Post == "P"
                            && (x.TxNo == null || x.TxNo == "")
                            && x.CompCode == creditsalehd.CompCode
                            && x.BrnCode == creditsalehd.BrnCode
                            && x.LocCode == creditsalehd.LocCode
                            && x.DocType == creditsalehd.DocType
                            && x.DocDate == creditsalehd.DocDate
                            && x.CustCode == creditsalehd.CustCode
                        ).Select(x => x.DocNo).ToList();

                        var salehd = this.context.SalCreditsaleHds.Where(x => x.CompCode == creditsalehd.CompCode
                                                                              && x.BrnCode == creditsalehd.BrnCode
                                                                              && x.LocCode == creditsalehd.LocCode
                                                                              && x.DocType == creditsalehd.DocType
                                                                              && docnos.Contains(x.DocNo)
                            ).GroupBy(x => new
                            {
                                x.CompCode,
                                x.BrnCode,
                                x.LocCode,
                                x.DocType,
                                x.DocDate,
                                x.CustCode,
                                x.Currency,
                                x.CurRate
                            })
                            .Select(x => new SalCreditsaleHd
                            {
                                CompCode = x.Key.CompCode,
                                BrnCode = x.Key.BrnCode,
                                LocCode = x.Key.LocCode,
                                DocType = x.Key.DocType,
                                DocDate = x.Key.DocDate,
                                CustCode = x.Key.CustCode,
                                Currency = x.Key.Currency,
                                CurRate = x.Key.CurRate,
                                SubAmt = x.Sum(s => Math.Round((decimal)s.SubAmt, 2)),
                                SubAmtCur = x.Sum(s => Math.Round((decimal)s.SubAmtCur, 2)),
                                DiscAmt = x.Sum(s => s.DiscAmt),
                                DiscAmtCur = x.Sum(s => s.DiscAmtCur),
                            }).FirstOrDefault();


                        var items = await this.context.SalCreditsaleDts.Where(d => d.CompCode == creditsalehd.CompCode
                                && d.BrnCode == creditsalehd.BrnCode
                                && d.LocCode == creditsalehd.LocCode
                                && d.DocType == creditsalehd.DocType
                                && docnos.Contains(d.DocNo)
                            ).GroupBy(x => new
                            {
                                x.CompCode,
                                x.BrnCode,
                                x.LocCode,
                                x.DocType,
                                x.PdId,
                                x.PdName,
                                x.UnitId,
                                x.UnitName,
                                x.UnitBarcode,
                                x.IsFree,
                                x.UnitPrice,
                                x.UnitPriceCur,
                                x.VatType,
                                x.VatRate
                            })
                            .Select(x => new SalTaxinvoiceDt
                            {
                                CompCode = x.Key.CompCode,
                                BrnCode = x.Key.BrnCode,
                                LocCode = x.Key.LocCode,
                                PdId = x.Key.PdId,
                                PdName = x.Key.PdName,
                                UnitId = x.Key.UnitId,
                                UnitName = x.Key.UnitName,
                                UnitBarcode = x.Key.UnitBarcode,
                                IsFree = x.Key.IsFree,
                                UnitPrice = x.Key.UnitPrice,
                                UnitPriceCur = x.Key.UnitPriceCur,
                                VatType = x.Key.VatType,
                                VatRate = x.Key.VatRate,
                                ItemQty = x.Sum(s => s.ItemQty),
                                StockQty = x.Sum(s => s.StockQty),
                                SumItemAmt = x.Sum(s => s.SumItemAmt),
                                SumItemAmtCur = x.Sum(s => s.SumItemAmtCur),
                                DiscAmt = x.Sum(s => s.DiscAmt),
                                DiscAmtCur = x.Sum(s => s.DiscAmtCur),
                                DiscHdAmt = x.Sum(s => s.DiscHdAmt),
                                DiscHdAmtCur = x.Sum(s => s.DiscHdAmtCur),
                                TaxBaseAmt = x.Sum(s => s.TaxBaseAmt),
                                TaxBaseAmtCur = x.Sum(s => s.TaxBaseAmtCur),
                                VatAmt = x.Sum(s => s.VatAmt),
                                VatAmtCur = x.Sum(s => s.VatAmtCur),
                                TotalAmt = x.Sum(s => s.TotalAmt),
                                TotalAmtCur = x.Sum(s => s.TotalAmtCur),
                            }).ToListAsync();


                        taxinvoicehd.RunNumber = ++runno;
                        taxinvoicehd.CompCode = salehd.CompCode;
                        taxinvoicehd.BrnCode = salehd.BrnCode;
                        taxinvoicehd.LocCode = salehd.LocCode;
                        taxinvoicehd.DocDate = salehd.DocDate;
                        taxinvoicehd.DocNo = GenDocNo(query, pattern, (int)taxinvoicehd.RunNumber);
                        taxinvoicehd.DocStatus = "Active";
                        taxinvoicehd.DocType = "CreditSale";
                        taxinvoicehd.CustCode = salehd.CustCode;
                        taxinvoicehd.CustName = $"{cust.CustPrefix} {cust.CustName}" ;
                        taxinvoicehd.CustAddr1 = cust.CustAddr1;
                        taxinvoicehd.CustAddr2 = cust.CustAddr2;
                        taxinvoicehd.CitizenId = cust.CitizenId ?? "";
                        taxinvoicehd.Currency = salehd.Currency;
                        taxinvoicehd.CurRate = salehd.CurRate;
                        taxinvoicehd.DiscRate = "";
                        taxinvoicehd.DiscAmt = Math.Round((decimal)salehd.DiscAmt, 2);
                        taxinvoicehd.DiscAmtCur = Math.Round((decimal)salehd.DiscAmtCur, 2);


                        int seqno = 0;
                        decimal? subamt = 0;
                        decimal? subamtcur = 0;

                        items.ForEach(x =>
                        {
                            x.SubAmt = x.SumItemAmt - x.DiscAmt;
                            x.SubAmtCur = x.SumItemAmtCur - x.DiscAmtCur;
                            subamt += x.SubAmt;
                            subamtcur += x.SubAmtCur;
                        });
                        taxinvoicehd.SubAmt = Math.Round((decimal)subamt, 2);
                        taxinvoicehd.SubAmtCur = Math.Round((decimal)subamtcur, 2);

                        foreach (var item in items)
                        {
                            item.DocNo = taxinvoicehd.DocNo;
                            item.SeqNo = ++seqno;

                            var licenseplates = this.context.SalCreditsaleDts.Where(d =>
                                d.CompCode == creditsalehd.CompCode
                                && d.BrnCode == creditsalehd.BrnCode
                                && d.LocCode == creditsalehd.LocCode
                                && d.DocType == creditsalehd.DocType
                                && d.PdId == item.PdId
                                && d.UnitBarcode == item.UnitBarcode
                                && docnos.Contains(d.DocNo)).Select(x => x.LicensePlate).Distinct().ToList();
                            item.LicensePlate = string.Join(",", licenseplates);
                            this.context.SalTaxinvoiceDts.Add(item);
                        } //items


                        taxinvoicehd.TotalAmt = taxinvoicehd.SubAmt - taxinvoicehd.DiscAmt; //totalamt;
                        taxinvoicehd.TotalAmtCur = taxinvoicehd.SubAmtCur - taxinvoicehd.DiscAmtCur; // totalamtcur;
                        taxinvoicehd.TaxBaseAmt = items.Sum(x => x.TaxBaseAmt);
                        taxinvoicehd.TaxBaseAmtCur = items.Sum(x => x.TaxBaseAmtCur);
                        taxinvoicehd.VatRate = 0;
                        taxinvoicehd.VatAmt = items.Sum(x => x.VatAmt);
                        taxinvoicehd.VatAmtCur = items.Sum(x => x.VatAmtCur);
                        taxinvoicehd.NetAmt = taxinvoicehd.TaxBaseAmt + taxinvoicehd.VatAmt;
                        taxinvoicehd.NetAmtCur = taxinvoicehd.TaxBaseAmtCur + taxinvoicehd.VatAmtCur;

                        taxinvoicehd.ItemCount = items.Count();
                        taxinvoicehd.Post = "P";
                        taxinvoicehd.DocPattern = pattern;
                        taxinvoicehd.Guid = Guid.NewGuid();
                        taxinvoicehd.CreatedBy = query.User;
                        taxinvoicehd.CreatedDate = DateTime.Now;
                        this.context.SalTaxinvoiceHds.Add(taxinvoicehd);

                        //insert balance
                        var balance = new FinBalance
                        {
                            CompCode = taxinvoicehd.CompCode,
                            BrnCode = taxinvoicehd.BrnCode,
                            LocCode = taxinvoicehd.LocCode,
                            DocType = taxinvoicehd.DocType,
                            DocNo = taxinvoicehd.DocNo,
                            DocDate = taxinvoicehd.DocDate,
                            CustCode = taxinvoicehd.CustCode,
                            Currency = taxinvoicehd.Currency,
                            NetAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2),
                            NetAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2),
                            BalanceAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2), //0,
                            BalanceAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2), //0,
                            CreatedBy = query.User,
                            CreatedDate = DateTime.Now
                        };
                        this.context.FinBalances.Add(balance);

                        // update creditsalehd
                        var sales = this.context.SalCreditsaleHds.Where(x => x.CompCode == creditsalehd.CompCode
                                                                             && x.BrnCode == creditsalehd.BrnCode
                                                                             && x.LocCode == creditsalehd.LocCode
                                                                             && x.DocType == creditsalehd.DocType
                                                                             && docnos.Contains(x.DocNo)
                        ).ToList();
                        sales.ForEach(x =>
                        {
                            x.DocStatus = "Reference";
                            x.TxNo = taxinvoicehd.DocNo;
                            x.UpdatedBy = query.User;
                            x.UpdatedDate = DateTime.Now;
                        });
                        this.context.SalCreditsaleHds.UpdateRange(sales);
                        this.context.SaveChanges();

                        //end รวบบิล  
                    }
                    else
                    {
                        //ไม่ต้องรวบบิล  => ดึงทุกใบของลูกค้านี้ขึ้นมา
                        var salehds = await this.context.SalCreditsaleHds.Where(x => x.DocStatus != "Cancel" &&
                            x.Post == "P"
                            && (x.TxNo == null || x.TxNo == "")
                            && x.CompCode == creditsalehd.CompCode
                            && x.BrnCode == creditsalehd.BrnCode
                            && x.LocCode == creditsalehd.LocCode
                            && x.DocType == creditsalehd.DocType
                            && x.DocDate == creditsalehd.DocDate
                            && x.CustCode == creditsalehd.CustCode
                        ).OrderBy(x => x.DocNo).ToListAsync();

                        foreach (var salehd in salehds)
                        {
                            //insert taxinvoice hd
                            taxinvoicehd = new SalTaxinvoiceHd();
                            taxinvoicehd.RunNumber = ++runno;
                            taxinvoicehd.CompCode = salehd.CompCode;
                            taxinvoicehd.BrnCode = salehd.BrnCode;
                            taxinvoicehd.LocCode = salehd.LocCode;
                            taxinvoicehd.DocNo = GenDocNo(query, pattern, (int)taxinvoicehd.RunNumber);
                            taxinvoicehd.DocStatus = "Active";
                            taxinvoicehd.DocType = "CreditSale";
                            taxinvoicehd.DocDate = salehd.DocDate;
                            taxinvoicehd.CustCode = salehd.CustCode;
                            taxinvoicehd.CustName = salehd.CustName;
                            taxinvoicehd.CitizenId = cust.CitizenId;
                            taxinvoicehd.CustAddr1 = cust.CustAddr1; //salehd.CustAddr1;
                            taxinvoicehd.CustAddr2 = cust.CustAddr2; //salehd.CustAddr2;
                            taxinvoicehd.RefNo = salehd.RefNo;
                            taxinvoicehd.ItemCount = salehd.ItemCount;
                            taxinvoicehd.Currency = salehd.Currency;
                            taxinvoicehd.CurRate = salehd.CurRate;
                            taxinvoicehd.SubAmt = salehd.SubAmt;
                            taxinvoicehd.SubAmtCur = salehd.SubAmtCur;
                            taxinvoicehd.DiscRate = salehd.DiscRate;
                            taxinvoicehd.DiscAmt = salehd.DiscAmt;
                            taxinvoicehd.DiscAmtCur = salehd.DiscAmtCur;
                            taxinvoicehd.TotalAmt = Math.Round((decimal)salehd.TotalAmt, 2);
                            taxinvoicehd.TotalAmtCur = Math.Round((decimal)salehd.TotalAmtCur, 2);
                            taxinvoicehd.TaxBaseAmt = Math.Round((decimal)salehd.TaxBaseAmt, 2);
                            taxinvoicehd.TaxBaseAmtCur = Math.Round((decimal)salehd.TaxBaseAmtCur, 2);
                            taxinvoicehd.VatRate = salehd.VatRate;
                            taxinvoicehd.VatAmt = Math.Round((decimal)salehd.VatAmt, 2);
                            taxinvoicehd.VatAmtCur = Math.Round((decimal)salehd.VatAmtCur, 2);
                            taxinvoicehd.NetAmt = Math.Round((decimal)salehd.NetAmt, 2);
                            taxinvoicehd.NetAmtCur = Math.Round((decimal)salehd.NetAmtCur, 2);
                            taxinvoicehd.Post = "P";
                            taxinvoicehd.DocPattern = pattern;


                            //insert taxinvoice dt
                            var items = this.context.SalCreditsaleDts.Where(x => x.CompCode == salehd.CompCode
                                    && x.BrnCode == salehd.BrnCode
                                    && x.LocCode == salehd.LocCode
                                    && x.DocType == salehd.DocType
                                    && x.DocNo == salehd.DocNo)
                                .Select(x => new SalTaxinvoiceDt
                                {
                                    CompCode = x.CompCode,
                                    BrnCode = x.BrnCode,
                                    LocCode = x.LocCode,
                                    DocNo = taxinvoicehd.DocNo,
                                    SeqNo = x.SeqNo,
                                    LicensePlate = x.LicensePlate,
                                    PdId = x.PdId,
                                    PdName = x.PdName,
                                    IsFree = x.IsFree,
                                    UnitId = x.UnitId,
                                    UnitBarcode = x.UnitBarcode,
                                    UnitName = x.UnitName,
                                    ItemQty = x.ItemQty ?? decimal.Zero,
                                    StockQty = x.StockQty ?? decimal.Zero,
                                    UnitPrice = x.UnitPrice,
                                    UnitPriceCur = x.UnitPriceCur,
                                    SumItemAmt = x.SumItemAmt,
                                    SumItemAmtCur = x.SumItemAmtCur,
                                    DiscAmt = x.DiscAmt,
                                    DiscAmtCur = x.DiscAmtCur,
                                    DiscHdAmt = Math.Round((decimal)x.DiscHdAmt, 2),
                                    DiscHdAmtCur = Math.Round((decimal)x.DiscHdAmtCur, 2),
                                    SubAmt = x.SubAmt,
                                    SubAmtCur = x.SubAmtCur,
                                    VatType = x.VatType,
                                    VatRate = x.VatRate,
                                    VatAmt = Math.Round((decimal)x.VatAmt, 2),
                                    VatAmtCur = Math.Round((decimal)x.VatAmtCur, 2),
                                    TaxBaseAmt = Math.Round((decimal)x.TaxBaseAmt, 2),
                                    TaxBaseAmtCur = Math.Round((decimal)x.TaxBaseAmtCur, 2),
                                    TotalAmt = Math.Round((decimal)x.TotalAmt, 2),
                                    TotalAmtCur = Math.Round((decimal)x.TotalAmtCur, 2)
                                }).ToList();

                            this.context.SalTaxinvoiceDts.AddRange(items);

                            taxinvoicehd.ItemCount = items.Count();
                            taxinvoicehd.Guid = Guid.NewGuid();
                            taxinvoicehd.CreatedBy = query.User;
                            taxinvoicehd.CreatedDate = DateTime.Now;

                            this.context.SalTaxinvoiceHds.Add(taxinvoicehd);

                            //insert balance
                            var balance = new FinBalance
                            {
                                CompCode = taxinvoicehd.CompCode,
                                BrnCode = taxinvoicehd.BrnCode,
                                LocCode = taxinvoicehd.LocCode,
                                DocType = taxinvoicehd.DocType,
                                DocNo = taxinvoicehd.DocNo,
                                DocDate = taxinvoicehd.DocDate,
                                CustCode = taxinvoicehd.CustCode,
                                Currency = taxinvoicehd.Currency,
                                NetAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2),
                                NetAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2),
                                BalanceAmt = Math.Round((decimal)taxinvoicehd.NetAmt, 2), // 0,
                                BalanceAmtCur = Math.Round((decimal)taxinvoicehd.NetAmtCur, 2), // 0,
                                CreatedBy = query.User,
                                CreatedDate = DateTime.Now
                            };
                            this.context.FinBalances.Add(balance);


                            // update creditsalehd

                            salehd.DocStatus = "Reference";
                            salehd.TxNo = taxinvoicehd.DocNo; // reference tax_doc_no
                            salehd.UpdatedBy = query.User;
                            salehd.UpdatedDate = DateTime.Now;
                            this.context.SalCreditsaleHds.Update(salehd);
                            this.context.SaveChanges();
                        } //foreach salehd
                    } //BillType
                } //foreach

                #endregion


                #region Invoice 1:1

                //ไม่ต้องรวบเอกสาร
                var taxinvs = await (from cre in this.context.SalCreditsaleHds
                                     join cus in this.context.MasCustomers on cre.CustCode equals cus.CustCode
                                     where cre.CompCode == query.CompCode
                                           && cre.BrnCode == query.BrnCode
                                           && cre.Post == "P"
                                           && cre.DocType == "Invoice"
                                           && (cre.TxNo == "" || cre.TxNo == null)
                                     select new { cre, cus }).Select(x => new SalTaxinvoiceHd
                                     {
                                         CompCode = x.cre.CompCode,
                                         BrnCode = x.cre.BrnCode,
                                         LocCode = x.cre.LocCode,
                                         DocType = x.cre.DocType,
                                         DocNo = x.cre.DocNo,
                                         DocDate = x.cre.DocDate,
                                         CustCode = x.cre.CustCode,
                                         CustName = x.cre.CustName,
                                         CustAddr1 = x.cre.CustAddr1,
                                         CustAddr2 = x.cre.CustAddr2,
                                         CitizenId = x.cus.CitizenId,
                                         Currency = x.cre.Currency,
                                         CurRate = x.cre.CurRate,
                                         ItemCount = x.cre.ItemCount,
                                         SubAmt = x.cre.SubAmt,
                                         SubAmtCur = x.cre.SubAmtCur,
                                         DiscRate = x.cre.DiscRate,
                                         DiscAmt = x.cre.DiscAmt,
                                         DiscAmtCur = x.cre.DiscAmtCur,
                                         TotalAmt = x.cre.TotalAmt,
                                         TotalAmtCur = x.cre.TotalAmtCur,
                                         TaxBaseAmt = Math.Round((decimal)x.cre.TaxBaseAmt, 2),
                                         TaxBaseAmtCur = Math.Round((decimal)x.cre.TaxBaseAmtCur, 2),
                                         VatRate = x.cre.VatRate,
                                         VatAmt = Math.Round((decimal)x.cre.VatAmt, 2),
                                         VatAmtCur = Math.Round((decimal)x.cre.VatAmtCur, 2),
                                         NetAmt = Math.Round((decimal)x.cre.NetAmt, 2),
                                         NetAmtCur = Math.Round((decimal)x.cre.NetAmtCur, 2),
                                         Post = x.cre.Post
                                     }).ToListAsync();


                foreach (var taxinvhd in taxinvs)
                {
                    var invno = taxinvhd.DocNo;
                    taxinvhd.RunNumber = ++runno;
                    taxinvhd.DocNo = GenDocNo(query, pattern, (int)taxinvhd.RunNumber);
                    taxinvhd.DocStatus = "Active";
                    taxinvhd.DocPattern = pattern;

                    List<string> pdmeter = new List<string>() { "90575", "90581" };

                    var items = this.context.SalCreditsaleDts.Where(x => x.CompCode == taxinvhd.CompCode
                                                                         && x.BrnCode == taxinvhd.BrnCode
                                                                         && x.LocCode == taxinvhd.LocCode
                                                                         && x.DocType == taxinvhd.DocType
                                                                         && x.DocNo == invno)
                        .Select(x => new SalTaxinvoiceDt
                        {
                            CompCode = x.CompCode,
                            BrnCode = x.BrnCode,
                            LocCode = x.LocCode,
                            DocNo = taxinvhd.DocNo,
                            SeqNo = x.SeqNo,
                            LicensePlate = "",
                            PdId = x.PdId,
                            PdName = x.PdName + (pdmeter.Contains(x.PdId) ? $"({x.MeterStart} - {x.MeterFinish})" : ""),
                            IsFree = x.IsFree,
                            UnitId = x.UnitId,
                            UnitBarcode = x.UnitBarcode,
                            UnitName = x.UnitName,
                            ItemQty = x.ItemQty ?? decimal.Zero,
                            StockQty = x.StockQty ?? decimal.Zero,
                            UnitPrice = x.UnitPrice,
                            UnitPriceCur = x.UnitPriceCur,
                            SumItemAmt = x.SumItemAmt,
                            SumItemAmtCur = x.SumItemAmtCur,
                            DiscAmt = x.DiscAmt,
                            DiscAmtCur = x.DiscAmtCur,
                            DiscHdAmt = Math.Round((decimal)x.DiscHdAmt, 2),
                            DiscHdAmtCur = Math.Round((decimal)x.DiscHdAmtCur, 2),
                            SubAmt = x.SubAmt,
                            SubAmtCur = x.SubAmtCur,
                            VatType = x.VatType,
                            VatRate = (x.VatType == "NV") ? 0 : x.VatRate,
                            VatAmt = Math.Round((decimal)x.VatAmt, 2),
                            VatAmtCur = Math.Round((decimal)x.VatAmtCur, 2),
                            TaxBaseAmt = Math.Round((decimal)x.TaxBaseAmt, 2),
                            TaxBaseAmtCur = Math.Round((decimal)x.TaxBaseAmtCur, 2),
                            TotalAmt = Math.Round((decimal)x.TotalAmt, 2),
                            TotalAmtCur = Math.Round((decimal)x.TotalAmtCur, 2)
                        }).ToList();

                    this.context.SalTaxinvoiceDts.AddRange(items);

                    taxinvhd.ItemCount = items.Count;
                    taxinvhd.CreatedBy = query.User;
                    taxinvhd.CreatedDate = DateTime.Now;
                    this.context.SalTaxinvoiceHds.Add(taxinvhd);

                    var balance = new FinBalance
                    {
                        CompCode = taxinvhd.CompCode,
                        BrnCode = taxinvhd.BrnCode,
                        LocCode = taxinvhd.LocCode,
                        DocType = taxinvhd.DocType,
                        DocNo = taxinvhd.DocNo,
                        DocDate = taxinvhd.DocDate,
                        CustCode = taxinvhd.CustCode,
                        Currency = taxinvhd.Currency,
                        NetAmt = Math.Round((decimal)taxinvhd.NetAmt, 2),
                        NetAmtCur = Math.Round((decimal)taxinvhd.NetAmtCur, 2),
                        BalanceAmt = Math.Round((decimal)taxinvhd.NetAmt, 2), //0,
                        BalanceAmtCur = Math.Round((decimal)taxinvhd.NetAmtCur, 2), //0,
                        CreatedBy = query.User,
                        CreatedDate = DateTime.Now
                    };
                    this.context.FinBalances.Add(balance);

                    //update salehd
                    var salehds = await this.context.SalCreditsaleHds.Where(x => (x.TxNo == null || x.TxNo == "")
                        && x.Post == taxinvhd.Post
                        && x.CompCode == taxinvhd.CompCode
                        && x.BrnCode == taxinvhd.BrnCode
                        && x.LocCode == taxinvhd.LocCode
                        && x.DocType == taxinvhd.DocType
                        && x.DocNo == invno
                    ).ToListAsync();
                    salehds.ForEach(x =>
                    {
                        x.DocStatus = "Reference";
                        x.TxNo = taxinvhd.DocNo;
                        x.UpdatedBy = query.User;
                        x.UpdatedDate = DateTime.Now;
                    });
                    this.context.SalCreditsaleHds.UpdateRange(salehds);
                    this.context.SaveChanges();
                } //end foreach

                #endregion
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the CreateTaxInvoiceAsync: {strMessage}");
            }
        }

        public async Task<RequestWarpadModel> GetDataToWarpadAsync(PostDayResource query)
        {
            try
            {
                var requestWarpad = new RequestWarpadModel();
                var lastPeriod = await context.DopPeriodTanks
                    .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate)
                    .OrderBy(x => x.PeriodNo).LastOrDefaultAsync();
                if (lastPeriod != null)
                {
                    var listTankData = await context.DopPeriodTanks
                        .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode &&
                                    x.DocDate == query.WDate && x.PeriodNo == lastPeriod.PeriodNo).AsNoTracking()
                        .ToListAsync();
                    var listMasTankData = await context.MasBranchTanks
                        .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode).AsNoTracking()
                        .ToListAsync();
                    var configApi = context.SysConfigApis.Where(x => x.SystemId == "Warpad" && x.ApiId == "M005")
                        .FirstOrDefault();

                    if (configApi != null)
                    {
                        var devCodeFrom = context.MasOrganizes
                            .Where(x => x.OrgComp == query.CompCode && x.OrgCode == query.BrnCode)
                            .Select(x => x.OrgCodedev).FirstOrDefault();
                        var toppic = configApi.Topic.Replace("{doc_date}",
                            query.WDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

                        requestWarpad.TOPIC = toppic;
                        requestWarpad.CREATE_DATE =
                            query.WDate.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                        requestWarpad.CREATE_TIME = query.WDate.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
                        requestWarpad.BRANCH_FROM = devCodeFrom ?? "";
                        requestWarpad.LINK = "#";

                        var listItemRequestWarpad = new List<RequestWarpadDataMedel>();

                        foreach (var item in listTankData)
                        {
                            var map = listMasTankData.Where(x => x.TankId == item.TankId).FirstOrDefault();
                            if (map != null)
                            {
                                if (item.RealQty < map.CapacityMin)
                                {
                                    listItemRequestWarpad.Add(new RequestWarpadDataMedel() { ITEM = item.PdName });
                                }
                            }
                        }

                        requestWarpad.DATA = listItemRequestWarpad;
                    }
                }

                return requestWarpad;
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                return new RequestWarpadModel();
                throw new Exception($"An error occurred when saving the GetDataToWarpad: {strMessage}");
            }
        }

        public async Task SendDataToWarpadAsync(RequestWarpadModel warpadData)
        {
            try
            {
                var configApi = context.SysConfigApis.Where(x => x.SystemId == "Warpad" && x.ApiId == "M005")
                    .FirstOrDefault();

                if(configApi != null)
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(configApi.ApiUrl);
                    var content = new StringContent(JsonConvert.SerializeObject(warpadData), Encoding.UTF8,
                        "application/json");
                    var response = await client.PostAsync(configApi.ApiUrl, content);
                }
                
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception($"An error occurred when saving the SendDataToWarpadAsync: {strMessage}");
            }
        }

        public async Task ExecuteStoredprocedureStockAsnyc(SaveDocumentRequest req, DateTime docDate)
        {
            try
            {
                string strSpAddStock = @"exec sp_add_stock_daily @p0 ,@p1,@p2,@p3 , @p4";
                object[] arrAddStockParam = new[]
                {
                    req.DopPostdayHd.CompCode,
                    req.DopPostdayHd.BrnCode,
                    req.DopPostdayHd.LocCode,
                    req.DopPostdayHd.User,
                    req.DopPostdayHd.DocDate
                };
                await context.Database.ExecuteSqlRawAsync(strSpAddStock, arrAddStockParam);

                //var docYear = docDate.Year;
                //var docMonth = docDate.Month;
                //var docDay = docDate.Day;

                //if (docDay == DateTime.DaysInMonth(docYear, docMonth))
                //{
                //    string strSpAddStockMonthly = @"exec sp_add_stock_monthly @p0,@p1,@p2,@p3,@p4,@p5";
                //    object[] arrAddStockMonthlyParam = new object[] {
                //        req.DopPostdayHd.CompCode,
                //        req.DopPostdayHd.BrnCode,
                //        req.DopPostdayHd.LocCode,
                //        req.DopPostdayHd.User,
                //        docYear ,
                //        docMonth,
                //    };
                //    await context.Database.ExecuteSqlRawAsync(strSpAddStockMonthly, arrAddStockMonthlyParam);
                //}
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                throw new Exception(
                    $"An error occurred when saving the ExecuteStoredprocedureStockMonthlyAsnyc: {strMessage}");
            }
        }

        public MasDocPattern GetDocPattern(string docType)
        {
            var docpattern = context.MasDocPatterns.FirstOrDefault(x => x.DocType == docType);
            return docpattern;
        }

        public int GetRunningNo(string compCode, string brnCode, string pattern)
        {
            int runNumber = 0;
            SalTaxinvoiceHd resp = new SalTaxinvoiceHd();
            resp = this.context.SalTaxinvoiceHds
                .Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.DocPattern == pattern)
                .OrderByDescending(x => x.RunNumber).FirstOrDefault();
            if (resp != null)
            {
                runNumber = (resp.RunNumber ?? 0);
            }

            return runNumber;
        }

        private string GenDocNo(PostDayResource query, string pattern, int runNumber)
        {
            string docno = "";

            var date = query.WDate.Value;
            var Brn = query.BrnCode;

            var patterns = (from hd in this.context.MasDocPatterns
                            join dt in this.context.MasDocPatternDts on hd.DocId equals dt.DocId
                            where hd.DocType == "TaxInvoice"
                            select dt).ToList();

            docno = pattern;
            docno = docno.Replace("Pre", patterns.FirstOrDefault(x => x.DocCode == "[Pre]").DocValue);
            docno = docno.Replace("#", "") +
                    runNumber.ToString("D" + patterns.FirstOrDefault(x => x.DocCode == "[#]").DocValue);
            return docno;
        }

        public async Task UpdateAdjustRequestAsync(PostDayResource query)
        {
            try
            {
                var invAdjustRequest = await context.InvAdjustRequestHds
                    .Where(
                        x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate == query.WDate &&
                             x.Post != "P"
                    ).ToListAsync();

                invAdjustRequest.ForEach(x =>
                {
                    x.UpdatedBy = query.User;
                    x.UpdatedDate = DateTime.Now;
                    x.Post = "P";
                });

                context.UpdateRange(invAdjustRequest);
            }
            catch (Exception ex)
            {
                //string strMessage = ex.StackTrace;
                //throw new Exception($"An error occurred when saving the UpdateAdjustAsync: {strMessage}");
            }
        }
    }
}