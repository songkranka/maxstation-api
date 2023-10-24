namespace MasterData.API.Domain.Models.Queries
{
    public class ProductReasonQuery : Query
    {
        public string Keyword { get; set; }
        public string PdId { get; set; }
        public string ReasonGroup { get; set; }
        public ProductReasonQuery(string keyword, string pdId, string reasonGroup, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            Keyword = keyword;
            PdId = pdId;
            ReasonGroup = reasonGroup;
        }
    }
}
