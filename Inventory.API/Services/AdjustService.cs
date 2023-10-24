using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.Adjust;
using MaxStation.Entities.Models;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class AdjustService : IAdjustService
    {
        private readonly IAdjustRepository _AdjustRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AdjustService(
            IAdjustRepository AdjustRepository, IUnitOfWork unitOfWork)
        {
            _AdjustRepository = AdjustRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdjustResponse> CreateAsync(InvAdjustHd AdjustHd)
        {
            try
            {

                int runno = _AdjustRepository.GetRunNumber(AdjustHd);
                var docNo = _AdjustRepository.GenDocNo(AdjustHd, runno);

                AdjustHd.DocNo = docNo;
                AdjustHd.Guid = Guid.NewGuid();
                AdjustHd.Post = "N";
                AdjustHd.RunNumber = runno;
                AdjustHd.CreatedDate = DateTime.Now;
                AdjustHd.BrnNameFrom = await _AdjustRepository.GetBranchName(AdjustHd.BrnCodeFrom);

                var listItemDetail = AdjustHd.InvAdjustDt.ToList();
                listItemDetail.ForEach(x => { x.DocNo = docNo; x.StockQty = _AdjustRepository.CalculateStockQty(x.PdId, x.ItemQty.Value); });

                await _AdjustRepository.AddHdAsync(AdjustHd);
                await _AdjustRepository.AddDtAsync(listItemDetail);

                _AdjustRepository.RemainAdjustRequest(AdjustHd);

                await _unitOfWork.CompleteAsync();

                return new AdjustResponse(AdjustHd);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new AdjustResponse($"An error occurred when create  adjust: {ex.Message}");
            }
        }

        public Task<QueryResult<InvAdjustHd>> GetAdjustHDList(AdjustQueryResource query)
        {
            return _AdjustRepository.GetAdjustHDList(query);
        }

        public async Task<InvAdjustHd> FindByIdAsync(Guid guid)
        {
            return await _AdjustRepository.FindByIdAsync(guid);
        }

        public async Task<AdjustResponse> UpdateAsync(Guid guid, InvAdjustHd AdjustHd)
        {
            var head = await _AdjustRepository.FindByIdAsync(guid);
            if (head == null)
                return new AdjustResponse("InvAdjustHd not found.");


            var items = await _AdjustRepository.GetDetailAsync(head.CompCode, head.BrnCode, head.LocCode, head.DocNo);
            if (items == null)
                return new AdjustResponse("Invalid InvAdjustDt.");


            head.DocStatus = AdjustHd.DocStatus;
            head.DocDate = AdjustHd.DocDate;
            head.ReasonId = AdjustHd.ReasonId;
            head.ReasonDesc = AdjustHd.ReasonDesc;
            head.Remark = AdjustHd.Remark;
            head.Post = AdjustHd.Post;
            head.RunNumber = AdjustHd.RunNumber;
            head.DocPattern = AdjustHd.DocPattern;
            head.UpdatedDate = DateTime.Now;
            head.UpdatedBy = AdjustHd.UpdatedBy;

            List<InvAdjustDt> invAdjustDts = AdjustHd.InvAdjustDt.Select(x => new InvAdjustDt
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
                RefQty = x.RefQty,
                ItemQty = x.ItemQty,
                StockQty = x.StockQty,
                UnitPrice= x.UnitPrice,
                UnitCost= x.UnitCost,
                DocType = x.DocType
            }).ToList();

            invAdjustDts.ForEach(x => { x.StockQty = _AdjustRepository.CalculateStockQty(x.PdId, x.ItemQty.Value); });

            head.InvAdjustDt = invAdjustDts;

            try
            {
                _AdjustRepository.UpdateAsync(head);
                _AdjustRepository.RemoveDetailAsync(items);
                _AdjustRepository.AddDetailAsync(head.InvAdjustDt);

                await _unitOfWork.CompleteAsync();

                return new AdjustResponse(head);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new AdjustResponse($"An error occurred when updating the Adjust: {ex.Message}");
            }
        }

        public async Task<MasReason[]> GetReasonAdjusts()
        {
            return await _AdjustRepository.GetReasonAdjusts();
        }
    }
}
