using MasterData.API.Resources.Message;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class MessageService
    {
        PTMaxstationContext _context;

        public MessageService(PTMaxstationContext pContext)
        {
            _context = pContext;
        }

        public async Task<ModelSysMessage[]> GetArrSysMessage()
        {
            string strCon = _context.Database.GetConnectionString();
            string strSql = "SELECT[MSG_CODE],[MSG_LANG],[MSG_TEXT]from[SYS_MESSAGE]";
            var result = await DefaultService.GetEntityFromSql<ModelSysMessage[]>(strCon, strSql);
            return result;
        }
    }
}
