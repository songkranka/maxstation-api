using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Repositories;
using MasterData.API.Helpers;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Repositories
{
    public class MenuRepository : SqlDataAccessHelper, IMenuRepository
    {
        public MenuRepository(PTMaxstationContext context) : base(context) { }

        public async Task<Menus> FindByBranchCodeAsync(string compCode, string brnCode)
        {
            var response = new Menus();

            var menus = await (from mr in context.SysMenuRoles
                               join mn in context.SysMenus on mr.MenuId equals mn.MenuId
                               where mr.CompCode == compCode && mr.BrnCode == brnCode
                               select mn)
                               .ToListAsync();

            //if (receiveOilHd != null)
            //{
            //    var receiveOilDts = context.InvReceiveProdDts.AsNoTracking().Where(x => x.CompCode == receiveOilHd.CompCode && x.BrnCode == receiveOilHd.BrnCode && x.DocNo == receiveOilHd.DocNo).OrderBy(y => y.SeqNo).ToList();
            //    response.InvReceiveProdHd = receiveOilHd;
            //    response.InvReceiveProdDts = receiveOilDts;
            //}
            response.SysMenus = menus.OrderBy(x => x.SeqNo).ToList();

            return response;
        }
    }
}
