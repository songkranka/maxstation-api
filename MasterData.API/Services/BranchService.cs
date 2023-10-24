using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.Product;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IEmployeeAuthenRepository _employeeAuthenRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductUnitRepository _productUnitRepository;
        private IUnitOfWork _unitOfWork = null;

        public BranchService(
            IBranchRepository branchRepository,
            IEmployeeAuthenRepository employeeAuthenRepository,
            IProductRepository productRepository,
            IProductUnitRepository productUnitRepository,
            IUnitOfWork pUnitOfWork)
        {
            _branchRepository = branchRepository;
            _employeeAuthenRepository = employeeAuthenRepository;
            _productRepository = productRepository;
            _productUnitRepository = productUnitRepository;
            _unitOfWork = pUnitOfWork;
        }

        public List<MasBranch> GetBranchList(BranchRequest req)
        {
            List<MasBranch> branchList = new List<MasBranch>();
            branchList = _branchRepository.GetBranchList(req);
            return branchList;
        }

        public List<MasBranch> GetAuthBranchList(AuthBranchRequest req)
        {
            List<MasBranch> branchList = new List<MasBranch>();
            branchList = _branchRepository.GetAuthBranchList(req);
            return branchList;
        }
        public async Task<QueryResult<MasBranch>> List(BranchQuery pQuery)
        {
            return await _branchRepository.List(pQuery);
        }
        public List<MasCompany> getCompanyDDL()
        {
            List<MasCompany> branchList = new List<MasCompany>();
            branchList = _branchRepository.getCompanyDDL();
            return branchList;
        }

        public async Task<MasCompany> getCompany(string CompCode)
        {
            return await _branchRepository.getCompany(CompCode);
        }

        public async Task<MasBranch> GetBranchByGuid(Guid guid)
        {
            return await _branchRepository.GetBranchByGuid(guid);
        }

        public async Task<MasBranch> getBranchDetail(string brnCode)
        {
            return await _branchRepository.getBranchDetail(brnCode);
        }

        public async Task<MasBranch> GetBranchDetailByCompCodeAndBrnCode(string CompCode, string BrnCode)
        {
            return await _branchRepository.GetBranchDetailByCompCodeAndBrnCode(CompCode, BrnCode);
        }

        public List<MasBranchTank> getTankByBranch(string CompCode, string BrnCode)
        {
            List<MasBranchTank> branchTank = new List<MasBranchTank>();
            branchTank = _branchRepository.getTankByBranch(CompCode, BrnCode);
            return branchTank;
        }

        public List<MasBranchDisp> getDispByBranch(string CompCode, string BrnCode)
        {
            List<MasBranchDisp> branchDisp = new List<MasBranchDisp>();
            branchDisp = _branchRepository.getDispByBranch(CompCode, BrnCode);
            return branchDisp;
        }

        public List<MasBranchTax> getTaxByBranch(string CompCode, string BrnCode)
        {
            List<MasBranchTax> branchTax = new List<MasBranchTax>();
            branchTax = _branchRepository.getTaxByBranch(CompCode, BrnCode);
            return branchTax;
        }

        public List<MasBranch> GetAllBranchByCompCode(string CompCode)
        {
            return _branchRepository.GetAllBranchByCompCode(CompCode);
        }

        public async Task SaveBranch(MasBranch param)
        {
            _branchRepository.SaveBranch(param);
            await _unitOfWork.CompleteAsync();
        }
        public Task<QueryResult<MeterResponse>> GetMasBranchDISP(BranchMeterRequest req)
        {
            return _branchRepository.GetMasBranchDISP(req);
        }

        public async Task<MasBranchConfig> GetMasBranchConfig(string pStrCompCode, string pStrBrnCode)
        {
            return await _branchRepository.GetMasBranchConfig(pStrCompCode, pStrBrnCode);
        }

        public async Task<List<MasBranch>> GetBranchEmployeeAuth(BranchEmployeeAuthRequest req)
        {
            var response = new List<MasBranch>();
            var empAuth = await _employeeAuthenRepository.FindByEmpCodeAsync(req.EmpCode);

            if (empAuth == null)
            {
                var branchs = _branchRepository.FindBranchByCompCode(req.CompCode);
                response.AddRange(branchs);
            }
            else
            {
                switch (empAuth.AuthCode)
                {
                    case 0:
                        var branch = _branchRepository.FindBranchByBrnCode(req.BrnCode);
                        response.Add(branch);
                        break;
                    case 1:
                        var branchs = _branchRepository.FindBranchByCompCode(req.CompCode);
                        response.AddRange(branchs);
                        break;
                    default:
                        var branchAuths = _branchRepository.FindBranchByAuthCode(req.CompCode, empAuth.AuthCode);
                        response.AddRange(branchAuths);
                        break;
                }
            }

            return response;
        }

        public async Task<SaveBranchResponse> SaveBranchAsync(SaveBranchRequest request)
        {
            try
            {
                var response = new MasBranch();

                var masBranchConfig = await _branchRepository.GetMasBranchConfig(request.MasBranchConfig.CompCode, request.MasBranchConfig.BrnCode);

                if (masBranchConfig == null)
                {
                    var branchConfig = new MasBranchConfig();
                    branchConfig.CompCode = request.MasBranchConfig.CompCode;
                    branchConfig.BrnCode = request.MasBranchConfig.BrnCode;
                    branchConfig.IsPos = request.MasBranchConfig.IsPos;
                    branchConfig.IsLockMeter = request.MasBranchConfig.IsLockMeter;
                    branchConfig.IsLockSlip = request.MasBranchConfig.IsLockSlip;
                    branchConfig.Trader = request.MasBranchConfig.Trader;
                    branchConfig.TraderPosition = request.MasBranchConfig.TraderPosition;
                    branchConfig.ReportTaxType = request.MasBranchConfig.ReportTaxType;
                    branchConfig.CreatedDate = DateTime.Now;
                    branchConfig.CreatedBy = request.MasBranchConfig.UpdatedBy;

                    await _branchRepository.AddBranchConfigAsync(branchConfig);
                }

                var masBranch = await _branchRepository.FindByGuidAsync(request.MasBranch.Guid);

                if (masBranch == null)
                {
                    var createBranch = new MasBranch()
                    {
                        CompCode = request.MasBranch.CompCode,
                        BrnCode = request.MasBranch.BrnCode,
                        MapBrnCode = request.MasBranch.MapBrnCode,
                        LocCode = request.MasBranch.LocCode,
                        BrnName = request.MasBranch.BrnName,
                        BrnStatus = request.MasBranch.BrnStatus,
                        BranchNo = request.MasBranch.BranchNo,
                        Address = request.MasBranch.Address,
                        SubDistrict = request.MasBranch.SubDistrict,
                        District = request.MasBranch.District,
                        ProvCode = request.MasBranch.ProvCode,
                        Province = request.MasBranch.Province,
                        Postcode = request.MasBranch.Postcode,
                        Phone = request.MasBranch.Phone,
                        Fax = request.MasBranch.Fax,
                        PosCount = request.MasBranch.PosCount,
                        CloseDate = request.MasBranch.CloseDate,
                        CreatedDate = DateTime.Now,
                        Guid = Guid.NewGuid()
                    };

                    await _branchRepository.AddBranchAsync(createBranch);
                    response = createBranch;
                }
                else
                {
                    masBranch.MapBrnCode = request.MasBranch.MapBrnCode;
                    masBranch.BrnName = request.MasBranch.BrnName;
                    masBranch.BrnStatus = request.MasBranch.BrnStatus;
                    masBranch.BranchNo = request.MasBranch.BranchNo;
                    masBranch.Address = request.MasBranch.Address;
                    masBranch.SubDistrict = request.MasBranch.SubDistrict;
                    masBranch.District = request.MasBranch.District;
                    masBranch.ProvCode = request.MasBranch.ProvCode;
                    masBranch.Province = request.MasBranch.Province;
                    masBranch.Postcode = request.MasBranch.Postcode;
                    masBranch.Phone = request.MasBranch.Phone;
                    masBranch.Fax = request.MasBranch.Fax;
                    masBranch.PosCount = request.MasBranch.PosCount;
                    masBranch.CloseDate = request.MasBranch.CloseDate;
                    masBranch.UpdatedDate = DateTime.Now;

                    await _branchRepository.UpdateBranchAsync(masBranch);
                    response = masBranch;
                }

                await _unitOfWork.CompleteAsync();
                return new SaveBranchResponse(response);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new SaveBranchResponse($"An error occurred when saving the branch: {strMessage}");
            }
        }

        public async Task<SaveBranchTankResponse> SaveBranchTankAsync(SaveBranchTankRequest requests)
        {
            try
            {
                var response = new BranchTankResponse();
                var tankRequest = requests.MasBranchTanks.FirstOrDefault();
                var masBranchTanks = await _branchRepository.FindByCompCodeAndBrnCodeAsync(tankRequest.CompCode, tankRequest.BrnCode);

                if(masBranchTanks != null)
                {
                    await _branchRepository.DeleteBranchTankAsync(tankRequest.CompCode, tankRequest.BrnCode);
                }

                foreach (var request in requests.MasBranchTanks)
                {
                    var branchTankResponse = new List<MasBranchTank>();
                    var saveMasBranchTank = new MasBranchTank()
                    {
                        CompCode = request.CompCode,
                        BrnCode = request.BrnCode,
                        TankId = request.TankId,
                        TankStatus = request.TankStatus,
                        PdId = request.PdId,
                        PdName = request.PdName,
                        Capacity = request.Capacity,
                        CapacityMin = request.CapacityMin,
                        CreatedDate = DateTime.Now,
                        CreatedBy = request.CreatedBy
                    };

                    await _branchRepository.AddBranchTankAsync(saveMasBranchTank);
                    branchTankResponse.Add(saveMasBranchTank);
                    response.MasBranchTanks = branchTankResponse;
                }

                var masBranchDisps = await _branchRepository.FindMasBranchDispByCompCodeAndBrnCodeAsync(tankRequest.CompCode, tankRequest.BrnCode);

                if (masBranchDisps != null)
                {
                    await _branchRepository.DeleteBranchDispAsync(tankRequest.CompCode, tankRequest.BrnCode);
                }

                foreach (var request in requests.MasBranchDisps)
                {
                    var productResource = new ProductResource();
                    productResource.PdId = request.PdId;
                    var masProduct = await _productRepository.FindByIdAsync(productResource);
                    var masProductUnit = await _productUnitRepository.FindByProductId(request.PdId);
                    var branchDispResponse = new List<MasBranchDisp>();
                    var saveMasBranchDisp = new MasBranchDisp()
                    {
                        CompCode = request.CompCode,
                        BrnCode = request.BrnCode,
                        DispId = request.DispId,
                        DispStatus = request.DispStatus,
                        MeterMax = request.MeterMax,
                        SerialNo = request.SerialNo,
                        TankId = request.TankId,
                        PdId = request.PdId,
                        PdName = masProduct == null ? string.Empty : masProduct.PdName,
                        UnitId = masProductUnit == null ? string.Empty : masProductUnit.UnitId,
                        UnitBarcode = masProductUnit == null ? string.Empty : masProductUnit.UnitBarcode,
                        HoseId = request.HoseId,
                        CreatedDate = DateTime.Now,
                        CreatedBy = request.CreatedBy
                    };

                    await _branchRepository.AddBranchDispAsync(saveMasBranchDisp);
                    branchDispResponse.Add(saveMasBranchDisp);
                    response.MasBranchDisps = branchDispResponse;
                }

                await _unitOfWork.CompleteAsync();
                return new SaveBranchTankResponse(response);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new SaveBranchTankResponse($"An error occurred when saving the branch tank: {strMessage}");
            }
        }

        public async Task<SaveBranchTaxResponse> SaveBranchTaxAsync(SaveBranchTaxRequest requests)
        {
            try
            {
                var response = new BranchTaxResponse();

                var masBranchConfig = await _branchRepository.GetMasBranchConfig(requests.MasBranchConfig.CompCode, requests.MasBranchConfig.BrnCode); 

                if(masBranchConfig != null)
                {
                    var branchConfig = new MasBranchConfig();
                    branchConfig.CompCode = masBranchConfig.CompCode;
                    branchConfig.BrnCode = masBranchConfig.BrnCode;
                    branchConfig.Trader = requests.MasBranchConfig.Trader;
                    branchConfig.TraderPosition = requests.MasBranchConfig.TraderPosition;
                    branchConfig.ReportTaxType = requests.MasBranchConfig.ReportTaxType;
                    branchConfig.UpdatedDate = DateTime.Now;
                    branchConfig.UpdatedBy = requests.MasBranchConfig.UpdatedBy;
                    await _branchRepository.UpdateBranchConfigAsync(branchConfig);
                }

                var masBranchTax = await _branchRepository.FindByCompCodeAndBrnCodeAndTaxIdAsync(requests.MasBranchConfig.CompCode, requests.MasBranchConfig.BrnCode);

                if (masBranchTax == null)
                {
                    foreach (var request in requests.MasBranchTaxs)
                    {
                        var branchTaxResponse = new List<MasBranchTax>();
                        var saveMasBranchTax = new MasBranchTax()
                        {
                            CompCode = request.CompCode,
                            BrnCode = request.BrnCode,
                            TaxId = request.TaxId,
                            TaxName = request.TaxName,
                            TaxAmt = request.TaxAmt,
                            CreatedDate = DateTime.Now,
                            CreatedBy = request.CreatedBy
                        };

                        await _branchRepository.AddBranchTaxAsync(saveMasBranchTax);
                        branchTaxResponse.Add(saveMasBranchTax);
                        response.MasBranchTaxs = branchTaxResponse;

                    }
                }
                else
                {
                    await _branchRepository.DeleteBranchTaxAsync(requests.MasBranchConfig.CompCode, requests.MasBranchConfig.BrnCode);

                    foreach (var request in requests.MasBranchTaxs)
                    {
                        var branchTaxResponse = new List<MasBranchTax>();
                        var saveMasBranchTax = new MasBranchTax()
                        {
                            CompCode = request.CompCode,
                            BrnCode = request.BrnCode,
                            TaxId = request.TaxId,
                            TaxName = request.TaxName,
                            TaxAmt = request.TaxAmt,
                            CreatedDate = DateTime.Now,
                            CreatedBy = request.CreatedBy
                        };

                        await _branchRepository.AddBranchTaxAsync(saveMasBranchTax);
                        branchTaxResponse.Add(saveMasBranchTax);
                        response.MasBranchTaxs = branchTaxResponse;

                    }
                    //await _branchRepository.DeleteBranchTaxAsync(saveMasBranchTax);
                    //var branchTaxResponse = new List<MasBranchTax>();
                    //masBranchTax.CompCode = request.CompCode;
                    //masBranchTax.BrnCode = request.BrnCode;
                    //masBranchTax.TaxName = request.TaxName;
                    //masBranchTax.TaxAmt = request.TaxAmt;
                    //masBranchTax.UpdatedDate = DateTime.Now;
                    //masBranchTax.UpdatedBy = request.UpdatedBy;

                    //await _branchRepository.UpdateBranchTaxAsync(masBranchTax);
                    //branchTaxResponse.Add(masBranchTax);
                    //response.MasBranchTaxs = branchTaxResponse;
                }

                


                await _unitOfWork.CompleteAsync();
                return new SaveBranchTaxResponse(response);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new SaveBranchTaxResponse($"An error occurred when saving the branch tax: {strMessage}");
            }
        }
    }
}
