using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Queries
{
    public class MasCosCenterQuery : Query
    {
        public string BrnName { get; set; }
        public string BrnCode { get; set; }
        public string MapBrnCode { get; set; }
        public string CompCode { get; set; }

        public MasCosCenterQuery(string compCode, string brnName, string brnCode, string mapBrnCode, int page, int itemsPerPage) : base(page, itemsPerPage)
        {
            CompCode = compCode;
            BrnName = brnName;
            BrnCode = brnCode;
            MapBrnCode = mapBrnCode;
        }
    }
}
