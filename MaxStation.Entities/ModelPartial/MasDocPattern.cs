using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace MaxStation.Entities.Models
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
