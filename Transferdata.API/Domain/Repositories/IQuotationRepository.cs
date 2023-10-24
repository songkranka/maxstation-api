using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Quotation;

namespace Transferdata.API.Domain.Repositories
{
    public interface IQuotationRepository
    {
        Task<SalQuotationHd> GetQuotationAsync(QuotationResource query);
        Task<SalQuotationLog> GetLogAsync(string LogStatus,QuotationResource query);
        Task<SalQuotationLog> AddLogAsync(SalQuotationLog log);
        Task<List<QuotationMaxCardResource>> ListByMaxCardAsync(QuotationResource query);
        Task<LogResource> CreateRemainQuotation(QuotationResource obj);  //จากการ save การขาย จะตัด remain 1 ครั้ง
        Task<SalQuotationHd> UpdateRemainQuotation(QuotationResource obj);  //จากการ edit การขาย จะคืนของเก่า แล้ว ตัดของใหม่
        Task<LogResource> CancelRemainQuotation(QuotationResource obj);  //จากการ cancel การขาย จะคืนของเก่า
    }
}
