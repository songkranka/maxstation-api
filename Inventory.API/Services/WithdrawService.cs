using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.Withdraw;
using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IWithdrawRepository _withdrawRepository;        
        private readonly IUnitOfWork _unitOfWork;
        private readonly PTMaxstationContext _context;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public WithdrawService(
            IWithdrawRepository withdrawRepository,
            IUnitOfWork unitOfWork, 
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _withdrawRepository = withdrawRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _serviceScopeFactory = serviceScopeFactory;
        }


        public async Task<QueryResult<InvWithdrawHd>> ListAsync(WithdrawQuery query)
        {
            return await _withdrawRepository.ListAsync(query);
        }

        public async Task<WithdrawResponse> CreateAsync(InvWithdrawHd withdraw)
        {
            try
            {
                var docpattern = this._context.MasDocPatterns.FirstOrDefault(x => x.DocType == "Withdraw");

                if (docpattern == null)
                {
                    throw new Exception("เกิดข้อผิดพลาดในการสร้างเลขที่เอกสาร กรุณาทำรายการใหม่อีกครั้ง");
                }
                if(await _withdrawRepository.CheckPOSWater(withdraw))
                {
                    throw new Exception(@$"สาขาที่ใช้ POS ไม่สามารถเพิ่ม/แก้ไข เบิกใช้น้ำแถมได้ {Environment.NewLine} กรุณาดึงข้อมูลจาก POS เท่านั้น");
                }

                var pattern = (docpattern == null) ? "" : docpattern.Pattern;
                pattern = pattern.Replace("Brn", withdraw.BrnCode);
                pattern = pattern.Replace("yy", withdraw.DocDate.Value.ToString("yy"));
                pattern = pattern.Replace("MM", withdraw.DocDate.Value.ToString("MM"));
                int runNumber = _withdrawRepository.GetRunNumber(withdraw.CompCode, withdraw.BrnCode, pattern);

                string docNo = string.Empty;
                do
                {
                    docNo = _withdrawRepository.GenDocNo(withdraw, pattern, ++runNumber);
                } while (await _withdrawRepository.IsDupplicateDocNo(withdraw, docNo));

                var head = new InvWithdrawHd
                {
                    CompCode = withdraw.CompCode,
                    BrnCode = withdraw.BrnCode.ToUpper(),
                    LocCode = withdraw.LocCode,
                    DocNo = docNo,
                    DocStatus = "Active",
                    DocDate = withdraw.DocDate,
                    UseBrnCode = withdraw.UseBrnCode,
                    //UseBy = withdraw.UseBy,
                    EmpCode = withdraw.EmpCode,
                    EmpName = withdraw.EmpName,
                    ReasonId = withdraw.ReasonId,
                    ReasonDesc = withdraw.ReasonDesc,
                    Remark = withdraw.Remark,
                    Guid = Guid.NewGuid(),
                    Post = "N",
                    RunNumber = runNumber,
                    DocPattern = pattern,
                    CreatedDate = DateTime.Now,
                    CreatedBy = withdraw.CreatedBy,
                    LicensePlate = withdraw.LicensePlate,
                };

                await _withdrawRepository.AddHdAsync(head);

                List<InvWithdrawDt> items = new List<InvWithdrawDt>();
                int SeqNo = 0;
                foreach (var item in withdraw.InvWithdrawDt)
                {
                    decimal stockQty = _withdrawRepository.CalculateStockQty(item.PdId, item.UnitId, item.ItemQty.Value);
                    var withdrawDt = new InvWithdrawDt
                    {
                        CompCode = head.CompCode,
                        BrnCode = head.BrnCode,
                        LocCode = head.LocCode,
                        DocNo = head.DocNo,
                        SeqNo = ++SeqNo,
                        PdId = item.PdId,
                        PdName = item.PdName,
                        UnitId = item.UnitId,
                        UnitBarcode = item.UnitBarcode,
                        UnitName = item.UnitName,
                        ItemQty = item.ItemQty,
                        StockQty = stockQty,
                    };
                    items.Add(withdrawDt);
                }
                await _withdrawRepository.AddDtAsync(items);
                await _unitOfWork.CompleteAsync();
                return new WithdrawResponse(head);
            }

            catch (Exception ex)
            {
                // Do some logging stuff
                return new WithdrawResponse($"{ex.Message}");
            }
        }

        public async Task<WithdrawResponse> UpdateAsync(Guid guid, InvWithdrawHd withdraw)
        {
            var head = await _withdrawRepository.FindByIdAsync(guid, withdraw.CompCode, withdraw.BrnCode, withdraw.LocCode);
            if (head == null)
                return new WithdrawResponse("withdrawHd not found.");


            var items = await _withdrawRepository.GetDetailAsync(head.CompCode,head.BrnCode,head.LocCode,head.DocNo);
            if (items == null)
                return new WithdrawResponse("Invalid CashSaleDT.");

            if (await _withdrawRepository.CheckPOSWater(withdraw))
            {
                return new WithdrawResponse(@$"สาขาที่ใช้ POS ไม่สามารถเพิ่ม/แก้ไข เบิกใช้น้ำแถมได้ {Environment.NewLine} กรุณาดึงข้อมูลจาก POS เท่านั้น");
            }


            head.DocStatus = withdraw.DocStatus;
            head.DocDate = withdraw.DocDate;
            head.UseBrnCode = withdraw.UseBrnCode;
            //head.UseBy = withdraw.UseBy;
            head.EmpCode = withdraw.EmpCode;
            head.EmpName = withdraw.EmpName;
            head.ReasonId = withdraw.ReasonId;
            head.ReasonDesc = withdraw.ReasonDesc;
            head.Remark = withdraw.Remark;
            head.Post = withdraw.Post;
            head.RunNumber = withdraw.RunNumber;
            head.DocPattern = withdraw.DocPattern;
            head.UpdatedDate = DateTime.Now;
            head.UpdatedBy = withdraw.UpdatedBy;
            head.LicensePlate = withdraw.LicensePlate;

            List<InvWithdrawDt> invWithdrawDts = withdraw.InvWithdrawDt.Select(x => new InvWithdrawDt
            {
                CompCode = x.CompCode,
                BrnCode = x.BrnCode,
                LocCode = x.LocCode,
                DocNo = x.DocNo,
                SeqNo = x.SeqNo,
                PdId = x.PdId,
                PdName = x.PdName,
                UnitId = x.UnitId,
                UnitBarcode = x.UnitBarcode,
                UnitName = x.UnitName,
                ItemQty = x.ItemQty,
                StockQty = x.StockQty
            }).ToList();

            invWithdrawDts.ForEach(x => { x.StockQty = _withdrawRepository.CalculateStockQty(x.PdId, x.UnitId, x.ItemQty.Value); });
            head.InvWithdrawDt = invWithdrawDts;

            try
            {
                _withdrawRepository.UpdateAsync(head);
                _withdrawRepository.RemoveDetailAsync(items);
                _withdrawRepository.AddDetailAsync(head.InvWithdrawDt);

                await _unitOfWork.CompleteAsync();

                return new WithdrawResponse(head);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new WithdrawResponse($"{ex.Message}");
            }
        }

        public async Task<InvWithdrawHd> FindByIdAsync(Guid guid, string compcode, string brnCode, string locCode)
        {
            return await _withdrawRepository.FindByIdAsync(guid, compcode, brnCode, locCode);
        }
        public async Task<MasReason[]> GetReasons()
        {
            return await _withdrawRepository.GetReasons();
        }

        public async Task<MasReasonGroup[]> GetReasonGroups(string pStrReasonId)
        {
            return await _withdrawRepository.GetReasonGroups(pStrReasonId);
        }

        public async Task<WithdrawResponse> CancelAsync(Guid guid, InvWithdrawHd withdraw)
        {
            var head = await _withdrawRepository.FindByIdAsync(guid, withdraw.CompCode, withdraw.BrnCode, withdraw.LocCode);
            if (head == null)
            {
                return new WithdrawResponse("ไม่พบรายการเอกสารในระบบ");
            }
           
            if (await _withdrawRepository.CheckPOSWater(withdraw))
            {
                return new WithdrawResponse(@$"สาขาที่ใช้ POS ไม่สามารถเพิ่ม/แก้ไข เบิกใช้น้ำแถมได้ {Environment.NewLine} กรุณาดึงข้อมูลจาก POS เท่านั้น");
            }


            try
            {
                await _withdrawRepository.CancelAsync(head);
                await _unitOfWork.CompleteAsync();
                return new WithdrawResponse(head);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new WithdrawResponse($"{ex.Message}");
            }
        }
    }
}
