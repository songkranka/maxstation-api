using Newtonsoft.Json;

namespace Finance.API.Domain.Models.Response
{
    public class DocPatternResponse
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }

        public DataResponse Data { get; set; }

        public class DataResponse
        {
            public string Success { get; set; }
            public string Message { get; set; }
            public ResourceResponse Resource { get; set; }
        }

        public class ResourceResponse
        {
            public string Pattern { get; set; }
        }
    }
}
