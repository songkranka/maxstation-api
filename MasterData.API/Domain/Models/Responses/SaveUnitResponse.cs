using MasterData.API.Domain.Services.Communication;
using MaxStation.Entities.Models;

namespace MasterData.API.Domain.Models.Responses
{
    public class SaveUnitResponse : BaseResponse<MasUnit>
    {
        public SaveUnitResponse(MasUnit masUnit) : base(masUnit) { }

        public SaveUnitResponse(string message) : base(message) { }
    }
}
