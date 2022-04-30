using Newtonsoft.Json;

namespace VaultLoginAPI.Models.VaultResponses
{
    public class StoreGetResponse
    {
        [JsonProperty("request_id")]
        public string? RequestID { get; set; }

        [JsonProperty("lease_id")]
        public string? LeaseID { get; set; }

        [JsonProperty("renewable")]
        public bool? Renewable { get; set; }

        [JsonProperty("data")]
        public Dictionary<string, string>? Data { get; set; }
    }
}
