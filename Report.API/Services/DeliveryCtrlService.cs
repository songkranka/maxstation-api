using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Report.API.Services
{
    public class DeliveryCtrlService : IDeliveryCtrlService
    {
        private readonly IDeliveryCtrlRepository repo;

        public DeliveryCtrlService(IDeliveryCtrlRepository _repo)
        {
            this.repo = _repo;
        }

        public async Task<List<DeliveryCtrlResponse.DeliveryCtrl>> GetDocument(DeliveryCtrlRequest request)
        {
            return await this.repo.GetDocument(request);
        }
    }
}
