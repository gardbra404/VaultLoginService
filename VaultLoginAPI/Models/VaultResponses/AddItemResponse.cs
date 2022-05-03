using Newtonsoft.Json;

namespace VaultLoginAPI.Models.VaultResponses
{
    /**
     A model containing detailing some of the facets contained within a cault response to create a new token
     */
    public class AddItemResponse
    {
        [JsonProperty("request_id")]
        public string? RequestID { get; set; }

        [JsonProperty("lease_id")]
        public string? LeaseID { get; set; }

        [JsonProperty("renewable")]
        public bool? Renewable { get; set; }

        public Authorization? Auth { get; set; }
    }
}
