using MasterData.API.Domain.Models.Responses.Price;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IPriceService
    {
        List<OilPrice> GetCurrentOilPriceAsync(string compCode, string brnCode, DateTime systemDate);
    }
}
