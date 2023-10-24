using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace MasterData.API.Repositories
{
    public class DocpatternRepository :SqlDataAccessHelper, IDocpatternRepository
    {
        public DocpatternRepository(PTMaxstationContext context) : base(context) { }

        public async Task<List<MasDocPatternDt>> FindDocPatternDtByDocTypeAsync(string docType)
        {
            var qryDocPattern = (from dp in context.MasDocPatterns
                                 join dt in context.MasDocPatternDts on dp.DocId equals dt.DocId
                                 where dp.DocType == docType
                                 select new MasDocPatternDt()
                                 {
                                     DocValue = dt.DocValue,
                                     DocCode = dt.DocCode,
                                     SeqNo = dt.SeqNo
                                 }).AsQueryable();
            var response = await qryDocPattern.ToListAsync();
            return response;
        }
    }
}
