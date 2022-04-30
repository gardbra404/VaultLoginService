using Newtonsoft.Json;

namespace VaultLoginAPI.Models.VaultResponses
{
    public class Authorization
    {
        [JsonProperty("client_token")]
        public string? ClientToken { get; set; }

        [JsonProperty("accessor")]
        public string? Accessor { get; set; }

        [JsonProperty("policies")]
        public List<string>? Policies { get; set; }
    }
}
