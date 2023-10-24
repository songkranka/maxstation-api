using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUnitOfWork _unitOfWork;
        private PTMaxstationContext _context = null;
        public MenuService(
            IMenuRepository menuRepository,
            IUnitOfWork unitOfWork , 
            PTMaxstationContext pContext)
        {
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
            _context = pContext;
        }

        public async Task<Menus> FindByBranchCodeAsync(string compCode, string brnCode)
        {
            return await _menuRepository.FindByBranchCodeAsync(compCode, brnCode);
        }

        public async Task<AutPositionRole> GetPositionRole(string pStrEmpCode , string pStrRouteUrl)
        {
            if(string.IsNullOrWhiteSpace(pStrEmpCode) || string.IsNullOrWhiteSpace(pStrRouteUrl))
            {
                return null;
            }
            var qrySysMenu = _context.SysMenus.Where(x => x.Route.Contains( pStrRouteUrl)).AsNoTracking();
            var qryEmp = _context.MasEmployees.Where(x => x.EmpCode == pStrEmpCode).AsNoTracking();
            var qrySysPosRole = _context.AutPositionRoles.Where(
                x => qryEmp.Any( y=> y.PositionCode == x.PositionCode)
                &&  qrySysMenu.Any( y=> y.MenuId == x.MenuId)
            ).AsNoTracking();
            var result = await qrySysPosRole.FirstOrDefaultAsync();
            return result;
        }

    }
}
