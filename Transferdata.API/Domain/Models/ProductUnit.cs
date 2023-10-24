using System;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Domain.Models
{
    public class ProductUnit
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime DocDate { get; set; }   
        public string PDBarcodeList { get; set; }
    }
}
