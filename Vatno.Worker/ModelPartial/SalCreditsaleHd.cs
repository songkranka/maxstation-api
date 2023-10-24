using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class SalCreditsaleHd
    {
        public SalCreditsaleHd()
        {
            SalCreditsaleDt = new List<SalCreditsaleDt>();
        }

        [NotMapped]
        public List<SalCreditsaleDt> SalCreditsaleDt { get; set; }

    }
}
