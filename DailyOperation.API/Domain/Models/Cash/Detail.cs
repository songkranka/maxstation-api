using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models.Cash
{
    public class Detail
    {
        public string JOURNAL_ID { get; set; }
        public int RUNNO { get; set; }
        public string ITEM_TYPE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string PLU_NUMBER { get; set; }
        public string DISC_GROUP { get; set; }
        public string PRODUCT_CODESAP { get; set; }
        public decimal SELL_QTY { get; set; }
        public decimal SELL_PRICE { get; set; }
        public int GOODS_AMT { get; set; }
        public string TAX_RATE { get; set; }
        public decimal TAX_AMT { get; set; }
        public int HOSE_ID { get; set; }
        public int PUMP_ID { get; set; }
        public int DELIVERY_ID { get; set; }
        public int TANK_ID { get; set; }
        public int DELIVERY_TYPE { get; set; }
        public decimal DISC_AMT { get; set; }
        public DateTime COMPLETED_TS { get; set; }
        public int SHIFT_ID { get; set; }
        public int POS_ID { get; set; }
    }
}
