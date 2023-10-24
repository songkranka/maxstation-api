using System;

namespace Transferdata.API.Domain.Models.Queries
{

    public class ReceiveProdQuery
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? DocDate { get; set; }

    }
}
