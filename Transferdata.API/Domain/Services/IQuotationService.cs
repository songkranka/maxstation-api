using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Quotation;

namespace Transferdata.API.Domain.Services
{
    public interface IQuotationService
    {
         Task<SalQuotationHd> GetQuotationAsync(QuotationResource query);
        Task<List<QuotationMaxCardResource>> ListByMaxCardAsync(QuotationResource query);

        Task<LogResource> CreateRemainQuotation(QuotationResource obj); //บันทึกขายฝั่ง pos : ตัด remain ครั้งเดียว
        Task<SalQuotationHd> UpdateRemainQuotation(QuotationResource obj); // แก้ไขการขายผั่ง pos : คืน remain เก่า แล้วตัด remain ใหม่
        Task<LogResource> CancelRemainQuotation(QuotationResource obj);// ยกเลิกการขายผั่ง pos : คืน remain เก่ากลับมา
    }
}
