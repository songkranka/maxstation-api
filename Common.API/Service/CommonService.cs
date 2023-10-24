using Common.API.Domain.Models;
using Common.API.Domain.Repositories;
using Common.API.Domain.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Common.API.Services
{
    public class CommonService : ICommonService
    {
        readonly ICommonRepository commonRepositories;
        private readonly IUnitOfWork _unitOfWork;

        readonly string urlMasterAPI = @"https://pt-max-station-masterdata-api.azurewebsites.net";
        protected PTMaxstationContext context;
        public CommonService(ICommonRepository _commonRepositories, IUnitOfWork unitOfWork, PTMaxstationContext context)
        {
            this.commonRepositories = _commonRepositories;
            this.context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> UpdateCloseDay(RequestData req)
        {
            string resp = "";
            try
            {
                commonRepositories.UpdateCloseDay(req);
                await _unitOfWork.CompleteAsync();
                resp = "Update Post Success!!!";
            }
            catch (Exception ex)
            {
                resp = "Error Because : " + ex.Message;
            }
            finally {
            }
            return resp;
        }
        public async Task<SysNotification[]> GetNoti(GetNotiParam param)
        {
            if(param == null)
            {
                return null;
            }
            var qryNoti = context.SysNotifications.Where(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.IsRead != "Y"
            ).OrderByDescending(x=> x.DocDate)
            .Take(10).AsNoTracking();
            var arrNoti = await qryNoti.ToArrayAsync();
            return arrNoti;
        }

        public async Task<bool> UpdateNoti(SysNotification param)
        {
            if(param == null)
            {
                return false;
            }
            var entNoti = context.Attach(param);
            entNoti.Property(x => x.IsRead).IsModified = true;
            entNoti.Property(x => x.Remark).IsModified = true;
            entNoti.Property(x => x.UpdatedBy).IsModified = true;
            param.UpdatedDate = DateTime.Now;
            await _unitOfWork.CompleteAsync();
            return true;
        }
    
    }
}
