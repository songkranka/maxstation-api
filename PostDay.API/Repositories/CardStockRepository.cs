using MaxStation.Entities.Models;
using PostDay.API.Domain.Repositories;
using PostDay.API.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PostDay.API.Repositories
{
    public class CardStockRepository : SqlDataAccessHelper, ICardStockRepository
    {
        public CardStockRepository(PTMaxstationContext context) : base(context)
        {
        }


        public async Task<string> GetSourceValue(string brnCode, string compCode)
        {
            var sourceValue = await context.DopFormulaBranches
                .Join(
                    context.DopFormulas,
                    dopFormulaBranch => dopFormulaBranch.FmNo,
                    dopFormula => dopFormula.FmNo,
                    (dopFormulaBranch, dopFormula) => new { brn = dopFormulaBranch, formula = dopFormula }
                )
                .Where(a => a.brn.BrnCode == brnCode && a.brn.CompCode == compCode && a.formula.FmName == "Maxplus")
                .Select(a => a.formula.SourceValue)
                .AsNoTracking()
                .ToListAsync();
            return sourceValue.FirstOrDefault();
        }

        public async Task<string> GetCompCode(string companyCode)
        {
            var compCode = await context.MasCompanies
                .Where(x => x.CompSname == companyCode)
                .Select(x => x.CompCode)
                .AsNoTracking()
                .ToListAsync();
            return compCode.FirstOrDefault();
        }
    }

}
