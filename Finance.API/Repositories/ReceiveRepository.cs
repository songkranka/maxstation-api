using Finance.API.Domain.Models;
using Finance.API.Domain.Models.Queries;
using Finance.API.Domain.Repositories;
using Finance.API.Helpers;
using Finance.API.Resources.Recive;
using Finance.API.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Sale.API.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Finance.API.Repositories
{
    public class ReceiveRepository : SqlDataAccessHelper, IReceiveRepository
    {
        public ReceiveRepository(PTMaxstationContext context) : base(context)
        {

        }
        public async Task<QueryResult<FinReceiveHd>> ListAsync(ReceiveHdQuery query)
        {
            if(query == null)
            {
                return null;
            }
            string strCompCode = string.Empty;
            strCompCode = (query?.CompCode ?? string.Empty).Trim();
            if (0.Equals(strCompCode.Length))
            {
                return null;
            }
            string strBrnCode = string.Empty;
            strBrnCode = (query?.BrnCode ?? string.Empty).Trim();
            if (0.Equals(strBrnCode.Length))
            {
                return null;
            }
            IQueryable<FinReceiveHd> qryReceive = null;
            qryReceive = context.FinReceiveHds.AsNoTracking().Where(
                x => strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && x.DocStatus != "Ready"
            ).OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo);

            string strKeyWord = string.Empty;
            strKeyWord = (query?.Keyword ?? string.Empty).Trim();
            if (!0.Equals(strKeyWord.Length))
            {
                qryReceive = qryReceive.Where(
                    p => (!string.IsNullOrEmpty(p.DocNo) && p.DocNo.Contains(query.Keyword))
                    || (!string.IsNullOrEmpty(p.CustName) && p.CustName.Contains(query.Keyword))
                    || (p.NetAmt.HasValue && p.NetAmt.ToString().Contains(query.Keyword))
                );
            }
            else if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                qryReceive = qryReceive.Where(p => p.DocDate >= query.FromDate && p.DocDate <= query.ToDate);
            }

            int intTotalItems = await qryReceive.CountAsync();
            int intPage = (query?.Page ?? 0) -1;
            int intItemPerPage = query?.ItemsPerPage ?? 0;
            if(intPage >= 0 && intItemPerPage > 0)
            {
                qryReceive = qryReceive
                    .Skip(intPage * intItemPerPage)
                    .Take(intItemPerPage);
            }
            List<FinReceiveHd> listReceive = null;
            listReceive = await qryReceive.ToListAsync();

            QueryResult<FinReceiveHd> result = null;
            result = new QueryResult<FinReceiveHd>()
            {
                Items = listReceive ,
                TotalItems = intTotalItems ,
                ItemsPerPage = intItemPerPage,
            };
            return result;
        }
        /*
        public async Task<QueryResult<FinReceiveHd>> ListAsync(ReceiveHdQuery query)
        {
            var queryable = context.FinReceiveHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                    || p.CustName.ToString().Contains(query.Keyword)
                    || p.NetAmt.ToString().Contains(query.Keyword)
                    || p.DocStatus.Contains(query.Keyword));
            }
            else if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                queryable = queryable.Where(p => p.DocDate >= query.FromDate && p.DocDate <= query.ToDate);
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<FinReceiveHd>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }
        */

        public async Task<FinReceiveHd> FindByIdAsync(Guid guid)
        {
            var response = new FinReceiveHd();
            response = await context.FinReceiveHds.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == guid);

            if (response != null)
            {

                var custPrefix = await context.MasCustomers.Where(x => x.CustCode == response.CustCode).Select(x => x.CustPrefix).FirstOrDefaultAsync();
                response.CustPrefix = custPrefix;

                response.FinReceiveDt = context.FinReceiveDts
                    .Where(dt => dt.CompCode == response.CompCode && dt.BrnCode == response.BrnCode && dt.DocNo == response.DocNo)
                    .OrderBy(y => y.SeqNo).ToList();

                response.FinReceivePay = context.FinReceivePays
                    .Where(dt => dt.CompCode == response.CompCode && dt.BrnCode == response.BrnCode && dt.DocNo == response.DocNo)
                    .OrderBy(y => y.SeqNo).ToList();
            }

            return response;
        }

        public async Task AddHdAsync(FinReceiveHd obj)
        {
            await context.FinReceiveHds.AddAsync(obj);
        }

        public async Task AddDtAsync(FinReceiveDt obj)
        {
            await context.FinReceiveDts.AddAsync(obj);
        }

        public async Task AddPayAsync(FinReceivePay obj)
        {
            await context.FinReceivePays.AddAsync(obj);
        }

        public void UpdateAsync(FinReceiveHd obj)
        {
            context.FinReceiveHds.Update(obj);
        }

        public void AddDtListAsync(IEnumerable<FinReceiveDt> listObj)
        {
            context.FinReceiveDts.AddRange(listObj);
        }
        public void AddPayListAsync(IEnumerable<FinReceivePay> listObj)
        {
            context.FinReceivePays.AddRange(listObj);
        }
        public int GetRunNumber(FinReceiveHd hd)
        {
            int runNumber = 1;
            FinReceiveHd resp = new FinReceiveHd();
            resp = this.context.FinReceiveHds.OrderByDescending(y => y.RunNumber).FirstOrDefault(
                x => (x.DocPattern == hd.DocPattern || hd.DocPattern == "" || hd.DocPattern == null)
            );

            if (resp != null)
            {
                runNumber = (int)resp.RunNumber + 1;
            }
            else
            {
                runNumber = 1;
            }
            return runNumber;
        }

        public async Task<List<FinReceiveDt>> GetFinReceiveDtByDocNoAsync(string docNo)
        {
            return await context.FinReceiveDts.AsNoTracking().Where(x => x.DocNo == docNo).ToListAsync();
        }

        public async Task<List<FinReceivePay>> GetFinReceivePayByDocNoAsync(string docNo)
        {
            return await context.FinReceivePays.AsNoTracking().Where(x => x.DocNo == docNo).ToListAsync();
        }

        public void RemoveDtAsync(IEnumerable<FinReceiveDt> listObj)
        {
            context.FinReceiveDts.RemoveRange(listObj);
        }

        public void RemovePayAsync(IEnumerable<FinReceivePay> listObj)
        {
            context.FinReceivePays.RemoveRange(listObj);
        }

        public async Task<List<FinReceivePay>> GetRemainFinBalanceList(ReceiveQuery query)
        {
            List<FinReceivePay> response = new List<FinReceivePay>();
            //ตรวจสอบกรณีมีการส่ง PDBarcodeList มาใช้ IN
            List<string> docTypeList = new List<string>();
            if (query.DocType != null && query.DocType != "")
            {
                docTypeList = query.DocType.Split(',').ToList();
            }

            var qryfinBalance = (from bal in this.context.FinBalances
                      join bil in this.context.SalBillingDts on bal.DocNo equals bil.TxNo
                      into merge
                      from right in merge.DefaultIfEmpty()
                      where (
                                bal.CompCode == query.CompCode || query.CompCode == "" || query.CompCode == null)
                              && (bal.BrnCode == query.BrnCode || query.BrnCode == "" || query.BrnCode == null)
                              && (bal.LocCode == query.LocCode || query.LocCode == "" || query.LocCode == null)
                              && (docTypeList.Contains(bal.DocType) || query.DocType == "" || query.DocType == null)
                              && (bal.CustCode == query.CustCode || query.CustCode == "" || query.CustCode == null)
                              && (bal.CompCode.Contains(query.Keyword)
                                  || bal.BrnCode.Contains(query.Keyword)
                                  || bal.LocCode.Contains(query.Keyword)
                                  || bal.DocNo.Contains(query.Keyword)
                                  || bal.CustCode.Contains(query.Keyword)
                                  || query.Keyword == ""
                                  || query.Keyword == null
                              )
                              && (bal.BalanceAmt > 0)
                              && (!query.FromDate.HasValue || bal.DocDate >= query.FromDate)
                              && (!query.ToDate.HasValue || bal.DocDate <= query.ToDate)
                                 orderby bal.DocDate
                      select new { bal, right }).AsQueryable();
            var finBalance = await qryfinBalance.ToListAsync();
            if (query.DocTypeFilter == "Billing")
            {
                //กรองเฉพาะที่มีรายการวางบิลเเล้ว
                finBalance = finBalance.Where(x => x.right != null).ToList();
            }

            response = finBalance
                    .Select(x => new FinReceivePay
                    {
                        CompCode = x.bal.CompCode,
                        BrnCode = x.bal.BrnCode,
                        LocCode = x.bal.LocCode,
                        DocNo = "",
                        SeqNo = 0,
                        ItemType = x.bal.CompCode,
                        BillBrnCode = (x.right == null) ? "" : x.right.BrnCode,
                        BillNo = (x.right == null) ? "" : x.right.DocNo,
                        TxBrnCode = x.bal.BrnCode,
                        TxNo = x.bal.DocNo,
                        TxDate = x.bal.DocDate,
                        TxAmt = x.bal.NetAmt,
                        TxBalance = x.bal.BalanceAmt,
                        PayAmt = 0,
                        RemainAmt = x.bal.BalanceAmt,
                    }).ToList();

            return response;
        }
        public async Task UpdateRemainFinBalance(FinReceiveHd obj)
        {
            if (obj.FinReceivePay != null)
            {
                foreach (FinReceivePay row in obj.FinReceivePay)
                {
                    var find = context.FinBalances.FirstOrDefault(
                        x => x.CompCode == row.CompCode
                        && x.BrnCode == row.TxBrnCode
                        && x.LocCode == row.LocCode
                        && x.DocNo == row.TxNo
                        && x.CustCode == obj.CustCode
                        && (x.DocType == "Invoice" || x.DocType == "CreditSale" || x.DocType == "CreditNote" || x.DocType == "DebitNote")
                    ); 
                    if (find != null)
                    {
                        find.BalanceAmt = find.BalanceAmt - row.PayAmt;
                        find.BalanceAmtCur = find.BalanceAmt;
                        find.UpdatedBy = obj.UpdatedBy;
                        find.UpdatedDate = DateTime.Now;
                        context.FinBalances.Update(find);

                        if (find.BalanceAmt < 0) {
                            throw new ArgumentException("จำนวนเงินตั้งหนี้ ไม่พอสำหรับตัดหนี้<br>กรุณาตรวจสอบข้อมูลอีกครั้ง");
                        }
                    }

                    //ใส่ Reference ให้เอกสารใบกำกับภาษี เพื่อไม่ให้สามารถแก้ไขได้เเล้ว เพราะมีการอ้างอิงทำรับชำระเเล้ว
                    /*var findTxIv = context.SalTaxinvoiceHds.FirstOrDefault(
                        x => x.CompCode == row.CompCode
                        && x.BrnCode == row.TxBrnCode
                        && x.LocCode == row.LocCode
                        && x.DocNo == row.TxNo
                        && x.CustCode == obj.CustCode
                    );*/
                    var qryTaxInvoice = context.SalTaxinvoiceHds.Where(
                        x => x.CompCode == row.CompCode
                        && x.BrnCode == row.TxBrnCode
                        && x.LocCode == row.LocCode
                        && x.DocNo == row.TxNo
                        && x.CustCode == obj.CustCode
                    );
                    var findTxIv = await qryTaxInvoice.FirstOrDefaultAsync();
                    if (findTxIv != null)
                    {
                        findTxIv.DocStatus = "Reference";
                        findTxIv.UpdatedBy = obj.UpdatedBy;
                        findTxIv.UpdatedDate = DateTime.Now;
                        context.SalTaxinvoiceHds.Update(findTxIv);
                    }
                    else {
                        //throw new ArgumentException("ไม่พบเอกสารตั้งหนี้ กรุณาติดต่อผู้ดูแลระบบ");
                    }
                }
            }
        }

        public async Task ReturnRemainFinBalance(FinReceiveHd obj)
        {
            if (obj.FinReceivePay != null)
            {
                foreach (FinReceivePay row in obj.FinReceivePay)
                {
                    var find = context.FinBalances.FirstOrDefault(
                            x => x.CompCode == row.CompCode
                            && x.BrnCode == row.TxBrnCode
                            && x.LocCode == row.LocCode
                            && x.DocNo == row.TxNo
                            && x.CustCode == obj.CustCode
                            && (x.DocType == "Invoice" || x.DocType == "CreditNote" || x.DocType == "DebitNote" || x.DocType == "CreditSale")
                        );
                    if (find != null)
                    {
                        find.BalanceAmt = find.BalanceAmt + row.PayAmt;
                        find.BalanceAmtCur = find.BalanceAmtCur + row.PayAmt;
                        find.UpdatedBy = obj.UpdatedBy;
                        find.UpdatedDate = DateTime.Now;
                        context.FinBalances.Update(find);
                    }

                    //ใส่ Reference ให้เอกสารใบกำกับภาษี เพื่อไม่ให้สามารถแก้ไขได้เเล้ว เพราะมีการอ้างอิงทำรับชำระเเล้ว
                    var findTxIv = context.SalTaxinvoiceHds.FirstOrDefault(
                        x => x.CompCode == row.CompCode
                        && x.BrnCode == row.TxBrnCode
                        && x.LocCode == row.LocCode
                        && x.DocNo == row.TxNo
                        && x.CustCode == obj.CustCode
                    );

                    if (findTxIv != null)
                    {
                        if (find.NetAmt == find.BalanceAmt)
                        {
                            //ถ้าเท่ากันเเสดงว่าไม่มีเอกสารไหนอ้างอิงเเล้ว ให้กลับไป Status เดิม
                            findTxIv.DocStatus = "Ready";
                            find.UpdatedBy = obj.UpdatedBy;
                            find.UpdatedDate = DateTime.Now;
                            context.SalTaxinvoiceHds.Update(findTxIv);
                        }
                        else {
                            findTxIv.DocStatus = "Reference";
                            find.UpdatedBy = obj.UpdatedBy;
                            find.UpdatedDate = DateTime.Now;
                            context.SalTaxinvoiceHds.Update(findTxIv);
                        }
                    }
                    //else
                    //{
                    //    throw new ArgumentException("ไม่พบเอกสารตั้งหนี้ กรุณาติดต่อผู้ดูแลระบบ");
                    //}
                }
            }
        }

        public async Task<FinReceivePay[]> GetFinReceivePays(FinReceiveHd pInput)
        {
            if(pInput == null)
            {
                return null;
            }
            IQueryable<FinReceivePay> qryFin = null;
            qryFin = context.FinReceivePays.AsNoTracking();
            if( !string.IsNullOrWhiteSpace(pInput.CompCode))
            {
                qryFin = qryFin.Where(x => pInput.CompCode == x.CompCode);
            }
            if (!string.IsNullOrWhiteSpace(pInput.BrnCode))
            {
                qryFin = qryFin.Where(x => pInput.BrnCode == x.BrnCode);
            }
            if (!string.IsNullOrWhiteSpace(pInput.LocCode))
            {
                qryFin = qryFin.Where(x => x.LocCode == pInput.LocCode);
            }
            if (!string.IsNullOrWhiteSpace(pInput.DocNo))
            {
                qryFin = qryFin.Where(x => x.DocNo == pInput.DocNo);
            }
            FinReceivePay[] result = null;
            result = await qryFin.ToArrayAsync();
            return result;
        }
        public async Task<MasMapping[]> GetMasMapping()
        {
            var qryMapping = context.MasMappings.Where(
                x => new[] { "FinPayType", "FinReceiveType" }.Contains(x.MapValue)
                && x.MapStatus == "Active"
            ).AsNoTracking();
            var result = await qryMapping.ToArrayAsync();
            return result;
        }

        public async Task<ModelSumRecivePayResult[]> SumReceivePay(string pStrComCode, string pStrBrnCode, DateTime pDatDocDate)
        {
            pStrComCode = DefaultService.EncodeSqlString(pStrComCode);
            pStrBrnCode = DefaultService.EncodeSqlString(pStrBrnCode);
            string pStrDocDate = DefaultService.EncodeSqlDate(pDatDocDate);
            string strSql = $@"select RECEIVE_TYPE_ID, isnull(sum(SUB_AMT),0) Sum_Net_Amt 
                    from FIN_RECEIVE_HD(nolock)  
                    where DOC_STATUS <> 'Cancel'  
                    and COMP_CODE = '{pStrComCode}'    -- comp_code
                    and BRN_CODE = '{pStrBrnCode}'   -- brn_code
                    and DOC_DATE = '{pStrDocDate}'  -- doc_date
                    group by RECEIVE_TYPE_ID";
            var result = await DefaultService.GetEntityFromSql<ModelSumRecivePayResult[]>(context, strSql);
            return result;
        }
    }
}
