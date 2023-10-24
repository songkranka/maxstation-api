using System.Threading.Tasks;

namespace PostDay.API.Domain.Repositories
{
    public interface ICardStockRepository
    {
        Task<string> GetSourceValue(string brnCode, string compCode);
        Task<string> GetCompCode(string companyCode);
    }
}
