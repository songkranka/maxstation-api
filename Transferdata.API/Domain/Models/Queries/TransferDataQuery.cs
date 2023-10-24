using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Domain.Models.Queries
{
    public class TransferDataQuery
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? DocDate { get; set; }



        public TransferDataQuery(string brnCode, string compCode, DateTime docDate)
        {
            BrnCode = brnCode;
            CompCode = compCode;
            DocDate = docDate;
        }
    }
}
