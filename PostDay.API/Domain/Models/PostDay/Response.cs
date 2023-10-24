using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PostDay.API.Domain.Models.PostDay
{
    public class SaveDocumentResponse
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class GetDocumentResponse
    {
        public DopPostdayHd DopPostdayHd { get; set; }
        public List<DopPostdayDt> CrItems { get; set; }
        public List<DopPostdayDt> DrItems { get; set; }
        public List<Formula> FormulaItems { get; set; }
        public SumInDay SumData { get; set; }
        public List<CheckBeforeSave> CheckBeforeSaveItems { get; set; }
        public List<ModelMecPostPaidValidate> ListValidatePostPaid { get; set; }
    }

    public class CheckBeforeSaveView
    {
        public List<CheckBeforeSave> checkBeforeSaves { get; set; }
        public List<ModelMecPostPaidValidate> mecPostPaidValidates { get; set; }
    }

    public class CheckBeforeSave
    {
        public string Label { get; set; }
        public string PassValue { get; set; }
        public int ValidNo { get; set; }
        public bool HaveValidSql { get; set; }
    }

    

    public class SumInDay
    {
        public decimal SumCashAmt { get; set; }
        public decimal SumDiffAmt { get; set; }
        public decimal SumCashDepositAmt { get; set; }
        public decimal SumChequeAmt { get; set; }
    }

    public class Formula
    {
        public decimal FmNo { get; set; }
        public string Remark { get; set; }
        public decimal SourceAmount { get; set; }
        public decimal DestinationAmount { get; set; }
        public string Unit { get; set; }
    }

    public class ResponseFormula
    {
        public string MsgCode { get; set; }
        public string MsgDesc { get; set; }
        public ResponseFormulaDetail Detail { get; set; }
    }

    public class ResponseFormulaDetail
    {
        public decimal Discount { get; set; }
        public string ShopName { get; set; }
        public string ReqtKey { get; set; }
        public string CompanyCode { get; set; }
        public string ShopCode { get; set; }
        public string SystemDate { get; set; }
    }

    [Serializable, JsonObject]
    public class CardStockResponse
    {
        [JsonProperty] public string ResponseCode { get; set; }
        [JsonProperty] public string ResponseMessage { get; set; }
        [JsonProperty] public List<ResultModel> ResultModel { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(ResultModel);
        }
    }

    [Serializable, JsonObject]
    public class ResultModel
    {
        public string ProvinceName { get; set; }
        public string OriShopCode { get; set; }
        public string ShopName { get; set; }
        public int StockTotal { get; set; }
        public int TouchPoint { get; set; }
        public int Online { get; set; }
        public int CardLost { get; set; }
        public int Other { get; set; }
        public int TransferOut { get; set; }
        public int TransferIn { get; set; }
        public int StockBalance { get; set; }
        public int ExtendCard { get; set; }
        public int ExtendCardPrice { get; set; }
        public int DiscountBuy { get; set; }
        public int DiscountBuyPrice { get; set; }
        public int DiscountExtend { get; set; }
        public int DiscountExtendPrice { get; set; }
    }

    [Serializable, JsonObject]
    public class MaxCardResponse
    {
        public ResultMaxCard RESULT { get; set; }
        public string CODE { get; set; }
        public string BRANCH { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(RESULT);
        }
    }

    [Serializable, JsonObject]
    public class ResultMaxCard
    {
        public List<PostPaid> POSTPAID { get; set; }
        public PrePaid PREPAID { get; set; }
        public UserComp USECOMP { get; set; }
    }

    [Serializable, JsonObject]
    public class UserComp
    {
        public decimal total_baht { get; set; }
        public decimal total_lite { get; set; }
    }


    [Serializable, JsonObject]
    public class PostPaid
    {
        public string maxcardno { get; set; }
        public string cusname { get; set; }
        public string cusno { get; set; }
        public string cusno_sap { get; set; }
        public decimal total_baht { get; set; }
        public decimal total_lite { get; set; }
    }

    [Serializable, JsonObject]
    public class PrePaid
    {
        public decimal total_baht { get; set; }
        public decimal total_lite { get; set; }
    }

}