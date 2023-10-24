using Finance.API.Models;
using Finance.API.Domain.Repositories;
using Finance.API.Helpers;
using Finance.API.Persistence.Context;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Finance.API.Domain.Models.Response;
using System.Linq;
using Finance.API.Domain.Models.Request;
using System;
using Finance.API.Domain.Models.Queries;
using Finance.API.Resources.Expense;
using System.Globalization;

namespace Finance.API.Repositories
{
    public class ExpenseRepository :  BaseRepository, IExpenseRepository
    {
        public ExpenseRepository(AppDbContext context) : base(context) { }

        public async Task<QueryResult<FinExpenseHd>> ListAsync(ExpenseQuery query)
        {
            var qryExpense = _context.FinExpenseHd
             .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode)
             .OrderByDescending(x => x.DocNo)
             .AsNoTracking();


            if (!string.IsNullOrEmpty(query.Keyword))
            {
                qryExpense = qryExpense.Where(p => p.CompCode.Contains(query.Keyword)
                || p.BrnCode.Contains(query.Keyword)
                || p.DocNo.Contains(query.Keyword));
            }

            int totalItems = await qryExpense.CountAsync();
            var resp = await qryExpense.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<FinExpenseHd>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };

        }

        public async Task<List<ExpenseTable>> GetMasExpenseTableAsync(string status, string compCode, string brnCode, string locCode, string docNo)
        {
            var expenseTable = new List<ExpenseTable>();
            var masExpenses = await _context.MasExpense.ToListAsync();
            var employeeSalaryMonth = masExpenses.FirstOrDefault(x => x.ExpenseNo == "0101").ExpenseQty;
            var employeeSalaryDay = masExpenses.FirstOrDefault(x => x.ExpenseNo == "0201").ExpenseQty;

            if (status == "New")
            {
                var headers = masExpenses.Where(x => x.Parent == null);
                

                foreach (var header in headers)
                {
                    var bodies = masExpenses.Where(x => x.Parent == header.ExpenseNo);
                    var bodyList = new List<ExpenseTable.ExpenseTableBody>();
                    var indexList = 0;

                    

                    foreach (var body in bodies)
                    {
                        var expenseTableBody = new ExpenseTable.ExpenseTableBody();
                        expenseTableBody.CategoryId = body.ExpenseNo ?? string.Empty;
                        expenseTableBody.IndexListId = indexList;
                        expenseTableBody.Title = body.ExpenseName ?? string.Empty;
                        //expenseTableBody.Qty = body.ExpenseQty == null ? 0 : (decimal)body.ExpenseQty;
                        

                        expenseTableBody.Qty = body.ExpenseNo switch
                        {
                            "0103" => Math.Round((decimal)((employeeSalaryMonth / 30) / 8) * 1, 2, MidpointRounding.AwayFromZero),
                            "0104" => Math.Round((decimal)((employeeSalaryMonth / 30) / 8) * (decimal)1.5, 2, MidpointRounding.AwayFromZero),
                            "0105" => Math.Round((decimal)((employeeSalaryMonth / 30) / 8) * 2, 2, MidpointRounding.AwayFromZero),
                            "0106" => Math.Round((decimal)((employeeSalaryMonth / 30) / 8) * 3, 2, MidpointRounding.AwayFromZero),
                            "0203" => Math.Round((decimal)(employeeSalaryDay / 8) * 1, 2, MidpointRounding.AwayFromZero),
                            "0204" => Math.Round((decimal)(employeeSalaryDay / 8) * (decimal)1.5, 2, MidpointRounding.AwayFromZero),
                            "0205" => Math.Round((decimal)(employeeSalaryDay / 8) * 2, 2, MidpointRounding.AwayFromZero),
                            "0206" => Math.Round((decimal)(employeeSalaryDay / 8) * 3, 2, MidpointRounding.AwayFromZero),
                            _ => body.ExpenseQty == null ? 0 : (decimal)body.ExpenseQty,
                        };
                        expenseTableBody.DisabledQty = body.LockQty ?? string.Empty;
                        expenseTableBody.LockData = body.LockData ?? string.Empty;
                        expenseTableBody.Unit = body.ExpenseUnit ?? string.Empty;
                        expenseTableBody.Data = body.ExpenseData ?? string.Empty;
                        expenseTableBody.SeqNo = body.SeqNo;


                        bodyList.Add(expenseTableBody);
                        indexList++;
                    }
                    expenseTable.Add(new ExpenseTable
                    {
                        Id = header.ExpenseNo,
                        Header = header.ExpenseName,
                        Body = bodyList
                    });
                }
            }
            else
            {
                var expenseDts = _context.FinExpenseDt.Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode && x.DocNo == docNo).ToList();
                var headers = expenseDts.Where(x => x.Parent == null).ToList();

                foreach (var header in headers)
                {
                    var bodies = expenseDts.Where(x => x.Parent == header.ExpenseNo);
                    var bodyList = new List<ExpenseTable.ExpenseTableBody>();
                    var indexList = 0;

                    foreach (var body in bodies)
                    {
                        var expenseMaster = masExpenses.FirstOrDefault(x => x.ExpenseNo == body.ExpenseNo);

                        //var expenseTableBody = new ExpenseTable.ExpenseTableBody
                        //{
                        //    CategoryId = body.ExpenseNo ?? string.Empty,
                        //    IndexListId = indexList,
                        //    Title = body.BaseName ?? string.Empty,
                        //    Qty = body.BaseQty ?? 0m,
                        //    DisabledQty = expenseMaster.LockQty ?? string.Empty,
                        //    Unit = body.BaseUnit ?? string.Empty,
                        //    Data = body.ItemName ?? string.Empty,
                        //    Number = body.ItemQty ?? 0m,
                        //    Formula = expenseMaster.Formula ?? string.Empty,
                        //    SeqNo = body.SeqNo
                        //};

                        var expenseTableBody = new ExpenseTable.ExpenseTableBody();
                        expenseTableBody.CategoryId = body.ExpenseNo ?? string.Empty;
                        expenseTableBody.IndexListId = indexList;
                        expenseTableBody.Title = body.BaseName ?? string.Empty;
                        expenseTableBody.Qty = body.ExpenseNo switch
                        {
                            "0103" => Math.Round((decimal)((employeeSalaryMonth / 30) / 8) * 1, 2, MidpointRounding.AwayFromZero),
                            "0104" => Math.Round((decimal)((employeeSalaryMonth / 30) / 8) * (decimal)1.5, 2, MidpointRounding.AwayFromZero),
                            "0105" => Math.Round((decimal)((employeeSalaryMonth / 30) / 8) * 2, 2, MidpointRounding.AwayFromZero),
                            "0106" => Math.Round((decimal)((employeeSalaryMonth / 30) / 8) * 3, 2, MidpointRounding.AwayFromZero),
                            "0203" => Math.Round((decimal)(employeeSalaryDay / 8) * 1, 2, MidpointRounding.AwayFromZero),
                            "0204" => Math.Round((decimal)(employeeSalaryDay / 8) * (decimal)1.5, 2, MidpointRounding.AwayFromZero),
                            "0205" => Math.Round((decimal)(employeeSalaryDay / 8) * 2, 2, MidpointRounding.AwayFromZero),
                            "0206" => Math.Round((decimal)(employeeSalaryDay / 8) * 3, 2, MidpointRounding.AwayFromZero),
                            _ => body.BaseQty ?? 0m,
                        };
                        expenseTableBody.DisabledQty = expenseMaster.LockQty ?? string.Empty;
                        expenseTableBody.LockData = expenseMaster.LockData ?? string.Empty;
                        expenseTableBody.Unit = body.BaseUnit ?? string.Empty;
                        expenseTableBody.Data = body.ItemName ?? string.Empty;
                        expenseTableBody.SeqNo = body.SeqNo;
                        expenseTableBody.Number = body.ItemQty;

                        bodyList.Add(expenseTableBody);
                        indexList++;
                    }
                    expenseTable.Add(new ExpenseTable
                    {
                        Id = header.ExpenseNo,
                        Header = header.CateName,
                        Body = bodyList
                    });
                }
            }
            
            return expenseTable;
        }

        public async Task<FinExpenseHd> GetExpenseHdAsync( string compCode, string brnCode, string locCode, Guid guId)
        {
            return await _context.FinExpenseHd.FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode && x.Guid == guId);
        }

        public async Task<FinExpenseHd> GetExpenseHdByDocNoAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            return await _context.FinExpenseHd.FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode && x.DocNo == docNo);
        }

        public async Task<FinExpenseHd> GetExpenseHdByDocDateAsync(string compCode, string brnCode, string locCode, DateTime docDate)
        {
            var docDateString = docDate.ToString("yyyy-MM-dd");
            var expenseDocDate =  DateTime.ParseExact(docDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return await _context.FinExpenseHd.FirstOrDefaultAsync(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode && x.DocDate == expenseDocDate && x.DocStatus == "Active");
        }

        public async Task<List<FinExpenseDt>> GetExpenseDtByDocNoAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            return await _context.FinExpenseDt.Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode && x.DocNo == docNo).ToListAsync();
        }

        public async Task<List<FinExpenseEss>> GetExpenseEssByDocNoAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            return await _context.FinExpenseEss.Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode && x.DocNo == docNo).ToListAsync();
        }

        public async Task<List<ExpenseEssTable>> GetExpenseEssTableAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            var respnse = new List<ExpenseEssTable>();
            var FinExpenseEss =  await _context.FinExpenseEss.Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode && x.DocNo == docNo).ToListAsync();
            var id = 0;

            foreach(var finExpenseEss in FinExpenseEss)
            {
                var expenseEssTable = new ExpenseEssTable();
                expenseEssTable.Id = id;
                expenseEssTable.EssNumber = finExpenseEss.EssNo;
                expenseEssTable.EssDetail = finExpenseEss.EssDesc;
                respnse.Add(expenseEssTable);
            }

            return respnse;
        }

        public async Task<int?> GetExpenseHdMaxRunNumber(string compCode, string brnCode, string locCode, DateTime docDate)
        {
            var result = await _context.FinExpenseHd.Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode
                                                           &&  x.DocDate.Value.Year == docDate.Year && x.DocDate.Value.Month == docDate.Month)
                                                    .MaxAsync(p => p.RunNumber);
            return result;
        }

        public async Task<int> CountExpenseHd(string compCode, string brnCode, string locCode, DateTime docDate)
        {
            var result = await _context.FinExpenseHd.Where(x => x.CompCode == compCode && x.BrnCode == brnCode && x.LocCode == locCode
                                                           && x.DocDate.Value.Year == docDate.Year && x.DocDate.Value.Month == docDate.Month)
                                                    .CountAsync();
            return result;
        }

        public async Task AddExpenseHdAsync(SaveFinExpenseHd expenseHd)
        {
            var request = new FinExpenseHd();
            request.CompCode = expenseHd.CompCode;
            request.BrnCode = expenseHd.BrnCode;
            request.LocCode = expenseHd.LocCode;
            request.DocNo = expenseHd.DocNo;
            request.DocStatus = expenseHd.DocStatus;
            request.DocDate = expenseHd.DocDate;
            request.WorkType = expenseHd.WorkType;
            request.WorkStart = expenseHd.WorkStart;
            request.WorkFinish = expenseHd.WorkFinish;
            request.Remark = expenseHd.Remark;
            request.Post = null;
            request.RunNumber = expenseHd.RunNumber;
            request.DocPattern = expenseHd.DocPattern;
            request.Guid = expenseHd.Guid;
            request.CreatedDate = DateTime.Now;
            request.CreatedBy = expenseHd.CreatedBy;

            await _context.FinExpenseHd.AddAsync(request);
        }

        public async Task AddExpenseDtRangeAsync(List<FinExpenseDt> finExpenseDts)
        {
            await _context.FinExpenseDt.AddRangeAsync(finExpenseDts);
        }

        public async Task AddExpenseEssRangeAsync(List<FinExpenseEss> finExpenseEss)
        {
            await _context.FinExpenseEss.AddRangeAsync(finExpenseEss);
        }

        public void UpdateExpenseHd(FinExpenseHd finExpenseHd)
        {
            _context.FinExpenseHd.Update(finExpenseHd);
        }

        public void UpdateStatus(FinExpenseHd finExpenseHd)
        {
            _context.FinExpenseHd.Update(finExpenseHd);
        }

        public void RemoveRangeExpenseDt(IEnumerable<FinExpenseDt> finExpenseDts)
        {
            _context.FinExpenseDt.RemoveRange(finExpenseDts);
        }

        public void RemoveRangeExpenseEss(IEnumerable<FinExpenseEss> finExpenseEsss)
        {
            _context.FinExpenseEss.RemoveRange(finExpenseEsss);
        }
    }
}
