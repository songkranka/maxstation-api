using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostDay.API.Domain.Models;
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Repositories;
using PostDay.API.Domain.Services;
using PostDay.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Services
{
    public class BillPaymentService : IBillPaymentService
    {
        private readonly IBillPaymentRepository _repo;
        private readonly IUnitOfWork _unitOfWork = null;

        public BillPaymentService(IBillPaymentRepository repository , IUnitOfWork pUnitOfWork)
        {
            _repo = repository;
            _unitOfWork = pUnitOfWork;
        }

        public async Task<List<MasBank>> GetBankList()
        {
            return await _repo.GetBankList();
        }

        public async Task<GetPostDayResult> GetPostDay(GetPostDayParam param)
        {
            return await _repo.GetPostDay(param);
        }

        public async Task<UpdateBillPaymentResult> UpdateBillPayment(UpdateBillPaymentParam param)
        {
            var result = new UpdateBillPaymentResult();
            try
            {
                await _repo.UpdateBillPayment(param);
                await _unitOfWork.CompleteAsync();

                result.Result = true;
                result.Message = "Update Complete";
                return result;
            }
            catch(Exception ex)
            {
                result.Result = false;
                result.Message = ex.Message;
                throw new Exception(ex.Message);
            }

        }
    }
}
