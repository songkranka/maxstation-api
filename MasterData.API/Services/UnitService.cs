using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Repositories;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private IUnitOfWork _unitOfWork;

        public UnitService(
           IUnitRepository unitRepository,
           IUnitOfWork unitOfWork)
        {
            _unitRepository = unitRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueryResult<MasUnit>> List(UnitQuery pQuery)
        {
            return await _unitRepository.ListAsync(pQuery);
        }

        public async Task<MasUnit> GetMasUnitAsync(string unitId)
        {
            return await _unitRepository.GetMasUnitAsync(unitId);
        }

        public async Task<SaveUnitResponse> SaveUnitAsync(SaveUnitRequest request)
        {
            try
            {
                var masUnit = await _unitRepository.GetMasUnitAsync(request.UnitId);

                if (masUnit == null)
                {
                    masUnit = new MasUnit();
                    masUnit.UnitId = request.UnitId;
                    masUnit.MapUnitId = request.MapUnitId;
                    masUnit.UnitStatus = "Active";
                    masUnit.UnitName = request.UnitName;
                    masUnit.CreatedBy = request.CreatedBy;
                    masUnit.CreatedDate = DateTime.Now;

                    await _unitRepository.AddUnitAsync(masUnit);
                    await _unitOfWork.CompleteAsync();

                    
                }
                else
                {
                    masUnit.MapUnitId = request.MapUnitId;
                    masUnit.UnitName = request.UnitName;
                    masUnit.UpdatedBy = request.UpdatedBy;
                    masUnit.UpdatedDate = DateTime.Now;

                    await _unitRepository.UpdateUnitAsync(masUnit);
                    await _unitOfWork.CompleteAsync();
                }
                
                return new SaveUnitResponse(masUnit);

            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new SaveUnitResponse($"An error occurred when saving the unit: {strMessage}");
            }
        }

        public async Task<SaveUnitResponse> UpdateStatusAsync(SaveUnitRequest request)
        {
            var masUnit = await _unitRepository.GetMasUnitAsync(request.UnitId);

            if (masUnit != null)
            {
                masUnit.UpdatedBy = request.UpdatedBy;
                masUnit.UpdatedDate = DateTime.Now;
                masUnit.UnitStatus = request.UnitStatus;

                await _unitRepository.UpdateStatusAsync(masUnit);
                await _unitOfWork.CompleteAsync();
            }
            return new SaveUnitResponse(masUnit);
        }
    }
}
