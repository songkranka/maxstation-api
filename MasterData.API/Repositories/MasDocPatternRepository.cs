using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MasterData.API.Repositories
{
    public class MasDocPatternRepository : SqlDataAccessHelper, IMasDocPatternRepository
    {
        public MasDocPatternRepository(PTMaxstationContext context) : base(context) { }

        public Task<List<MasDocPatternDt>> GetMasDocPatternDts(string docType)
        {
            var masDocPatternDts = (from dp in context.MasDocPatterns
                         join dt in context.MasDocPatternDts on dp.DocId equals dt.DocId
                         where dp.DocType == docType
                         select new MasDocPatternDt()
                         {
                             DocValue = dt.DocValue,
                             DocCode = dt.DocCode,
                             SeqNo = dt.SeqNo
                         }).ToListAsync();
            return masDocPatternDts;
        }
    }
}
