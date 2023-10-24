using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class SalCashsaleHd
    {
        public SalCashsaleHd()
        {
            SalCashsaleDt = new HashSet<SalCashsaleDt>();
        }

        [NotMapped]
        public ICollection<SalCashsaleDt> SalCashsaleDt { get; set; }


    }
}
