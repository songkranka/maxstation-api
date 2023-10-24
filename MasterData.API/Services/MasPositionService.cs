using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class MasPositionService : IMasPositionService
    {
        private readonly IMasPositionRepository _masPositionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MasPositionService(
            IMasPositionRepository masPositionRepository,
            IUnitOfWork unitOfWork)
        {
            _masPositionRepository = masPositionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<MasPosition>> GetAll()
        {
            return await _masPositionRepository.GetAll();
        }
    }
}
