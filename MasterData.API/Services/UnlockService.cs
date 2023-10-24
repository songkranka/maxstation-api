using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Domain.Services.Communication;
using MasterData.API.Resources.Unlock;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class UnlockService : IUnlockService
    {
        private readonly IUnlockRepository _unlockRepository;
        private readonly IMasBranchConfigRepository _masBranchConfigRepository;
        private readonly IMasControlRepository _masControlRepository;
        private IUnitOfWork _unitOfWork;

        public UnlockService(
            IUnlockRepository unlockRepository,
            IMasBranchConfigRepository masBranchConfigRepository,
            IMasControlRepository masControlRepository,
            IUnitOfWork pUnitOfWork)
        {
            _unlockRepository = unlockRepository;
            _masBranchConfigRepository = masBranchConfigRepository;
            _masControlRepository = masControlRepository;
            _unitOfWork = pUnitOfWork;
        }

        public async Task<List<EmployBranchConfigResponse>> GetEmployeeBranchConfig(EmployeeBranchConfigRequest req)
        {
            var _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            var employBranchConfigs = await _unlockRepository.GetEmployeeBranchConfig(req);

            if (employBranchConfigs != null)
            {
                var masBranchConfig = await _unlockRepository.GetIsLockMasBranchConfig(req.CompCode, req.BrnCode);
                var masControl =  _masControlRepository.GetMasControl(new MasControlRequest { CompCode = req.CompCode, BrnCode = req.BrnCode, CtrlCode = "WDATE" });
                var docDate = (masControl == null) ? DateTime.Now : DateTime.ParseExact(masControl.CtrlValue, "dd/MM/yyyy", _curCulture);
                var sysBranchConfigs = await _unlockRepository.GetSysBranchConfigListAsync(req.CompCode, req.BrnCode, req.DocDate);

                foreach (var employBranchConfig in employBranchConfigs)
                {
                    if(masBranchConfig != null)
                    {
                        if (employBranchConfig.ConfigId == "IS_POS")
                        {
                            employBranchConfig.IsLock = masBranchConfig.IsLockMeter;
                        }
                        else if(employBranchConfig.ConfigId == "IS_LOCK_METER")
                        {
                            employBranchConfig.IsLock = masBranchConfig.IsLockMeter;
                        }
                        else if (employBranchConfig.ConfigId == "IS_LOCK_SLIP")
                        {
                            employBranchConfig.IsLock = masBranchConfig.IsLockSlip;
                        }
                    }
                    else
                    {
                        employBranchConfig.IsLock = "Y";
                    }

                    if(employBranchConfig.IsLockDate == "Y")
                    {
                        employBranchConfig.StartDate = docDate;
                        employBranchConfig.EndDate = docDate;
                    }
                    else
                    {
                        var sysBranchConfig = sysBranchConfigs.OrderByDescending(x => x.SeqNo).FirstOrDefault(x => x.ConfigId == employBranchConfig.ConfigId);
                        
                        if (sysBranchConfig != null)
                        {
                            if (sysBranchConfig.EndDate.Value.Date < docDate.Date)
                            {
                                employBranchConfig.EndDate = docDate;
                            }
                            else
                            {
                                employBranchConfig.EndDate = sysBranchConfig.EndDate.Value.Date;
                            }

                            employBranchConfig.StartDate = docDate;
                            employBranchConfig.Remark = sysBranchConfig.Remark;
                        }
                        else
                        {
                            employBranchConfig.Remark = string.Empty;
                            employBranchConfig.StartDate = docDate;
                            employBranchConfig.EndDate = docDate;
                        }
                    }
                }
            }

            return employBranchConfigs;
        }

        public async Task<SysBranchConfigResponse> GetSysBranchConfig(SysBranchConfigRequest req)
        {
            return await _unlockRepository.GetSysBranchConfig(req);
        }

        public async Task<UnlockResponse> SaveUnlockAsync(SaveUnlockResource request)
        {
            try
            {
                await _unlockRepository.AddUnlockAsync(request);
                var masBranchConfig = await _masBranchConfigRepository.GetMasBranchConfig(request.CompCode, request.BrnCode);
                var isLockMeter = request._Unlock.FirstOrDefault(x => x.ConfigId == "IS_LOCK_METER");
                var isLockSlip = request._Unlock.FirstOrDefault(x => x.ConfigId == "IS_LOCK_SLIP");

                if (masBranchConfig == null)
                {
                    masBranchConfig.CompCode = request.CompCode;
                    masBranchConfig.BrnCode = request.BrnCode;
                    masBranchConfig.IsPos = "N";
                    masBranchConfig.IsLockMeter =  (isLockMeter == null) ? masBranchConfig.IsLockMeter : isLockMeter.IsLock;
                    masBranchConfig.IsLockSlip = (isLockSlip == null)? masBranchConfig.IsLockSlip : isLockSlip.IsLock;
                    masBranchConfig.CreatedDate = DateTime.Now;
                    masBranchConfig.CreatedBy = request.EmpCode;

                    await _masBranchConfigRepository.AddMasBranchConfigAsync(masBranchConfig);
                }
                else
                {
                    //await _masBranchConfigRepository.DeleteMasBranchConfigAsync(request.CompCode, request.BrnCode);

                    //masBranchConfig.CompCode = masBranchConfig.CompCode;
                    //masBranchConfig.BrnCode = masBranchConfig.BrnCode;
                    //masBranchConfig.IsPos = masBranchConfig.IsPos;
                    //masBranchConfig.IsLockMeter = (isLockMeter == null) ? masBranchConfig.IsLockMeter : isLockMeter.IsLock;
                    //masBranchConfig.IsLockSlip = (isLockSlip == null) ? masBranchConfig.IsLockSlip : isLockSlip.IsLock;
                    //masBranchConfig.CreatedDate = DateTime.Now;
                    //masBranchConfig.CreatedBy = request.EmpCode;

                    //await _masBranchConfigRepository.AddMasBranchConfigAsync(masBranchConfig);

                    masBranchConfig.IsLockMeter = (isLockMeter == null) ? masBranchConfig.IsLockMeter : isLockMeter.IsLock;
                    masBranchConfig.IsLockSlip = (isLockSlip == null) ? masBranchConfig.IsLockSlip : isLockSlip.IsLock;
                    masBranchConfig.UpdatedDate = DateTime.Now;
                    masBranchConfig.UpdatedBy = request.EmpCode;

                    await _masBranchConfigRepository.UpdateMasBranchConfigAsync(masBranchConfig);
                }

                await _unitOfWork.CompleteAsync();
                return new UnlockResponse(request);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new UnlockResponse($"An error occurred when saving the unlock: {strMessage}");
            }
        }
    }
}
