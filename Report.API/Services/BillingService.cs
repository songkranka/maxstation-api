using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class BillingService : IBillingService
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BillingService(
            IBillingRepository billingRepository,
            IUnitOfWork unitOfWork)
        {
            _billingRepository = billingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BillingResponse> GetBilling(BillingRequest request)
        {
            try
            {
                var billing = await _billingRepository.GetBillingAsync(request);

                if (billing == null)
                    throw new Exception("Billing not found.");

                await _unitOfWork.CompleteAsync();

                return billing;
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                throw new Exception($"An error occurred when get billing: {strMessage}");
            }
        }

        public async Task<MemoryStream> ExportExcelAsync(BillingRequest req)
        {
            return await _billingRepository.ExportExcelAsync(req);
        }
    }
}
