using Report.API.Domain.Models.Requests;
using static Report.API.Domain.Models.Response.DeliveryCtrlResponse;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Report.API.Domain.Repositories
{
    public interface IDeliveryCtrlRepository
    {
        Task<List<DeliveryCtrl>> GetDocument(DeliveryCtrlRequest request);
    }
}
