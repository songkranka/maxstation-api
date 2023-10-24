using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IOtherRepository
    {
        RespDocType GetPatternByDocType(OtherRequest req);
        List<MasMapping> GetMasMappingList(OtherRequest req);
    }
}
