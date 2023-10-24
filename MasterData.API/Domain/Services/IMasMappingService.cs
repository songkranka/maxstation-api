﻿using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IMasMappingService
    {
        Task<List<MasMapping>> GetMasMapping(string mapValue);
    }
}
