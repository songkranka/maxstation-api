#nullable disable

using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class MasDocPattern
    {
        public MasDocPattern()
        {
            MasDocPatternDt = new List<MasDocPatternDt>();
        }

        [NotMapped]
        public List<MasDocPatternDt> MasDocPatternDt { get; set; }
    }
}
