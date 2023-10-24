using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.CreditNote
{
    public class InsertCreditNoteQuery : QueryResource
    {
        public MaxStation.Entities.Models.SalCreditsaleHd CreditSaleHeader { get; set; }
        public MaxStation.Entities.Models.SalCreditsaleDt[] ArrCreditSaleDetail { get; set; }


    }
}
