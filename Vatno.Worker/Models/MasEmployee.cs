#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasEmployee
    {
        public string EmpCode { get; set; }
        public string PrefixThai { get; set; }
        public string PersonFnameThai { get; set; }
        public string PersonLnameThai { get; set; }
        public string PrefixEng { get; set; }
        public string PersonFnameEng { get; set; }
        public string PersonLnameEng { get; set; }
        public string WorkStatus { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? EmployDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? DepartDate { get; set; }
        public string TaxId { get; set; }
        public string CitizenId { get; set; }
        public string SocialSecurityId { get; set; }
        public string Gender { get; set; }
        public string Mstatus { get; set; }
        public string OrgnameThai { get; set; }
        public string PositionCode { get; set; }
        public string PostnameThai { get; set; }
        public string EmptypeCode { get; set; }
        public string EmptypeDescThai { get; set; }
        public int? JlCode { get; set; }
        public string JlDescThai { get; set; }
        public string PlCode { get; set; }
        public string PlDescTha { get; set; }
        public string WorkplaceThai { get; set; }
        public DateTime? Dteupd { get; set; }
        public string CodeDev { get; set; }
        public string BkCode { get; set; }
        public string BkName { get; set; }
        public string BkAccount { get; set; }
    }
}
