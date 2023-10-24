using Inventory.API.Domain.Repositories;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
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
