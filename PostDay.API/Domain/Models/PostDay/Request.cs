using MaxStation.Entities.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PostDay.API.Domain.Models.PostDay
{
    public class SaveDocumentRequest
    {
        public CtDopPostdayHd DopPostdayHd { get; set; }
        public List<DopPostdayDt> CrItems { get; set; }
        public List<DopPostdayDt> DrItems { get; set; }
        public List<Formula> FormulaItems { get; set; }
        public List<CheckBeforeSave> CheckBeforeSaveItems { get; set; }
    }

    [Serializable, JsonObject]
    public class GetDocumentRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocDate { get; set; }
        public string SystemDate { get; set; }
    }

    public class CtDopPostdayHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string User { get; set; }
        public string DocDate { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public decimal CashAmt { get; set; }
        public decimal DiffAmt { get; set; }
        public decimal DepositAmt { get; set; }
        public decimal ChequeAmt { get; set; }
    }

    public class RequestFormula
    {
        public string ReqtKey { get; set; }
        public string CompanyCode { get; set; }
        public string ShopCode { get; set; }
        public string SystemDate { get; set; }
    }

    public class AddStockParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime SysDate { get; set; }
    }

    public class AddStockMonthlyParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string CreatedBy { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class GetDopValidDataParam
    {
        public int ValidNo { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocDate { get; set; }
    }

    public class CardStockRequest
    {
        public DateTime StockDateFrom { get; set; }
        public DateTime StockDateTo { get; set; }
        public string OriShopCode { get; set; }
        public string CompanyCode { get; set; }
    }

    public class MaxCardRequest
    {
        public string date { get; set; }
        public string branch_id { get; set; }
        public string apikey { get; set; }
    }
}
