using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using Inventory.API.Resources.Request;
using Inventory.API.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class RequestRepository : SqlDataAccessHelper, IRequestRepository
    {
        public RequestRepository(PTMaxstationContext context) : base(context)
        {

        }

        public int GetRunNumber(InvRequest obj)
        {
            int runNumber = 1;
            InvRequestHd resp = new InvRequestHd();
            resp = this.context.InvRequestHds.OrderByDescending(y => y.RunNumber).FirstOrDefault(
                x => (x.CompCode == obj.CompCode || obj.CompCode == "" || obj.CompCode == null)
                && (x.BrnCode == obj.BrnCode || obj.BrnCode == "" || obj.BrnCode == null)
                && (x.LocCode == obj.LocCode || obj.LocCode == "" || obj.LocCode == null)
                && (x.DocDate.Value.Year == obj.DocDate.Value.Year)
                && (x.DocDate.Value.Month == obj.DocDate.Value.Month)
            );

            if (resp != null)
            {
                runNumber = (int)resp.RunNumber + 1;
            }
            else
            {
                runNumber = 1; //เริ่มต้นด้วย 1
            }
            return runNumber;
        }

        public decimal CalculateStockQty(string pdId, string unitId, decimal itemQty)
        {
            decimal stockQty = 0m;
            MasProductUnit productUnit = this.context.MasProductUnits.FirstOrDefault(x => x.PdId == pdId && x.UnitId == unitId);
            if (productUnit != null)
            {
                stockQty = (itemQty * (productUnit.UnitStock / productUnit.UnitRatio)).Value;
            }
            return stockQty;
        }

        public async Task CreateRequest(InvRequest obj)
        {
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            await context.InvRequestHds.AddAsync(obj);
            await context.InvRequestDts.AddRangeAsync(obj.InvRequestDt);
            await creatApprove(obj);
            #region Send data to warpad

            var configApi = context.SysConfigApis.Where(x => x.SystemId == "Warpad" && x.ApiId == "M001").FirstOrDefault();

            if (configApi != null) 
            {
                var devCodeFrom = context.MasOrganizes.Where(x => x.OrgComp == obj.CompCode && x.OrgCode == obj.BrnCodeFrom).Select(x => x.OrgCodedev).FirstOrDefault();
                var devCodeTo = context.MasOrganizes.Where(x => x.OrgComp == obj.CompCode && x.OrgCode == obj.BrnCodeTo).Select(x => x.OrgCodedev).FirstOrDefault();

                var toppic = configApi.Topic.Replace("{doc_date}", obj.DocDate.Value.ToString("dd/MM/yyyy"));

                var request = new RequestWarpadModel()
                {
                    TOPIC = toppic,
                    CREATE_DATE = obj.CreatedDate.Value.ToString("yyyyMMdd"),
                    CREATE_TIME = obj.CreatedDate.Value.ToString("HH:mm"),
                    BRANCH_FROM = devCodeFrom ?? "",
                    BRANCH_TO = devCodeTo ?? "",
                    DOC_NUMBER = obj.DocNo,
                    LINK = "https://maxstation.pt.co.th/RequestList",
                };

                var listData = new List<RequestWarpadDataMedel>();
                foreach (var item in obj.InvRequestDt)
                {
                    var data = new RequestWarpadDataMedel()
                    {
                        ITEM = $"{item.PdId} - {item.PdName} : {item.ItemQty} {item.UnitName}"
                    };

                    listData.Add(data);
                }

                request.DATA = listData;

                await SendDataToWarpadAsync(request, configApi.ApiUrl);
            }

            #endregion
            
        }
        private async Task creatApprove(InvRequest param)
        {
            if (param == null)
            {
                return;
            }
            SysApproveConfig config = await context.SysApproveConfigs.FirstOrDefaultAsync(x => "Request" == x.DocType);
            if (config == null)
            {
                return;
            }
            var appResult = await DefaultService.GetApproverStep(config.StepCount ?? 1, param.CreatedBy, context);
            if (!appResult?.workflowApprover?.Any() ?? true)
            {
                return;
            }
            var arrEmpId = appResult.workflowApprover.Select(x => x.empId).ToArray();
            var arrEmp = await context.MasEmployees
                .Where(x => arrEmpId.Contains(x.EmpCode))
                .AsNoTracking().ToArrayAsync();
            int intStepSeq = 1;
            foreach (var item in appResult.workflowApprover)
            {
                SysApproveStep step = new SysApproveStep();
                step.BrnCode = param.BrnCode;
                step.CompCode = param.CompCode;
                step.LocCode = param.LocCode;
                step.DocType = "Request";
                step.CreatedBy = param.CreatedBy;
                step.CreatedDate = DateTime.Now;
                step.DocNo = param.DocNo;
                step.StepNo = intStepSeq++;
                step.EmpCode = item.empId;
                step.Guid = param.Guid;
                step.EmpName = arrEmp?.FirstOrDefault(x => x.EmpCode == item.empId)?.EmpName ?? string.Empty;
                await this.context.SysApproveSteps.AddAsync(step);
            }
        }
        public List<InvRequestHds> GetRequestHDList(RequestQueryResource req)
        {
            var sql = (from reHD in context.InvRequestHds
                       join dt in context.MasDocumentTypes on reHD.DocTypeId equals dt.DocTypeId
                       where (reHD.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                            && (reHD.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                            && (reHD.DocNo.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                            && (reHD.BrnCodeFrom == req.BrnCodeFrom || req.BrnCodeFrom == "" || req.BrnCodeFrom == null)
                            && (reHD.DocStatus == req.DocStatus || req.DocStatus == "" || req.DocStatus == null)
                       select new { reHD, dt }).AsQueryable();

            List<InvRequestHds> resp = sql.Select(x => new InvRequestHds
            {
                CompCode = x.reHD.CompCode,
                BrnCode = x.reHD.BrnCode,
                LocCode = x.reHD.LocCode,
                DocTypeId = x.reHD.DocTypeId,
                DocNo = x.reHD.DocNo,
                DocStatus = x.reHD.DocStatus,
                DocDate = x.reHD.DocDate,
                BrnCodeFrom = x.reHD.BrnCodeFrom,
                BrnNameFrom = x.reHD.BrnNameFrom,
                BrnCodeTo = x.reHD.BrnCodeTo,
                BrnNameTo = x.reHD.BrnNameTo,
                Remark = x.reHD.Remark,
                Post = x.reHD.Post,
                RunNumber = x.reHD.RunNumber,
                Guid = x.reHD.Guid,
                CreatedDate = x.reHD.CreatedDate,
                CreatedBy = x.reHD.CreatedBy,
                UpdatedDate = x.reHD.UpdatedDate,
                UpdatedBy = x.reHD.UpdatedBy,
                PdGroupName = x.dt.DocTypeName
            }).ToList();
            return resp;
        }

        public async Task<List<InvRequestHds>> GetRequestHdListNew(RequestData req)
        {
            var sql = (from reHD in context.InvRequestHds
                       join dt in context.MasDocumentTypes on reHD.DocTypeId equals dt.DocTypeId
                       where (reHD.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                            && (reHD.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                            && (reHD.LocCode == req.LocCode || req.LocCode == "" || req.LocCode == null)
                            && (reHD.DocNo.Contains(req.Keyword) || reHD.DocStatus.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                            && (reHD.DocStatus == req.DocStatus || req.DocStatus == "" || req.DocStatus == null)
                            && ((reHD.DocDate >= req.FromDate && reHD.DocDate <= req.ToDate) || req.FromDate == null || req.ToDate == null)
                       select new { reHD, dt }).AsQueryable();

            List<InvRequestHds> resp = sql.Select(x => new InvRequestHds
            {
                CompCode = x.reHD.CompCode,
                BrnCode = x.reHD.BrnCode,
                LocCode = x.reHD.LocCode,
                DocTypeId = x.reHD.DocTypeId,
                DocNo = x.reHD.DocNo,
                DocStatus = x.reHD.DocStatus,
                DocDate = x.reHD.DocDate,
                BrnCodeFrom = x.reHD.BrnCodeFrom,
                BrnNameFrom = x.reHD.BrnNameFrom,
                BrnCodeTo = x.reHD.BrnCodeTo,
                BrnNameTo = x.reHD.BrnNameTo,
                Remark = x.reHD.Remark,
                Post = x.reHD.Post,
                RunNumber = x.reHD.RunNumber,
                Guid = x.reHD.Guid,
                CreatedDate = x.reHD.CreatedDate,
                CreatedBy = x.reHD.CreatedBy,
                UpdatedDate = x.reHD.UpdatedDate,
                UpdatedBy = x.reHD.UpdatedBy,
                PdGroupName = x.dt.DocTypeName
            })
            //.OrderByDescending(y => y.DocDate)
            .OrderByDescending(x=> x.CompCode)
            .ThenByDescending(z => z.BrnCode)
            .ThenByDescending(z => z.LocCode)
            .ThenByDescending(z => z.DocNo)
            .Skip((req.Skip - 1) * req.Take)
            .Take(req.Take).ToList();
            return resp;
        }

        public async Task<int> GetRequestHdCount(RequestData req)
        {
            int resp = 0;
            resp = this.context.InvRequestHds.Where(
                x => (x.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                && (x.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                && (x.LocCode == req.LocCode || req.LocCode == "" || req.LocCode == null)
                && (x.DocNo.Contains(req.Keyword) || x.DocStatus.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                && (x.DocStatus == req.DocStatus || req.DocStatus == "" || req.DocStatus == null)
                && ((x.DocDate >= req.FromDate && x.DocDate <= req.ToDate) || req.FromDate == null || req.ToDate == null)
            ).OrderByDescending(y => y.DocDate).ThenByDescending(z => z.DocNo).Count();
            return resp;
        }

        public InvRequest GetRequest(RequestQueryResource req)
        {
            InvRequest resp = new InvRequest();
            var reqHD = this.context.InvRequestHds
                .Select(x => new InvRequest
                {
                    CompCode = x.CompCode,
                    BrnCode = x.BrnCode,
                    LocCode = x.LocCode,
                    DocTypeId = x.DocTypeId,
                    DocNo = x.DocNo,
                    DocStatus = x.DocStatus,
                    DocDate = x.DocDate,
                    DocPattern = x.DocPattern,
                    BrnCodeFrom = x.BrnCodeFrom,
                    BrnNameFrom = x.BrnNameFrom,
                    BrnCodeTo = x.BrnCodeTo,
                    BrnNameTo = x.BrnNameTo,
                    Remark = x.Remark,
                    Post = x.Post,
                    RunNumber = x.RunNumber,
                    Guid = x.Guid,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    UpdatedDate = x.UpdatedDate,
                    UpdatedBy = x.UpdatedBy
                })
                .FirstOrDefault(x => (x.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                    && (x.BrnCodeFrom == req.BrnCodeFrom || req.BrnCodeFrom == "" || req.BrnCodeFrom == null)
                    && (x.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                    && (x.DocNo == req.DocNo || req.DocNo == "" || req.DocNo == null)
                    && (req.Guid == "" || req.Guid == null || x.Guid == Guid.Parse(req.Guid))
                )
                ;

            if (reqHD != null)
            {
                var reqDT = this.context.InvRequestDts.Where(
                    x => x.CompCode == reqHD.CompCode
                    && x.BrnCode == reqHD.BrnCode
                    && x.DocNo == reqHD.DocNo
                    && x.DocTypeId == reqHD.DocTypeId
                ).OrderBy(y => y.SeqNo).ToList();

                resp = reqHD;
                resp.InvRequestDt = reqDT;
            }
            return resp;
        }

        public async Task UpdateRequest(InvRequest obj)
        {
            InvRequestHd rqHD = this.context.InvRequestHds.FirstOrDefault(
                x => (x.CompCode == obj.CompCode || obj.CompCode == "" || obj.CompCode == null)
                && (x.BrnCode == obj.BrnCode || obj.BrnCode == "" || obj.BrnCode == null)
                && (x.LocCode == obj.LocCode || obj.LocCode == "" || obj.LocCode == null)
                && (x.DocNo == obj.DocNo || obj.DocNo == "" || obj.DocNo == null)
                && (x.DocTypeId == obj.DocTypeId || obj.DocTypeId == "" || obj.DocTypeId == null)
                && (x.Guid == obj.Guid)
            );

            //Mapping RequestHD
            rqHD.CompCode = obj.CompCode;
            rqHD.BrnCode = obj.BrnCode;
            rqHD.LocCode = obj.LocCode;
            rqHD.DocTypeId = obj.DocTypeId;
            rqHD.DocNo = obj.DocNo;
            rqHD.DocStatus = obj.DocStatus;
            rqHD.DocDate = obj.DocDate;
            rqHD.BrnCodeFrom = obj.BrnCodeFrom;
            rqHD.BrnNameFrom = obj.BrnNameFrom;
            rqHD.BrnCodeTo = obj.BrnCodeTo;
            rqHD.BrnNameTo = obj.BrnNameTo;
            rqHD.Remark = obj.Remark;
            rqHD.Post = obj.Post;
            rqHD.RunNumber = obj.RunNumber;
            rqHD.Guid = obj.Guid;
            //rqHD.CreatedDate = obj.CreatedDate;
            //rqHD.CreatedBy = obj.CreatedBy;
            rqHD.UpdatedDate = DateTime.Now;
            rqHD.UpdatedBy = obj.UpdatedBy;

            //Delete RequestDT
            DeleteRequestDT(obj);

            //Insert RequestDT
            int seqNo = 1;
            foreach (InvRequestDt row in obj.InvRequestDt)
            {
                row.SeqNo = seqNo;
                seqNo++;
            }
            await context.InvRequestDts.AddRangeAsync(obj.InvRequestDt);
            if("Cancel".Equals(obj.DocStatus))
            {
                await CancelApprove(obj);
            }
        }

        public async Task CancelApprove(InvRequest param)
        {
            var arrStep = await this.context.SysApproveSteps.Where(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.DocNo == param.DocNo
            ).ToArrayAsync();
            foreach (var item in arrStep)
            {
                item.ApprStatus = "C";
            }
        }

        public void DeleteRequestDT(InvRequest obj)
        {
            var rqDT = context.InvRequestDts.Where(
                x => x.CompCode == obj.CompCode
                && x.BrnCode == obj.BrnCode
                && x.LocCode == obj.LocCode
                && x.DocTypeId == obj.DocTypeId
                && x.DocNo == obj.DocNo
            ).ToList();
            context.RemoveRange(rqDT);
        }

        
    }
}
