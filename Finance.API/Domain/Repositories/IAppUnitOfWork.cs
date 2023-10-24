using System.Threading.Tasks;

namespace Finance.API.Domain.Repositories
{
    public interface IAppUnitOfWork
    {
        Task CompleteAsync();
    }
}
