using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class MasMappingService : IMasMappingService
    {
        private readonly IMasMappingRepository _masMappingitory;
        private readonly IUnitOfWork _unitOfWork;

        public MasMappingService(
            IMasMappingRepository masMappingitory,
            IUnitOfWork unitOfWork)
        {
            _masMappingitory = masMappingitory;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MasMapping>> GetMasMapping(string mapValue)
        {
            return await _masMappingitory.GetMasMapping(mapValue);
        }
    }
}
