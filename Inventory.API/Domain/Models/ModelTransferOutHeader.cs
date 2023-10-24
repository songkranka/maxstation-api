using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models
{
    public class ModelTransferOutHeader : InvTranoutHd
    {
        public List<InvTranoutDt> ListTransOutDt { get; set; }
        //public DateTime? RefDocDate { get; set; }
    }


    public class ModelRequestDetail : InvRequestDt
    {

    }
    //public class ModelTransferOutDetail : InvTranoutDt
    //{
    //    public decimal? SumAmt { get; set; }
    //}

    public class ModelTransferOutHeaderLog
    {
        public string compCode { get; set; }
        public string brnCode { get; set; }
        public string locCode { get; set; }
        public DateTime? docDate { get; set; }
        public string refNo { get; set; }
        public string brnCodeTo { get; set; }
        public string remark { get; set; }
        public string createdBy { get; set; }
        public ModelTransferOutDetailLog[] listTransOutDt { get; set; }
    }

    public class ModelTransferOutDetailLog
    {
        public string brnCode { get; set; }
        public string compCode { get; set; }
        public string locCode { get; set; }
        public string pdId { get; set; }
        public string unitBarcode { get; set; }
        public int seqNo { get; set; }
        public decimal refQty { get; set; }
        public decimal itemQty { get; set; }
    } 
    public class ModelCheckStockRealtimeParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime? DocDate { get; set; }
        public string Json { get; set; }
    }

    public class ModelStockRealTime
    {
        public string  PdId { get; set; }
        public string UnitId { get; set; }
        public string UnitBarCode { get; set; }
        public decimal? ItemQty { get; set; }
        public decimal? Remain { get; set; }
    }

}
