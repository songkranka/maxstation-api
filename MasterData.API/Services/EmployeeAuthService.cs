using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MasterData.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static MasterData.API.Domain.Models.Request.SaveBranchRequest;

namespace MasterData.API.Services
{
    public class EmployeeAuthService : IEmployeeAuthService
    {
        private readonly IEmployeeAuthRepository _employeeAuthRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PTMaxstationContext _context;

        public EmployeeAuthService(
            IEmployeeAuthRepository employeeAuthRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context
        )
        {
            _employeeAuthRepository = employeeAuthRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<AutEmployeeRole> FindByEmpCodeAsync(string empCode)
        {
            return await _employeeAuthRepository.FindByEmpCodeAsync(empCode);
        }

        public async Task<EmployeeAuthResponse> SaveEmployeeAuthAsync(SaveEmployeeAuthRequest request)
        {
            try
            {
                foreach(var employeeAuth in request._EmployeeAuth)
                {
                    var authEmployeeRole = await _employeeAuthRepository.FindByEmpCodeAsync(employeeAuth.EmpCode);
                    
                    if(authEmployeeRole == null)
                    {
                        var saveAuthEmployeeRole = new AutEmployeeRole()
                        {
                            EmpCode = employeeAuth.EmpCode,
                            AuthCode = employeeAuth.AuthCode,
                            PositionCode = employeeAuth.PositionCode,
                            CreatedBy = request.User,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = request.User,
                            UpdatedDate = DateTime.Now
                    };

                        await _employeeAuthRepository.AddAuthEmployeeRoleAsync(saveAuthEmployeeRole);
                    }
                    else
                    {
                        authEmployeeRole.AuthCode = employeeAuth.AuthCode;
                        authEmployeeRole.PositionCode = employeeAuth.PositionCode;
                        authEmployeeRole.UpdatedBy = request.User;
                        authEmployeeRole.UpdatedDate = DateTime.Now;

                        await _employeeAuthRepository.UpdateAuthEmployeeRoleAsync(authEmployeeRole);
                    }
                }

                await _unitOfWork.CompleteAsync();
                return new EmployeeAuthResponse(request);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return new EmployeeAuthResponse($"An error occurred when saving the employeeauth: {strMessage}");
            }
        }


        public async Task<MasOrganize[]> GetArrOrganize(string pStrEmpCode)
        {
            pStrEmpCode = (pStrEmpCode ?? string.Empty).Trim();
            IQueryable<MasEmployee> qryEmp;
            qryEmp = _context.MasEmployees
                .Where(x=> x.EmpCode == pStrEmpCode)
                .AsNoTracking();

            IQueryable<MasOrganize> qryOrg;
            qryOrg = _context.MasOrganizes
                .Where(x => qryEmp.Any(y => y.CodeDev == x.OrgCodedev))
                .AsNoTracking();

            MasOrganize[] result;
            result = await qryOrg.ToArrayAsync();
            return result;
        }

        public async Task<List<AuthBranch>> GetAuthBranch(string compCode, string empCode)
        {
            List<MasBranch> branches = new List<MasBranch>();
            var authEmployee = await _employeeAuthRepository.FindByEmpCodeAsync(empCode); //_authService.FindAuthEmpRoleByEmpCode(user.UserName);


            if (authEmployee != null)
            {
                switch (authEmployee.AuthCode)
                {
                    case 0:
                        var branch = await _employeeAuthRepository.FindBranchByBrnCode(compCode, empCode);
                        if(branch != null)
                        {
                            branches.Add(branch);
                        }
                        break;
                    case 1:
                        branches = await _employeeAuthRepository.GetBranchByCompCode(compCode);
                        break;
                    default:
                        branches = await _employeeAuthRepository.GetBranchByAuthCode(compCode, authEmployee.AuthCode);
                        break;
                }
            }
         
            return  branches.Select(x => new AuthBranch { BrnCode = x.BrnCode, BrnName = x.BrnName }).ToList(); ;
        }
    }
}
