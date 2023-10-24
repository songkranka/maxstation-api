﻿using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class VInfSapOil02
    {
        public string SrcName { get; set; }
        public string PoNumber { get; set; }
        public string DelDate { get; set; }
        public string DelTime { get; set; }
        public decimal? ComfirmQty { get; set; }
        public string TrUom { get; set; }
        public string MatNr { get; set; }
        public string PlantNr { get; set; }
        public string SlocNr { get; set; }
        public decimal? Mcf { get; set; }
        public decimal? TestDen { get; set; }
        public decimal? TestTemp { get; set; }
        public string TestTempUom { get; set; }
        public decimal? MatTemp { get; set; }
        public string MatTempUom { get; set; }
        public string PoItemNo { get; set; }
    }
}
