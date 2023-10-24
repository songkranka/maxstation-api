using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class CompanyRepository : SqlDataAccessHelper, ICompanyRepository
    {
        public CompanyRepository(PTMaxstationContext context) : base(context)
        {

        }

        public async Task<List<MasCompany>> GetAll()
        {
            var response = new List<MasCompany>();
            response = await context.MasCompanies.AsNoTracking().Where(x => x.CompStatus == "Active" && x.IsLogin == "Y").ToListAsync();
            return response;
        }

        public async Task<MasCompany> FindCustomerCompanyById(string custCode)
        {
            var response = new MasCompany();
            response = await context.MasCompanies.AsNoTracking().FirstOrDefaultAsync(x => x.CustCode == custCode);
            return response;
        }
    }
}
