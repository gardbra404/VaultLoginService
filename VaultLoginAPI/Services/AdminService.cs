using Newtonsoft.Json;
using VaultLoginAPI.Models;

namespace VaultLoginAPI.Services
{
    public class AdminService : IAdminService
    {
        public async Task<string> AddItemAsync(NewItemRequest request)
        {
            await RegisterPolicyAsync(request.UID, request.LoginToken);
            
            await RegisterPermissionsAsync(request.Permissions, request.LoginToken, request.UID);
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-Vault-Token", request.LoginToken);
            List<string?> policies = new();
            policies.Add(request.UID);
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("policies", request.UID)
            });

            var response = await client.PostAsync("http://127.0.0.1:8200/v1/auth/token/create", formContent);

            return await response.Content.ReadAsStringAsync();

        }

        private static async Task<string> RegisterPolicyAsync(string? policyName, string? loginToken)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-Vault-Token", loginToken);
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("policy", "path \"store/data/"+policyName+"\" {\n  capabilities = [\"read\"]\n}\npath \"store/cred/\" {\n  capabilities = [\"read\"]\n}"),
            });
            var result = await client.PutAsync("http://127.0.0.1:8200/v1/sys/policy/"+policyName, formContent);

            return await result.Content.ReadAsStringAsync();
        }

        private static async Task<string> RegisterPermissionsAsync(List<string>? permissions, string? loginToken, string? uid)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-Vault-Token", loginToken);
            var formContent = new FormUrlEncodedContent(new[] { 
                new KeyValuePair<string, string>("permissions", JsonConvert.SerializeObject(permissions))
            });

            var result = await client.PutAsync("http://127.0.0.1:8200/v1/store/data/" + uid, formContent);

            return await result.Content.ReadAsStringAsync();
        }

        public Task<string> AddSecretAsync(Secret secret)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateItemAsync(ModifyItemRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
