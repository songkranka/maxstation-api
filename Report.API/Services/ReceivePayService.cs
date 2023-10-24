using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class ReceivePayService : IReceivePayService
    {
        private readonly IReceivePayRepository _receivePayRepository;
        public ReceivePayService(IReceivePayRepository receivePayRepository)
        {
            _receivePayRepository = receivePayRepository;
        }

        public async Task<ReceivePayResponse> GetReceivePayPDF(ReceivePayRequest req)
        {
            return await _receivePayRepository.GetReceivePayPDF(req);
        }
    }
}
