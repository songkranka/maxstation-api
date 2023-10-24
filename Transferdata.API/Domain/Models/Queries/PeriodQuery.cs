using System;

namespace Transferdata.API.Domain.Models.Queries
{
    public class PeriodQuery
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? DocDate { get; set; }
        public int PeriodNo { get; set; }
    }
}
