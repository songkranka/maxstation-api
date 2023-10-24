using MasterData.API.Domain.Repositories;
using MaxStation.Entities.Models;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PTMaxstationContext _context;

        public UnitOfWork(PTMaxstationContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
