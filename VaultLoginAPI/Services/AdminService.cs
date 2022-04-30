
using Newtonsoft.Json;
using VaultLoginAPI.Models;
using VaultLoginAPI.Models.VaultResponses;

namespace VaultLoginAPI.Services
{
    public class AdminService : IAdminService
    {
        public AddItemResponse? AddItem(NewItemRequest request)
        {
            RegisterPolicy(request.UID, request.LoginToken);
            RegisterPermissions(request.Permissions, request.LoginToken, request.UID);
            string resp = new StreamReader(CreateItemToken(request)).ReadToEnd();
            return JsonConvert.DeserializeObject<AddItemResponse>(resp);
        }

        private static Stream CreateItemToken(NewItemRequest request)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-Vault-Token", request.LoginToken);
            List<string?> policies = new();
            policies.Add(request.UID);
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("policies", request.UID ?? "")
            });
            HttpRequestMessage message = new(HttpMethod.Post, "http://127.0.0.1:8200/v1/auth/token/create");
            message.Headers.Add("X-Vault-Token", request.LoginToken);
            message.Content = formContent;
            var result = client.Send(message);
            return result.Content.ReadAsStream();
        }

        private static void RegisterPolicy(string? policyName, string? loginToken)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-Vault-Token", loginToken);
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("policy", "path \"store/data/"+policyName+"\" {\n  capabilities = [\"read\"]\n}\npath \"store/cred\" {\n  capabilities = [\"read\"]\n}"),
            });
            HttpRequestMessage message = new(HttpMethod.Put, "http://127.0.0.1:8200/v1/sys/policy/" + policyName);
            message.Headers.Add("X-Vault-Token", loginToken);
            message.Content = formContent;
            client.Send(message);
        }

        private static void RegisterPermissions(List<string>? permissions, string? loginToken, string? uid)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-Vault-Token", loginToken);
            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("permissions", JsonConvert.SerializeObject(permissions))
            });

            HttpRequestMessage message = new(HttpMethod.Post, "http://127.0.0.1:8200/v1/store/data/" + uid);
            message.Headers.Add("X-Vault-Token", loginToken);
            message.Content = formContent;
            client.Send(message);
        }

        public Task<string> AddSecret(Secret secret)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteItem(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateItem(ModifyItemRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
