using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SapPlantMapping
    {
        public string SrcName { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string SapCompCode { get; set; }
        public string SapPlant { get; set; }
        public string SapPcaCode { get; set; }
        public string SapCcaCode { get; set; }
        public string SapBusinessPlace { get; set; }
        public string ControlVersion { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
