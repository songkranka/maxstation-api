namespace MasterData.API.Resources.Reason
{
    public class ProductReasonQueryResource : QueryResource
    {
        public string Keyword { get; set; }
        public string PdId { get; set; }
        public string ReasonGroup { get; set; }
    }
}
