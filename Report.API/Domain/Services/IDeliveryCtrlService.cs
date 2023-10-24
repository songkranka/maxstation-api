using MaxStation.Entities.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Response.DeliveryCtrlResponse;

namespace Report.API.Domain.Services
{
    public interface IDeliveryCtrlService
    {
        Task<List<DeliveryCtrl>> GetDocument(DeliveryCtrlRequest request);
    }
}
