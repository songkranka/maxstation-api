using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class OtherRepository : SqlDataAccessHelper, IOtherRepository
    {
        public OtherRepository(PTMaxstationContext context) : base(context)
        {

        }

        public List<MasMapping> GetMasMappingList(OtherRequest req)
        {
            return context.MasMappings.Where(x => x.MapValue == req.Keyword).ToList();
        }

        public RespDocType GetPatternByDocType(OtherRequest req)
        {
            RespDocType resp = new RespDocType();
            var masDocPattern = this.context.MasDocPatterns.FirstOrDefault(x => x.DocType == req.DocType);
            if (masDocPattern != null)
            {
                var docId = masDocPattern.DocId.ToString();
                var masDocPatternDt = this.context.MasDocPatternDts.Where(y => y.DocId == docId).OrderBy(z => z.SeqNo).ToList();

                resp.DocType = req.DocType;
                resp.MasDocPattern = masDocPattern;
                resp.MasDocPattern.MasDocPatternDt = masDocPatternDt;
            }
            return resp;
        }
    }
}
