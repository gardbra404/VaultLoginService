using Newtonsoft.Json;

namespace VaultLoginAPI.Models.VaultResponses
{
    /**
     A model containing all of the information returned from a request to read from a secret store
     */
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
