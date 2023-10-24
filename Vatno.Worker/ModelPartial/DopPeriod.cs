using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class DopPeriod
    {
        public DopPeriod()
        {
            DopPeriodEmp = new HashSet<DopPeriodEmp>();
            DopPeriodMeter = new HashSet<DopPeriodMeter>();
            DopPeriodTank = new HashSet<DopPeriodTank>();
            DopPeriodTankSum = new HashSet<DopPeriodTankSum>();
            DopPeriodCash = new HashSet<DopPeriodCash>();
        }

        [NotMapped]
        public DopPeriodCashSum DopPeriodCashSum { get; set; }

        [NotMapped]
        public ICollection<DopPeriodEmp> DopPeriodEmp { get; set; }

        [NotMapped]
        public ICollection<DopPeriodMeter> DopPeriodMeter { get; set; }
        [NotMapped]
        public ICollection<DopPeriodTank> DopPeriodTank { get; set; }
        [NotMapped]
        public ICollection<DopPeriodTankSum> DopPeriodTankSum { get; set; }
        [NotMapped]
        public ICollection<DopPeriodCash> DopPeriodCash { get; set; }


    }
}
