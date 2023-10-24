using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class MasCustomer
    {
        [NotMapped]
        public string CustAddr1 { get { return (this.Address + " " + this.SubDistrict ?? "").Trim(); } }

        [NotMapped]
        public string CustAddr2 { get { return (this.District + " " + this.ProvName + " " + this.Postcode).Trim(); } }

    }
}
