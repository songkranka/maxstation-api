using Report.API.Domain.Models.Requests;
using System.Collections.Generic;
using System.IO;
using static Report.API.Domain.Models.Response.StationResponse;

namespace Report.API.Domain.Repositories
{
    public interface IStationRepository
    {
        MemoryStream GetST310Excel(StationRequest req);
        List<ST312Response> GetST312PDF(StationRequest req);
        List<ST313Response> GetST313PDF(StationRequest req);
        MemoryStream GetST313Excel(StationRequest req);
        List<ST315Response> GetST315PDF(StationRequest req);
        MemoryStream GetST315Excel(StationRequest req);
        List<ST316Response> GetST316PDF(StationRequest req);
        MemoryStream GetST316Excel(StationRequest req);
        List<ST317Response> GetST317PDF(StationRequest req);
        MemoryStream GetST317Excel(StationRequest req);
    }
}
