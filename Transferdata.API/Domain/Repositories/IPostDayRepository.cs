using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Resources.Pos;
using Transferdata.API.Resources.PostDay;

namespace Transferdata.API.Domain.Repositories
{
    public interface IPostDayRepository
    {
        Task<DepositResponse> GetDepositAmt(PostDayQueryResource query);
    }
}
