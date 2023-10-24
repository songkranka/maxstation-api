using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.AdjustRequest;
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
    public class AdjustRequestService : IAdjustRequestService
    {
        private readonly IAdjustRequestRepository _AdjustRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AdjustRequestService(
            IAdjustRequestRepository AdjustRequestRepository, IUnitOfWork unitOfWork)
        {
            _AdjustRequestRepository = AdjustRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdjustRequestResponse> CreateAsync(InvAdjustRequestHd AdjustRequestHd)
        {
            try
            {

                int runno = _AdjustRequestRepository.GetRunNumber(AdjustRequestHd);
                var docNo = _AdjustRequestRepository.GenDocNo(AdjustRequestHd, runno);

                var head = new InvAdjustRequestHd
                {
                    CompCode = AdjustRequestHd.CompCode,
                    BrnCode = AdjustRequestHd.BrnCode,
                    LocCode = AdjustRequestHd.LocCode,
                    DocNo = docNo,
                    DocStatus = "Active",
                    DocDate = AdjustRequestHd.DocDate,
                    ReasonId = AdjustRequestHd.ReasonId,
                    ReasonDesc = AdjustRequestHd.ReasonDesc,
                    Remark = AdjustRequestHd.Remark,

                    Guid = Guid.NewGuid(),
                    Post = "N",
                    RunNumber = runno,
                    DocPattern = AdjustRequestHd.DocPattern,
                    CreatedDate = DateTime.Now,
                    CreatedBy = AdjustRequestHd.CreatedBy
                };

                await _AdjustRequestRepository.AddHdAsync(head);

                List<InvAdjustRequestDt> items = new List<InvAdjustRequestDt>();
                int SeqNo = 0;
                foreach (var item in AdjustRequestHd.InvAdjustRequestDt)
                {
                    decimal stockQty = _AdjustRequestRepository.CalculateStockQty(item.PdId, item.UnitId, item.ItemQty.Value);
                    var AdjustRequestDt = new InvAdjustRequestDt
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
                        StockRemain = stockQty
                    };
                    items.Add(AdjustRequestDt);
                }
                await _AdjustRequestRepository.AddDtAsync(items);

                await _unitOfWork.CompleteAsync();

                return new AdjustRequestResponse(head);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new AdjustRequestResponse($"An error occurred when create request adjust: {ex.Message}");
            }
        }

        public async Task<QueryResult<InvAdjustRequestHd>> GetAdjustRequestHDList(AdjustRequestQueryResource query)
        {
            return await _AdjustRequestRepository.GetAdjustRequestHDList(query);
        }

        public async Task<InvAdjustRequestHd> FindByIdAsync(Guid guid)
        {
            return await _AdjustRequestRepository.FindByIdAsync(guid);
        }

        public async Task<AdjustRequestResponse> UpdateAsync(Guid guid, InvAdjustRequestHd AdjustRequestHd)
        {
            var head = await _AdjustRequestRepository.FindByIdAsync(guid);
            if (head == null)
                return new AdjustRequestResponse("InvAdjustRequestHd not found.");


            var items = await _AdjustRequestRepository.GetDetailAsync(head.CompCode, head.BrnCode, head.LocCode, head.DocNo);
            if (items == null)
                return new AdjustRequestResponse("Invalid InvAdjustRequestDt.");


            head.DocStatus = AdjustRequestHd.DocStatus;
            head.DocDate = AdjustRequestHd.DocDate;
            head.ReasonId = AdjustRequestHd.ReasonId;
            head.ReasonDesc = AdjustRequestHd.ReasonDesc;
            head.Remark = AdjustRequestHd.Remark;
            head.Post = AdjustRequestHd.Post;
            head.RunNumber = AdjustRequestHd.RunNumber;
            head.DocPattern = AdjustRequestHd.DocPattern;
            head.UpdatedDate = DateTime.Now;
            head.UpdatedBy = AdjustRequestHd.UpdatedBy;

            List<InvAdjustRequestDt> invAdjustRequestDts = AdjustRequestHd.InvAdjustRequestDt.Select(x => new InvAdjustRequestDt
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

            invAdjustRequestDts.ForEach(x => { x.StockQty = _AdjustRequestRepository.CalculateStockQty(x.PdId, x.UnitId, x.ItemQty.Value); x.StockRemain = x.StockQty; });
            head.InvAdjustRequestDt = invAdjustRequestDts;

            try
            {
                _AdjustRequestRepository.UpdateAsync(head);
                _AdjustRequestRepository.RemoveDetailAsync(items);
                _AdjustRequestRepository.AddDetailAsync(head.InvAdjustRequestDt);

                await _unitOfWork.CompleteAsync();

                return new AdjustRequestResponse(head);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new AdjustRequestResponse($"An error occurred when updating the AdjustRequest: {ex.Message}");
            }
        }

        public async Task<List<InvAdjustRequestDt>> GetDetailAsync(string compCode, string brnCode, string locCode, string docNo)
        {
            return await _AdjustRequestRepository.GetDetailAsync(compCode, brnCode, locCode, docNo);
        }
    }
}
