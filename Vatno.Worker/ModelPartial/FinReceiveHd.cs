using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class FinReceiveHd
    {
        public FinReceiveHd()
        {
            FinReceiveDt = new HashSet<FinReceiveDt>();
            FinReceivePay = new HashSet<FinReceivePay>();
        }

        [NotMapped]
        public string CustPrefix { get; set; }

        [NotMapped]
        public ICollection<FinReceiveDt> FinReceiveDt { get; set; }
        [NotMapped]
        public ICollection<FinReceivePay> FinReceivePay { get; set; }
    }
}
