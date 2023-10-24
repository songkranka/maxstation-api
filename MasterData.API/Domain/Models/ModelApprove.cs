using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models
{
    public class ModelPendingApprove
    {   
        public string DocName { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public string DocDate { get; set; }
        public string BrnCode { get; set; }
        public string BrnName { get; set; }
        public string Guid { get; set; }
        public string RoutePath { get; set; }

    }
    public class ModelArrayApprove
    {
        public SysApproveHd[] ArrayHeader { get; set; }
        public SysApproveStep[] ArrayStep { get; set; }
        public MasEmployee[] ArrayEmployee { get; set; }
    }
    public class ModelApprove
    {
        public SysApproveHd Header { get; set; }
        public SysApproveStep[] ArrayStep { get; set; }
    }

    public class ModelApproveParam
    {
        public string DocNo { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string EmpCode { get; set; }
        public string DocType { get; set; }
    }

    public class ModelSearchApproveParam
    {
        public string KeyWord { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EmpCode { get; set; }
        public int PageIndex { get; set; }
        public int ItemPerPage { get; set; }
    }
    public class ModelSearchApproveResult
    {
        public SysApproveHd[] ArrApproveHeader { get; set; }
        public MasEmployee[] ArrEmployee { get; set; }
        public int TotalItem { get; set; }
    }
}
