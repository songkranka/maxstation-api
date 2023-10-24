using MaxStation.Entities.Models;
using Price.API.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price.API.Domain.Services
{
    public interface IStandardPriceService
    {
        Task<ModelStandardPriceResult> GetArrayHeader(ModelStandardPriceParam param);
        Task<string> SaveStandardPrice(ModelStandardPrice pStandardPrice);
        Task<ModelStandardPrice> GetStandardPrice(string pStrGuid);
        Task<MasBranch[]> GetArrayBranch(string pStrComCode);
        Task<ModelStandardPriceProduct> GetArrayProduct(MasProductPrice pInput);
        Task<string> UpdateStatus(OilStandardPriceHd pHeader);
        Task<OilStandardPriceDt[]> GetArrayStandardPriceDetail(string pStrCompCode, string pStrBrnCode);
        Task<OilStandardPriceHd> GetUnApproveDocument(string pStrCompCode);
    }
}
