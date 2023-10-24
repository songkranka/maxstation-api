using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Repositories;
using DailyOperation.API.Domain.Services;
using DailyOperation.API.Resources.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Services
{
    public class QueueService : IQueueService
    {
        private readonly IQueueRepository _queueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QueueService(
            IQueueRepository queueRepository,
            IUnitOfWork unitOfWork
            )
        {
            _queueRepository = queueRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<TranferPos> TransferPOS(TranferPosResource query)
        {
            var response = new TranferPos();
            var posFunction2List = _queueRepository.GetPosFunction2(query);

            if (posFunction2List.Any())
            {
                var function2s = _queueRepository.CheckDuplicateFunction2(query, posFunction2List);
                await _queueRepository.AddFunction2Async(function2s);
            }

            var posFunction4List = _queueRepository.GetPosFunction4(query);

            if (posFunction4List.Any())
            {
                var function4s = _queueRepository.CheckDuplicateFunction4(query, posFunction4List);
                await _queueRepository.AddFunction4Async(function4s);
            }

            var posFunction5List = _queueRepository.GetPosFunction5(query);

            if (posFunction5List.Any())
            {
                var function5s = _queueRepository.CheckDuplicateFunction5(query, posFunction5List);
                await _queueRepository.AddFunction5Async(function5s);
            }

            var posFunction14List = _queueRepository.GetPosFunction14(query);

            if (posFunction14List.Any())
            {
                var function14s = _queueRepository.CheckDuplicateFunction14(query, posFunction14List);
                await _queueRepository.AddFunction14Async(function14s);
            }

            await _unitOfWork.CompleteAsync();

            response.PosFunction2 = posFunction2List;
            response.PosFunction4 = posFunction4List;
            response.PosFunction5 = posFunction5List;
            response.PosFunction14 = posFunction14List;

            return response;
        }
    }
}
