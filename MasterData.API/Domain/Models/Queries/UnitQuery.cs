namespace MasterData.API.Domain.Models.Queries
{
    public class UnitQuery : Query
    {
        public string Keyword { get; set; }

        public UnitQuery(string keyword, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            Keyword = keyword;
        }
    }
}
