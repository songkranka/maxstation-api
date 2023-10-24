using Newtonsoft.Json;

namespace Vatno.Worker.Domain.Models.response
{
    [JsonObject, Serializable]
    public class VatNoResponse
    {
        [JsonProperty("br_code")] public string brnCode { get; set; }
        [JsonProperty("br_name")] public string brnName { get; set; }
        [JsonProperty("br_addr1")] public string brnAddress { get; set; }
        [JsonProperty("br_postcode")] public string brnPostcode { get; set; }
        [JsonProperty("br_tel")] public string brnPhone { get; set; }
        [JsonProperty("br_compnew")] public string brnCompSname { get; set; }
        [JsonProperty("br_codenew")] public string brnCodeNew { get; set; }
        [JsonProperty("br_vatno")] public string brnBranchNo { get; set; }
        [JsonProperty("compname")] public string compName { get; set; }
        [JsonProperty("compaddr")] public string compAddress { get; set; }
        [JsonProperty("comptel")] public string compPhoneFax { get; set; }
        [JsonProperty("compidno")] public string compRegisterId { get; set; }
        [JsonProperty("compename")] public string compNameEn { get; set; }
        [JsonProperty("compeaddr")] public string compAddressEn { get; set; }
        [JsonProperty("competel")] public string compPhoneFaxEn { get; set; }

    }
}