using System.Threading.Tasks;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using PTWebService;

namespace MasterData.API.Domain.Services
{
    public interface ISoapPTWebServiceApi
    {
        Task<PTWebServiceSoapClient> GetInstanceAsync();
        Task<OilPrice> GetOilPriceByBrn(OilPriceRequest request);
    }
}
