namespace Transferdata.API.Domain.Models.Queries
{
    public class MaxmeQuery
    {
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string shopid { get; set; }
    }

    public class MaxPosSumQuery
    {
        public string token { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string shopid { get; set; }
    }

}
