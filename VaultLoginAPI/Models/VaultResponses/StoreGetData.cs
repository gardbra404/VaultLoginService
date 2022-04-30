using Newtonsoft.Json;

namespace VaultLoginAPI.Models.VaultResponses
{
    public class StoreGetData
    {
        [JsonProperty("permissions")]
        public string? Permissions { get; set; }
    }
}
