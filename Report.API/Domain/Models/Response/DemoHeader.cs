using System.Collections.Generic;

namespace Report.API.Domain.Models.Response
{
    public class header
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public string company_name { get; set; }
        public string company_image { get; set; }

        public List<item> item = new List<item>();
    }

    public class item
    {
        public string id { get; set; }
        public int qty { get; set; }
    }

}
