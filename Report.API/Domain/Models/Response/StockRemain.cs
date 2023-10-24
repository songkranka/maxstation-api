using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class StockRemain
    {
        public string PdId { get; set; }
        public string GroupId { get; set; }

        public decimal Banlance { get; set; }
        public decimal Receive { get; set; }
        public decimal TranIn { get; set; }
        public decimal TranOut { get; set; }
        public decimal CashSale { get; set; }
        public decimal CreditSale { get; set; }
        public decimal FreeCashSale { get; set; }
        public decimal FreeCreditSale { get; set; }
        public decimal ReturnSup { get; set; }
        public decimal WithDraw { get; set; }
        public decimal Adjust { get; set; }
        public decimal Remain { get; set; }
    }
}
