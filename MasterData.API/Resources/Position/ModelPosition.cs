using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.Position
{
    public class ModelPosition
    {
        public MasPosition Position { get; set; }
        public AutPositionRole[] ArrPositionRole { get; set; }
        //public SysMenu[] ArrMenu { get; set; }
    }

    public class ModelGetPositionListParam
    {
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
    }

    public class ModelGetPositionListResult
    {
        public MasPosition[] ArrPosition { get; set; }
        public int TotalItem { get; set; }
    }
}
