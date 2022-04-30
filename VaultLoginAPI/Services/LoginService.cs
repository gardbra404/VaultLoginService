using Newtonsoft.Json;
using System.Text.RegularExpressions;
using VaultLoginAPI.Models.VaultResponses;

namespace VaultLoginAPI.Services
{
    public class LoginService : ILoginService
    {
        public string? GetKey(string? key, string? loginToken)
        {

            HttpClient client = new();
            HttpRequestMessage message = new(HttpMethod.Get, "http://127.0.0.1:8200/v1/store/cred");
            message.Headers.Add("X-Vault-Token", loginToken);
            var result = client.Send(message);
            string text = new StreamReader(result.Content.ReadAsStream()).ReadToEnd();
            StoreGetResponse? response = JsonConvert.DeserializeObject<StoreGetResponse>(text);
            string? respKey = null;
            if(key != null)
                respKey = response?.Data?[key];
            return respKey;
        }

        public List<string> GetPermissions(string? UID, string? loginToken)
        {
            HttpClient client = new();
            HttpRequestMessage message = new(HttpMethod.Get, "http://127.0.0.1:8200/v1/store/data/" + UID);
            message.Headers.Add("X-Vault-Token", loginToken);
            var result = client.Send(message);
            string text = new StreamReader(result.Content.ReadAsStream()).ReadToEnd();
            StoreGetResponse? response = JsonConvert.DeserializeObject<StoreGetResponse>(text);

            return BreakDownListResponse(response?.Data?["permissions"]);
        }

        private static List<string> BreakDownListResponse(string? response)
        {
            string newString = Regex.Replace(response ?? "", "[^A-Za-z0-9]", " ");
            return new(newString.Split(" ", StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
