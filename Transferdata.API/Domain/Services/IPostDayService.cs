using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Resources.Pos;
using Transferdata.API.Resources.PostDay;

namespace Transferdata.API.Domain.Services
{
    public interface IPostDayService
    {
        Task<DepositResponse> GetDepositAmt(PostDayQueryResource query);
    }
}
