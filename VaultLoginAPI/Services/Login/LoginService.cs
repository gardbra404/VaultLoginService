using Newtonsoft.Json;
using VaultLoginAPI.Models.VaultResponses;
using VaultLoginAPI.Services.Utility;

namespace VaultLoginAPI.Services
{
    public class LoginService : ILoginService
    {
        public string? GetKey(string? key, string? loginToken)
        {
            var result = VaultRequestUtility.SendRequestToVault(
                HttpMethod.Get, 
                "http://127.0.0.1:8200/v1/store/cred", 
                null, 
                loginToken
            );
            
            string text = new StreamReader(result.Content.ReadAsStream()).ReadToEnd();
            StoreGetResponse? response = JsonConvert.DeserializeObject<StoreGetResponse>(text);
            
            string? respKey = null;
            if (key != null)
                respKey = response?.Data?[key];
            return respKey;
        }

        public List<string> GetPermissions(string? UID, string? loginToken)
        {
            var result = VaultRequestUtility.SendRequestToVault(
                HttpMethod.Get,
                "http://127.0.0.1:8200/v1/store/data/" + UID,
                null,
                loginToken
            );

            string text = new StreamReader(result.Content.ReadAsStream()).ReadToEnd();
            StoreGetResponse? response = JsonConvert.DeserializeObject<StoreGetResponse>(text);

            return VaultRequestUtility.BreakDownListResponse(response?.Data?["permissions"]);
        }

        
    }
}
