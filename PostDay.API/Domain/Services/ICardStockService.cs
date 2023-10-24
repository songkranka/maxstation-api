using PostDay.API.Domain.Models.PostDay;
using System.Threading.Tasks;
using PostDay.API.Services;

namespace PostDay.API.Domain.Services
{
    public interface ICardStockService
    {
        Task<ResultModel> GetCardStock(CardStockRequest request);
    }
}
