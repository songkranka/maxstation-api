using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class RevenueResponse
    {
        public int statusCode { get; set; }
        public string statusMessage { get; set; }
        public List<Revenue> dataList { get; set; }
    }

    public class Revenue
    {
        public string nid { get; set; }
        public string titleName { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string branchTitleName { get; set; }
        public string branchName { get; set; }
        public string branchNumber { get; set; }
        public string buildingName { get; set; }
        public string floorNumber { get; set; }
        public string villageName { get; set; }
        public string roomNumber { get; set; }
        public string houseNumber { get; set; }
        public string mooNumber { get; set; }
        public string soiName { get; set; }
        public string streetName { get; set; }
        public string thumbolName { get; set; }
        public string amphurName { get; set; }
        public string provinceName { get; set; }
        public string postCode { get; set; }
        public string telephone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string businessFirstDate { get; set; }
        public string lastUpdatedDate { get; set; }
        public string custCode { get; set; }
    }
}
