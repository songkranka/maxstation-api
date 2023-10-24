namespace Vatno.Worker.Models
{
    public partial class MasEmployee
    {
        public string EmpName { get { return ($"{this.PrefixThai} {this.PersonFnameThai} {this.PersonLnameThai}").Trim(); } }
    }
}
