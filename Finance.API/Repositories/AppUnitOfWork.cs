using Finance.API.Domain.Repositories;
using Finance.API.Persistence.Context;
using System.Threading.Tasks;

namespace Finance.API.Repositories
{
    public class AppUnitOfWork : IAppUnitOfWork
    {
        private readonly AppDbContext _context;

        public AppUnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
