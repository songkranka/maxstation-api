namespace MasterData.API.Domain.Models.Request
{
    public class SaveUnitRequest
    {
        public string UnitId { get; set; }
        public string MapUnitId { get; set; }
        public string UnitStatus { get; set; }
        public string UnitName { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
