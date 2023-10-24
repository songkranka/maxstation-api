using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Repositories;
using Sale.API.Helpers;
using Sale.API.Resources;
using Sale.API.Resources.Quotation;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sale.API.Repositories
{
    public class QuotationRepository : SqlDataAccessHelper, IQuotationRepository
    {
        private const string _strActive = "Active";
        public QuotationRepository(PTMaxstationContext context) : base(context)
        {
        }

        public void CreateQuotation(SalQuotationHd obj)
        {
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            this.context.SalQuotationHds.Add(obj);
            this.context.SalQuotationDts.AddRange(obj.SalQuotationDt);
        }

        public async Task CreatApprove(SalQuotationHd param)
        {
            if(param == null)
            {
                return;
            }
            SysApproveConfig config = await context.SysApproveConfigs.FirstOrDefaultAsync(x=> "Quotation" == x.DocType);
            if(config == null)
            {
                return;
            }
            var appResult = await DefaultService.GetApproverStep(config.StepCount ?? 1, param.CreatedBy, context);
            if( !appResult?.workflowApprover?.Any() ?? true)
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
                step.DocType = "Quotation";
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
        public async Task CancelApprove(SalQuotationHd param)
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
        public async Task<SysApproveStep[]> GetApproveStep(SalQuotationHd param)
        {
            var result = await this.context.SysApproveSteps.Where(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.DocNo == param.DocNo
            ).AsNoTracking().ToArrayAsync();
            return result;
        }

        public void DeleteQuotationDT(SalQuotationHd obj)
        {
            var dt = this.context.SalQuotationDts.Where(
                x => x.CompCode == obj.CompCode
                && x.BrnCode == obj.BrnCode
                && x.LocCode == obj.LocCode
                && x.DocNo == obj.DocNo
            ).ToList();
            this.context.RemoveRange(dt);
        }

        public string GetMaxCardProfile(string pStrMaxcardId)
        {
            pStrMaxcardId = (pStrMaxcardId ?? string.Empty).Trim();
            if (0.Equals(pStrMaxcardId))
            {
                return string.Empty;
            }
            pStrMaxcardId = HttpUtility.UrlEncode(pStrMaxcardId);
            string result = string.Empty;
            string strUrl = @"https://pcp.pt.co.th:4083/crm_pos_getprofile.asmx/CrmPosGetProfile";
            WebRequest request = WebRequest.Create(strUrl);
            request.Method = "POST";
            string strPostData = "MaxcardNo=" + pStrMaxcardId + "&MidNo=3951510000000";
            byte[] byteArray = Encoding.UTF8.GetBytes(strPostData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = null;
            using (dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            using (WebResponse response = request.GetResponse())
            using (dataStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        public async Task<SalQuotationHd> GetQuotation(RequestData req)
        {
            SalQuotationHd result = null;
            result = await context.SalQuotationHds.FirstOrDefaultAsync(
                x => (x.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                && (x.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                && (x.LocCode == req.LocCode || req.LocCode == "" || req.LocCode == null)
                && (x.DocNo == req.DocNo || req.DocNo == "" || req.DocNo == null)
                && (req.Guid == "" || req.Guid == null || x.Guid == Guid.Parse(req.Guid))
            );

            var cusPrefix = await context.MasCustomers.Where(x => x.CustCode == result.CustCode).Select(x => x.CustPrefix).FirstOrDefaultAsync();
            result.CustPrefix = cusPrefix;

            if (result == null)
            {
                return null;
            }
            List<SalQuotationDt> dt = null;
            dt = await context.SalQuotationDts.Where(
                x => x.CompCode == result.CompCode
                && x.BrnCode == result.BrnCode
                && x.LocCode == result.LocCode
                && x.DocNo == result.DocNo
            ).OrderBy(y => y.SeqNo)
            .ToListAsync();
            result.SalQuotationDt = dt;
            return result;
        }

        /*
         public async Task<SalQuotationHd> GetQuotation(RequestData req)
        {
            SalQuotationHd resp = new SalQuotationHd();
            var hd = this.context.SalQuotationHds
                .Select(x => new SalQuotationHd
                {
                    CompCode = x.CompCode,
                    BrnCode = x.BrnCode,
                    LocCode = x.LocCode,
                    DocNo = x.DocNo,
                    DocStatus = x.DocStatus,
                    DocDate = x.DocDate,
                    CustCode = x.CustCode,
                    CustName = x.CustName,
                    CustAddr1 = x.CustAddr1,
                    CustAddr2 = x.CustAddr2,
                    BrnCodeFrom = x.BrnCodeFrom,
                    BrnNameFrom = x.BrnNameFrom,
                    StartDate = x.StartDate,
                    FinishDate = x.FinishDate,
                    Remark = x.Remark,
                    ApprCode = x.ApprCode,
                    ItemCount = x.ItemCount,
                    Currency = x.Currency,
                    CurRate = x.CurRate,
                    SubAmt = x.SubAmt,
                    SubAmtCur = x.SubAmtCur,
                    DiscRate = x.DiscRate,
                    DiscAmt = x.DiscAmt,
                    DiscAmtCur = x.DiscAmtCur,
                    TotalAmt = x.TotalAmt,
                    TotalAmtCur = x.TotalAmtCur,
                    TaxBaseAmt = x.TaxBaseAmt,
                    TaxBaseAmtCur = x.TaxBaseAmtCur,
                    VatRate = x.VatRate,
                    VatAmt = x.VatAmt,
                    VatAmtCur = x.VatAmtCur,
                    NetAmt = x.NetAmt,
                    NetAmtCur = x.NetAmtCur,
                    Post = x.Post,
                    RunNumber = x.RunNumber,
                    DocPattern = x.DocPattern,
                    Guid = x.Guid,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    UpdatedDate = x.UpdatedDate,
                    UpdatedBy = x.UpdatedBy,
                    DocType = x.DocType
                })
                .FirstOrDefault(x => (x.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                    && (x.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                    && (x.LocCode == req.LocCode || req.LocCode == "" || req.LocCode == null)
                    && (x.DocNo == req.DocNo || req.DocNo == "" || req.DocNo == null)
                    && (req.Guid == "" || req.Guid == null || x.Guid == Guid.Parse(req.Guid))
                );

            if (hd != null)
            {
                var dt = this.context.SalQuotationDts.Where(
                    x => x.CompCode == hd.CompCode
                    && x.BrnCode == hd.BrnCode
                    && x.LocCode == hd.LocCode
                    && x.DocNo == hd.DocNo
                ).OrderBy(y => y.SeqNo).ToList();

                resp = hd;
                resp.SalQuotationDt = dt;
            }
            return resp;
        }
         
         */

        public async Task<QueryResult<SalQuotationHd>> GetQuotationHdList(QuotationHdQuery req)
        {

            //QueryResult<SalQuotationHd> resp = new List<SalQuotationHd>();
            try
            {
                var query = this.context.SalQuotationHds.Where(
                              x => (x.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                              && (x.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)                              
                              && (x.DocNo.Contains(req.Keyword) || x.DocStatus.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                              && (x.DocStatus == req.DocStatus || req.DocStatus == "" || req.DocStatus == null)
                              && (x.CustCode == req.CustCode || req.CustCode == "" || req.CustCode == null)
                              && ((x.DocDate >= req.FromDate && x.DocDate <= req.ToDate) || req.FromDate == null || req.ToDate == null)
                          ).OrderByDescending(y => y.DocDate).ThenByDescending(z => z.DocNo).AsNoTracking();


                int totalItems = await query.CountAsync();
                var resp = await query.Skip((req.Page - 1) * req.ItemsPerPage)
                                               .Take(req.ItemsPerPage)
                                               .ToListAsync();
                return new QueryResult<SalQuotationHd>
                            {
                                Items = resp,
                                TotalItems = totalItems,
                                ItemsPerPage = req.ItemsPerPage
                            };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<QueryResult<QuotationResource>> SearchList(QuotationHdQuery query)
        {
            if (query == null)
            {
                return null;
            }
            string strIsoLevel = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            string strSelect = @"select doc_no , doc_date , Cust_Code , Cust_Name , net_Amt , doc_status , guid from SAL_QUOTATION_HD(nolock)";
            string strCount = @"select COUNT(*) from SAL_QUOTATION_HD(nolock)";
            string strWhere = string.Empty;
            string strOrderBy = @" order by COMP_CODE, BRN_CODE, DOC_NO desc ";
            var listWhere = new List<string>();
            string strComCode = DefaultService.EncodeSqlString(query.CompCode);
            if (!0.Equals(strComCode.Length))
            {
                listWhere.Add($"COMP_CODE = '{strComCode}'");
            }
            string strBrnCode = DefaultService.EncodeSqlString(query.BrnCode);
            if (!0.Equals(strBrnCode.Length))
            {
                listWhere.Add($"BRN_CODE = '{strBrnCode}'");
            }
            if (query.FromDate.HasValue && query.ToDate.HasValue)
            {
                listWhere.Add($"DOC_DATE between '{DefaultService.EncodeSqlDate(query.FromDate.Value)}' and '{DefaultService.EncodeSqlDate(query.ToDate.Value)}'");
            }
            string strKeyWord = DefaultService.EncodeSqlString(query.Keyword);
            if (!0.Equals(strKeyWord.Length))
            {
                listWhere.Add($"( DOC_NO like '%{strKeyWord}%' or DOC_STATUS like '%{strKeyWord}%' )");
            }
            if (listWhere.Any())
            {
                strWhere = " where " + listWhere.Aggregate((x, y) => x+ Environment.NewLine + "and " + y );
            }
            string strPage = string.Empty;
            if (query.Page > 0 && query.ItemsPerPage > 0)
            {
                strPage = $" OFFSET {(query.Page - 1) * query.ItemsPerPage} row fetch next {query.ItemsPerPage} row only";
            }
            int intTotal = await DefaultService.ExecuteScalar<int>(context, strIsoLevel + strCount + strWhere);
            var listQuotation = await DefaultService.GetEntityFromSql<List<QuotationResource>>(
                context, strIsoLevel + strSelect + strWhere + strOrderBy + strPage
            );
            return new QueryResult<QuotationResource>
            {
                Items = listQuotation ?? new List<QuotationResource>(),
                TotalItems = intTotal,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<QueryResult<QuotationResource>> SearchListOld(QuotationHdQuery req)
        {            
            var query = this.context.SalQuotationHds.AsNoTracking().Where(
                x => (x.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                && (x.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                && (x.DocNo.Contains(req.Keyword) || x.DocStatus.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                && (x.DocStatus == req.DocStatus || req.DocStatus == "" || req.DocStatus == null)
                && (x.CustCode == req.CustCode || req.CustCode == "" || req.CustCode == null)
                && ((x.DocDate >= req.FromDate && x.DocDate <= req.ToDate) || req.FromDate == null || req.ToDate == null)
            ).OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
            .Select(x=>new QuotationResource { 
                DocDate = x.DocDate,
                DocNo = x.DocNo,
                DocStatus = x.DocStatus,
                CustCode = x.CustCode,
                CustName = x.CustName,
                Guid = x.Guid.Value,
                NetAmt = x.NetAmt,
                NetAmtCur = x.NetAmtCur
            });


            int totalItems = await query.CountAsync();
            var resp = await query.Skip((req.Page - 1) * req.ItemsPerPage)
                .Take(req.ItemsPerPage)
                .ToListAsync();
            return new QueryResult<QuotationResource>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = req.ItemsPerPage
            };
        }
        /*
         public async Task<QueryResult<QuotationResource>> SearchList(QuotationHdQuery req)
        {

            //QueryResult<SalQuotationHd> resp = new List<SalQuotationHd>();
            try
            {
                var query = this.context.SalQuotationHds.Where(
                              x => (x.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                              && (x.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                              && (x.DocNo.Contains(req.Keyword) || x.DocStatus.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                              && (x.DocStatus == req.DocStatus || req.DocStatus == "" || req.DocStatus == null)
                              && (x.CustCode == req.CustCode || req.CustCode == "" || req.CustCode == null)
                              && ((x.DocDate >= req.FromDate && x.DocDate <= req.ToDate) || req.FromDate == null || req.ToDate == null)
                          ).OrderByDescending(y => y.DocDate).ThenByDescending(z => z.DocNo)
                          .Select(x=>new QuotationResource { 
                                                DocDate = x.DocDate,
                                                DocNo = x.DocNo,
                                                DocStatus = x.DocStatus,
                                                CustCode = x.CustCode,
                                                CustName = x.CustName,
                                                Guid = x.Guid.Value,
                                                NetAmt = x.NetAmt,
                                                NetAmtCur = x.NetAmtCur
                                            }).AsNoTracking();


                int totalItems = await query.CountAsync();
                var resp = await query.Skip((req.Page - 1) * req.ItemsPerPage)
                                               .Take(req.ItemsPerPage)
                                               .ToListAsync();
                return new QueryResult<QuotationResource>
                {
                    Items = resp,
                    TotalItems = totalItems,
                    ItemsPerPage = req.ItemsPerPage
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
         */

        public int GetQuotationHdCount(RequestData req)
        {
            int resp = 0;
            var query = this.context.SalQuotationHds.Where(
                        x => (x.CompCode == req.CompCode || req.CompCode == "" || req.CompCode == null)
                        && (x.BrnCode == req.BrnCode || req.BrnCode == "" || req.BrnCode == null)
                        && (x.LocCode == req.LocCode || req.LocCode == "" || req.LocCode == null)
                        && (x.DocNo.Contains(req.Keyword) || x.DocStatus.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                        && (x.DocStatus == req.DocStatus || req.DocStatus == "" || req.DocStatus == null)
                        && (x.CustCode == req.CustCode || req.CustCode == "" || req.CustCode == null)
                        && ((req.FromDate >= x.DocDate && x.DocDate <= req.ToDate) || req.FromDate == null || req.ToDate == null)
                    ).AsQueryable();

            resp = query.Count();
            return resp;
        }

        public async Task<List<SalQuotationHd>> GetQuotationHdRemainList(RequestData req)
        {
            List<SalQuotationHd> resp = new List<SalQuotationHd>();


            var query = (from hd in this.context.SalQuotationHds
                         join dt in this.context.SalQuotationDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo }
                                                                equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         where hd.CompCode == req.CompCode
                         && hd.BrnCodeFrom == req.BrnCode
                         && (hd.DocStatus == "Ready" || hd.DocStatus == "Reference")
                         && (hd.DocNo.Contains(req.Keyword) || hd.CustCode.Contains(req.Keyword) || hd.CustName.Contains(req.Keyword) || req.Keyword == "" || req.Keyword == null)
                         && (hd.CustCode == req.CustCode || req.CustCode == "" || req.CustCode == null)
                         && ("CreditSale".Equals(hd.DocType))
                         select new { hd, dt }
                          ).AsQueryable();


            resp = await query.GroupBy(x => new
            {
                x.hd.CompCode,
                x.hd.BrnCode,
                x.hd.LocCode,
                x.hd.DocDate,
                x.hd.DocNo
                                                ,
                x.hd.CustCode,
                x.hd.CustName,
                x.hd.BrnCodeFrom,
                x.hd.BrnNameFrom,
                x.hd.StartDate,
                x.hd.FinishDate,
                x.hd.Guid
            })
                        .Select(x => new SalQuotationHd
                        {
                            CompCode = x.Key.CompCode,
                            BrnCode = x.Key.BrnCode,
                            LocCode = x.Key.LocCode,
                            DocDate = x.Key.DocDate,
                            DocNo = x.Key.DocNo,
                            StartDate = x.Key.StartDate,
                            FinishDate = x.Key.FinishDate,
                            CustCode = x.Key.CustCode,
                            CustName = x.Key.CustName,
                            Guid = x.Key.Guid,
                            SumStockRemain = x.Sum(s => s.dt.StockRemain)
                        })
                        .Where(x => x.SumStockRemain > 0)
                        .OrderByDescending(y => y.DocDate).ThenByDescending(z => z.DocNo).ToListAsync();

            return resp;
        }

        public int GetRunNumber(SalQuotationHd obj)
        {
            int runNumber = 1;
            SalQuotationHd resp = new SalQuotationHd();
            resp = this.context.SalQuotationHds.OrderByDescending(y => y.RunNumber).FirstOrDefault(
                x => (x.DocPattern == obj.DocPattern || obj.DocPattern == "" || obj.DocPattern == null)
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

        public void UpdateQuotation(SalQuotationHd obj)
        {
            if(obj == null)
            {
                return;
            }
            obj.UpdatedDate = DateTime.Now;

            EntityEntry<SalQuotationHd> entQuotation = null;
            entQuotation = this.context.SalQuotationHds.Update(obj);            
            entQuotation.Property(x => x.CreatedBy).IsModified = false;
            entQuotation.Property(x => x.CreatedDate).IsModified = false;
            entQuotation.Property(x => x.RunNumber).IsModified = false;
            entQuotation.Property(x => x.DocPattern).IsModified = false;
            entQuotation.Property(x => x.Guid).IsModified = false;



            DeleteQuotationDT(obj);
            int seqNo = 0;
            foreach (SalQuotationDt row in obj.SalQuotationDt)
            {
                row.SeqNo = ++seqNo;
            }
            this.context.SalQuotationDts.AddRange(obj.SalQuotationDt);

        }
        public async void UpdateQuotation2(SalQuotationHd obj)
        {
            SalQuotationHd hd = this.context.SalQuotationHds.FirstOrDefault(
                x => (x.Guid == obj.Guid)
            );

            //Mapping QuotationHD
            hd.CompCode = obj.CompCode;
            hd.BrnCode = obj.BrnCode;
            hd.LocCode = obj.LocCode;
            hd.DocNo = obj.DocNo;
            hd.DocStatus = obj.DocStatus;
            hd.DocDate = obj.DocDate;
            hd.CustCode = obj.CustCode;
            hd.CustName = obj.CustName;
            hd.CustAddr1 = obj.CustAddr1;
            hd.CustAddr2 = obj.CustAddr2;
            hd.BrnCodeFrom = obj.BrnCodeFrom;
            hd.BrnNameFrom = obj.BrnNameFrom;
            hd.StartDate = obj.StartDate;
            hd.FinishDate = obj.FinishDate;
            hd.Remark = obj.Remark;
            hd.ApprCode = obj.ApprCode;
            hd.ItemCount = obj.ItemCount;
            hd.Currency = obj.Currency;
            hd.CurRate = obj.CurRate;
            hd.SubAmt = obj.SubAmt;
            hd.SubAmtCur = obj.SubAmtCur;
            hd.DiscRate = obj.DiscRate;
            hd.DiscAmt = obj.DiscAmt;
            hd.DiscAmtCur = obj.DiscAmtCur;
            hd.NetAmt = obj.NetAmt;
            hd.NetAmtCur = obj.NetAmtCur;
            hd.VatRate = obj.VatRate;
            hd.VatAmt = obj.VatAmt;
            hd.VatAmtCur = obj.VatAmtCur;
            hd.TotalAmt = obj.TotalAmt;
            hd.TotalAmtCur = obj.TotalAmtCur;
            hd.Post = obj.Post;
            hd.RunNumber = obj.RunNumber;
            hd.DocPattern = obj.DocPattern;
            
            hd.UpdatedDate = DateTime.Now;
            hd.UpdatedBy = obj.UpdatedBy;

            //Delete QuotationDT
            DeleteQuotationDT(obj);

            //Insert QuotationDT
            int seqNo = 0;
            foreach (SalQuotationDt row in obj.SalQuotationDt)
            {
                seqNo++;
                row.SeqNo = seqNo;
            }
            this.context.SalQuotationDts.AddRange(obj.SalQuotationDt);
        }

        public async void UpdateDocStatusQuotation(SalQuotationHd obj)
        {
            SalQuotationHd hd = this.context.SalQuotationHds.FirstOrDefault(
                x => (x.Guid == obj.Guid)
            );

            //Update Some Culumn
            hd.DocStatus = obj.DocStatus;
            hd.UpdatedDate = DateTime.Now;
            hd.UpdatedBy = obj.UpdatedBy;
        }

        public async Task<MasPayType[]> GetArrayPayType()
        {
            IQueryable<MasPayType> qryPayType = null;
            qryPayType = context.MasPayTypes.Where(
                x => _strActive.Equals(x.PayTypeStatus)
            ).AsNoTracking();

            MasPayType[] result = null;
            result = await qryPayType.ToArrayAsync();

            return result;
        }

        public async Task<MasEmployee[]> GetArrayEmployee()
        {
            IQueryable<MasEmployee> qryEmployee = null;
            qryEmployee = context.MasEmployees.Where(
                x => _strActive.Equals(x.WorkStatus)
            ).AsNoTracking();

            MasEmployee[] result = null;
            result = await qryEmployee.ToArrayAsync();
            return result;
        }

        public async Task<MasEmployee> GetEmployee(string pStrEmpCode)
        {
            pStrEmpCode = (pStrEmpCode ?? string.Empty).Trim();
            Expression<Func<MasEmployee, bool>> expEmployee = null;
            expEmployee = x => _strActive.Equals(x.WorkStatus) && pStrEmpCode.Equals(x.EmpCode);
            IQueryable<MasEmployee> qryEmp = null;
            qryEmp = context.MasEmployees.Where(expEmployee).AsNoTracking();
            MasEmployee result = null;
            result = await qryEmp.FirstOrDefaultAsync();
            return result;
        }
    }
}
