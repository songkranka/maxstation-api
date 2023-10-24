#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InfPoItem
    {
        public string PoNumber { get; set; }
        public string PoItem { get; set; }
        public string DeleteInd { get; set; }
        public string ShortText { get; set; }
        public string Material { get; set; }
        public string Plant { get; set; }
        public string StgeLoc { get; set; }
        public string MatlGroup { get; set; }
        public string InfoRec { get; set; }
        public decimal? Quantity { get; set; }
        public string PoUnit { get; set; }
        public string OrderprUn { get; set; }
        public decimal? NetPrice { get; set; }
        public int? PriceUnit { get; set; }
        public string TaxCode { get; set; }
        public decimal? OverDlvTol { get; set; }
        public decimal? UnderDlvTol { get; set; }
        public string ValType { get; set; }
        public string NoMoreGr { get; set; }
        public string ItemCat { get; set; }
        public string Acctasscat { get; set; }
        public string FreeItem { get; set; }
        public string RetItem { get; set; }
        public int? PlanDel { get; set; }
        public string RfqItem { get; set; }
        public string PreqItem { get; set; }
        public decimal? RetentionPercentage { get; set; }
        public decimal? DownpayAmount { get; set; }
        public decimal? DownpayPercent { get; set; }
        public DateTime? DownpayDuedate { get; set; }
        public string PoItem1 { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name4 { get; set; }
        public string COName { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string CityNo { get; set; }
        public string PostlCod1 { get; set; }
        public string PostlCod2 { get; set; }
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public string HouseNo { get; set; }
        public string StrSuppl1 { get; set; }
        public string StrSuppl2 { get; set; }
        public string Location { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string RoomNo { get; set; }
        public string Country { get; set; }
        public string Langu { get; set; }
        public string Region { get; set; }
        public string Sort1 { get; set; }
        public string Sort2 { get; set; }
        public string Transpzone { get; set; }
        public string EMail { get; set; }
        public string PoItem2 { get; set; }
        public string SchedLine { get; set; }
        public string DelDatcatExt { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DelivTime { get; set; }
        public DateTime? StatDate { get; set; }
        public string PreqNo { get; set; }
        public string PreqItem2 { get; set; }
        public DateTime? PoDate { get; set; }
        public string IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string InfStatus { get; set; }
        public string ErrorMsg { get; set; }
        public decimal? ReceiveQty { get; set; }
        public decimal? ReceiveFloor { get; set; }
        public DateTime? ReceiveUpdate { get; set; }
    }
}
