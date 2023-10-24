using log4net;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using MaxStation.Utility.Helpers.CollectLogError;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class CostCenterService : ICostCenterService
    {
        private readonly ICostCenterRepository _costCenterRepository;
        private readonly ILogErrorService _logService;
        private readonly ILog _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;


        public CostCenterService(
            ICostCenterRepository costCenterRepository, IUnitOfWork unitOfWork, ILogErrorService logService, IHttpContextAccessor contextAccessor)
        {
            _costCenterRepository = costCenterRepository;
            _logService = logService;
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(CostCenterService));
        }

        public List<MasCostCenter> GetCostCenterList(CostCenterRequest req)
        {
            return _costCenterRepository.GetCostCenterList(req);
        }

        public async Task<QueryResult<MasCostCenter>> ListAsync(CosCenterQuery query)
        {
            return await _costCenterRepository.ListAsync(query);
        }

        public async Task<MasCostCenter> GetCostCenterByGuid(Guid guid)
        {
            return await _costCenterRepository.GetCostCenterByGuid(guid);
        }

        public async Task<SaveCostCenterResponse> SaveCostCenterAsync(MasCostCenterRequest request, string host, string path, string method)
        {
            try
            {
                var response = new MasCostCenter();
                var costCenterResult = await _costCenterRepository.GetMasConstCenterFromBranCodeAndCompCode(request.MasCostCenter.CompCode, request.MasCostCenter.BrnCode);
                
                if (costCenterResult == null)
                {
                    var costCenter = new MasCostCenter();
                    costCenter.CompCode = request.MasCostCenter.CompCode;
                    costCenter.BrnCode = request.MasCostCenter.BrnCode;
                    //costCenter.MapBrnCode = request.MasCostCenter.MapBrnCode;
                    costCenter.CostCenter = request.MasCostCenter.CostCenter;
                    costCenter.ProfitCenter = request.MasCostCenter.ProfitCenter;
                    costCenter.BrnStatus = "Active";
                    costCenter.BrnName = request.MasCostCenter.BrnName;
                    costCenter.CreatedBy = request.MasCostCenter.CreatedBy;
                    costCenter.CreatedDate = DateTime.Now;

                    await _costCenterRepository.AddCostCenterAsync(costCenter);
                    await _unitOfWork.CompleteAsync();
                    response = costCenter;
                    return new SaveCostCenterResponse(response);
                }
                else
                {
                    //costCenterResult.MapBrnCode = request.MasCostCenter.MapBrnCode;
                    costCenterResult.BrnName = request.MasCostCenter.BrnName;
                    costCenterResult.CostCenter = request.MasCostCenter.CostCenter;
                    costCenterResult.ProfitCenter = request.MasCostCenter.ProfitCenter;
                    costCenterResult.UpdatedBy = request.MasCostCenter.UpdatedBy;
                    costCenterResult.UpdatedDate = DateTime.Now;

                    await _costCenterRepository.UpdateCostCenterAsync(costCenterResult);
                    response = costCenterResult;
                    await _unitOfWork.CompleteAsync();
                }
                
                return new SaveCostCenterResponse(response);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                ThreadContext.Properties["log_status"] = "Error";
                ThreadContext.Properties["comp_code"] = request.MasCostCenter.CompCode;
                ThreadContext.Properties["brn_code"] = request.MasCostCenter.BrnCode;
                ThreadContext.Properties["loc_code"] = null;
                ThreadContext.Properties["json_data"] = JsonConvert.SerializeObject(request);
                ThreadContext.Properties["create_by"] = _contextAccessor.HttpContext.User.Identity.Name;
                ThreadContext.Properties["stack_trace"] = ex.StackTrace;
                ThreadContext.Properties["path"] = path;
                ThreadContext.Properties["method"] = method;
                ThreadContext.Properties["host"] = host;
                _log.Error(ex);
                return new SaveCostCenterResponse($"An error occurred when saving the cost center: {strMessage}");
            }
        }


        public async Task<CostCenterResponse> UpdateCostCenterAsync(MasCostCenterRequest request, string host, string path, string method)
        {
            try
            {
                //var result = new MasCostCenter();
                //MasCostCenter model = await _costCenterRepository.CheckMasCostCenter(request.MasCostCenter);

                //if (model == null)
                //{
                //    await SaveLogError("UpdateCostCenter", new Exception("CompCode, BrnCode, BrnName and MapBrnCode does not exist"), JsonConvert.SerializeObject(request), request.MasCostCenter);
                //    await _unitOfWork.CompleteAsync();
                //    return new CostCenterResponse("CompCode, BrnCode, BrnName and MapBrnCode does not exist");
                //}

                var response = new MasCostCenter();
                var costCenterResult = await _costCenterRepository.GetMasConstCenterFromBranCodeAndCompCode(request.MasCostCenter.CompCode, request.MasCostCenter.BrnCode);
                
                if (costCenterResult == null)
                {
                    await SaveLogError("UpdateCostCenter", new Exception("CompCode, BrnCode, BrnName and MapBrnCode does not exist"), JsonConvert.SerializeObject(request), request.MasCostCenter);
                    await _unitOfWork.CompleteAsync();
                    return new CostCenterResponse("CompCode, BrnCode, BrnName and MapBrnCode does not exist");
                }
                else
                {
                    costCenterResult.BrnStatus = "Cancel";
                    costCenterResult.UpdatedBy = request.MasCostCenter.UpdatedBy;
                    costCenterResult.UpdatedDate = DateTime.Now;
                }

                await _costCenterRepository.UpdateCostCenterAsync(costCenterResult);
                await _unitOfWork.CompleteAsync();
                response = costCenterResult;
                return new CostCenterResponse(response);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                ThreadContext.Properties["log_status"] = "Error";
                ThreadContext.Properties["comp_code"] = request.MasCostCenter.CompCode;
                ThreadContext.Properties["brn_code"] = request.MasCostCenter.BrnCode;
                ThreadContext.Properties["loc_code"] = null;
                ThreadContext.Properties["json_data"] = JsonConvert.SerializeObject(request);
                ThreadContext.Properties["create_by"] = _contextAccessor.HttpContext.User.Identity.Name;
                ThreadContext.Properties["stack_trace"] = ex.StackTrace;
                ThreadContext.Properties["path"] = path;
                ThreadContext.Properties["method"] = method;
                ThreadContext.Properties["host"] = host;

                _log.Error(ex);
                return new CostCenterResponse($"An error occurred when saving the cost center: {strMessage}");
            }
        }

        public async Task SaveLogError(string logStatus, Exception exception, string jsonData, MasCostCenter request)
        {
            try
            {
                LogError logError = await _logService.ConvertLogError(logStatus, request.CompCode, request.BrnCode, null, jsonData, exception.Message, request.CreatedDate, request.CreatedBy, null, "POST", null, exception.StackTrace);
                await _costCenterRepository.SaveLogError(logError);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
            }
        }
    }
}