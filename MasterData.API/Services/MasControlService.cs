using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class MasControlService : IMasControlService
    {
        private readonly IMasControlRepository _masControlRepository;
        private IUnitOfWork _unitOfWork = null;

        public MasControlService(
            IMasControlRepository masControlRepository,
            IUnitOfWork unitOfWork)
        {
            _masControlRepository = masControlRepository;
            _unitOfWork = unitOfWork;
        }

        public MasControl GetMasControl(MasControlRequest req)
        {
            MasControl resp = _masControlRepository.GetMasControl(req);
            return resp;
        }

        public async Task<UpdateCtrlValueDateResponse> UpdateCtrlValueAsync(UpdateCtrlValueDateRequest request)
        {
            try
            {
                var response = await _masControlRepository.UpdateCtrlValueAsync(request);
                await _unitOfWork.CompleteAsync();
                return new UpdateCtrlValueDateResponse(response);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new UpdateCtrlValueDateResponse($"An error occurred when update the mascontrol: {strMessage}");
            }
        }

    }
}
