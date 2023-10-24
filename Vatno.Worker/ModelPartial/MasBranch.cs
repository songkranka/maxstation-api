using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class MasBranch
    {
        [NotMapped]
        public string FullAddress { get { return (this.Address + " " + (this.SubDistrict ?? "") + " " + (this.District ?? "") + " " + (this.Postcode ?? "")).Trim(); } }

        [NotMapped]
        public string CompanyName { get; set; }
    }
}
