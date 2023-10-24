using System;
using System.Collections.Generic;
using System.Text;

namespace MaxStation.Utility.Helpers.Token
{
    [Serializable]
    public class GenerateTokenModel
    {
        public string UrlEndpoint { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
    }
}
