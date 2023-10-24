namespace DailyOperation.API.Resources.Meter
{
    //public class PosAmountResource
    //{
    //    public string JournalId { get; set; }
    //    public decimal SaleQty { get; set; }
    //    public decimal GoodAmt { get; set; }
    //    public decimal TaxAmt { get; set; }
    //    public decimal DiscAmt { get; set; }
    //    public int MopCode { get; set; }
    //}

    public class PosAmountResource
    {
        public decimal GoodAmt { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal DiscAmt { get; set; }
        public int MopCode { get; set; }
    }
}
