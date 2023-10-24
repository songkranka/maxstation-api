using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.Withdraw
{
    public class WithdrawResource
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }        
        public string DocStatus { get; set; }
        //public string UseBy { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string ReasonDesc { get; set; }
        public Guid Guid { get; set; }
    }
}
