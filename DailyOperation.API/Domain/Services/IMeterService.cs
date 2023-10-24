using DailyOperation.API.Domain.Models.Meter;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using System;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Services
{
    public interface IMeterService
    {        
        Task<MeterResponse> SaveDocument(SaveDocumentRequest req);
        Task<MeterResponse> SubmitDocument(SaveDocumentRequest req);
        Task<MeterResponse> DeleteDocument(DeleteDocumentRequest req);
        Task<GetResponse> GetDocument(GetDocumentRequest req);
        Task<QueryResult<MasBranchMeterResponse>> GetPosMeter(BranchMeterRequest req);
        QueryResult<MasBranchCalibrate> GetMasBranchCalibrate(MasBranchCalibrateRequest req);
        Task<MasReason[]> GetHoldReason();
        Task<MeterResponse> ValidatePOS(string pStrBrnCode, int pIntPeriodNo, DateTime pDatDocDate);
    }
}
