using System.Net.Http;
using System.Threading.Tasks;

namespace Finance.API.Domain.Services
{
    public interface IHttpClientService
    {
        Task<TResult> PostAsync<TRequest, TResult>(string jwt, string url, TRequest t);
    }
}
