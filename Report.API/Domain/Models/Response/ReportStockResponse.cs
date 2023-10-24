using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class ReportStockResponse
    {
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string docDate { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public List<Stock> stocks { get; set; }

        public class Stock
        {
            public string productGroupId { get; set; }
            public string productGroupName { get; set; }
            public List<StockDt> stockDts { get; set; }
        }

        public class StockDt
        {
            public int seqNo { get; set; }
            public string productId { get; set; }
            public string productName { get; set; }
            public decimal stockBanlance { get; set; }
            public decimal receiveStock { get; set; }
            public decimal traninStock { get; set; }
            public decimal sale { get; set; }
            public decimal isFree { get; set; }
            public decimal tranoutStock { get; set; }
            public decimal withdrawStock { get; set; }
            public decimal returnSupStock { get; set; }
            public decimal adjustStock { get; set; }
            public decimal auditStock { get; set; }
            public decimal balance { get; set; }
            public string productGroupId { get; set; }
            public string productGroupName { get; set; }
            public decimal sumStockBanlance { get; set; }
            public decimal sumReceiveStock { get; set; }
            public decimal sumTraninStock { get; set; }
            public decimal sumSale { get; set; }
            public decimal sumIsFree { get; set; }
            public decimal sumTranOutStock { get; set; }
            public decimal sumWithdrawStock { get; set; }
            public decimal sumReturnSupStock { get; set; }
            public decimal sumAdjustStock { get; set; }
            public decimal sumAuditStock { get; set; }
            public decimal sumBalance { get; set; }
        }
    }
}
