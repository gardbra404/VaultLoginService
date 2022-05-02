
using Newtonsoft.Json;
using VaultLoginAPI.Models;
using VaultLoginAPI.Models.VaultResponses;
using VaultLoginAPI.Services.Utility;

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
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("policies", request.UID ?? "")
            });
            var result = VaultRequestUtility.SendRequestToVault(
                HttpMethod.Post,
                "http://127.0.0.1:8200/v1/auth/token/create",
                formContent,
                request.LoginToken
            );
            return result.Content.ReadAsStream();
        }

        private static void RegisterPolicy(string? policyName, string? loginToken)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("policy", "path \"store/data/"+policyName+"\" {\n  capabilities = [\"read\"]\n}\npath \"store/cred\" {\n  capabilities = [\"read\"]\n}"),
            });
            VaultRequestUtility.SendRequestToVault(
                HttpMethod.Put,
                "http://127.0.0.1:8200/v1/sys/policy/" + policyName,
                formContent,
                loginToken
            );
        }

        private static void RegisterPermissions(List<string>? permissions, string? loginToken, string? uid)
        {
            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("permissions", string.Join(',', permissions?.ConvertAll(x => x.ToUpper()) ?? new List<string>()))
            });
            VaultRequestUtility.SendRequestToVault(
                HttpMethod.Post,
                "http://127.0.0.1:8200/v1/store/data/" + uid,
                formContent,
                loginToken
            );
        }

        public string? AddSecret(Secret secret)
        {
            string? result;

            HttpContent? formContent = GetSecretContent(secret);

            var response = VaultRequestUtility.SendRequestToVault(
                HttpMethod.Post,
                "http://127.0.0.1:8200/v1/store/cred",
                formContent,
                secret.LoginToken
            );
            result = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();

            return result;
        }

        private static HttpContent? GetSecretContent(Secret secret)
        {
            HttpContent? formContent = null;
            if (secret.SecretKey != null && secret.SecretValue != null)
            {
                Dictionary<string, string>? currentCreds = GetCurrentCreds(secret.LoginToken);
                if (currentCreds != null)
                {
                    bool updatedCred = false;
                    foreach (KeyValuePair<string, string> kvp in currentCreds)
                    {
                        if (kvp.Key == secret.SecretKey.ToUpper())
                        {
                            currentCreds[kvp.Key] = secret.SecretValue;
                            updatedCred = true;
                            break;
                        }
                    }
                    if (!updatedCred)
                        currentCreds.Add(secret.SecretKey.ToUpper(), secret.SecretValue);
                    formContent = new FormUrlEncodedContent(currentCreds);
                }
                else
                {
                    formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>(secret.SecretKey.ToUpper(), secret.SecretValue)
                    });
                }
            }
            return formContent;
        }

        private static Dictionary<string, string>? GetCurrentCreds(string? loginToken)
        {
            var result = VaultRequestUtility.SendRequestToVault(
                HttpMethod.Get,
                "http://127.0.0.1:8200/v1/store/cred",
                null,
                loginToken
            );

            string text = new StreamReader(result.Content.ReadAsStream()).ReadToEnd();
            StoreGetResponse? response = JsonConvert.DeserializeObject<StoreGetResponse>(text);
            return response?.Data;
        }

        public bool DeleteItem(DeleteItemRequest request)
        {
            var result = VaultRequestUtility.SendRequestToVault(
                HttpMethod.Delete,
                "http://127.0.0.1:8200/v1/sys/policy/" + request.ItemId,
                null,
                request.LoginToken
            );
            return (new StreamReader(result.Content.ReadAsStream()).ReadToEnd() == null);
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference. Handled by code, VS just isn't pleased
        public bool UpdateItem(ModifyItemRequest request)
        {
            bool result = false;
            if (request != null && request.Validate())
            {
                List<string> currentPermissions = new LoginService().GetPermissions(request.Policy, request.LoginToken);
                List<string> uniqueAdd = request.AddedCreds.ConvertAll(x => x.ToUpper()).FindAll(x =>
                    !request.DeletedCreds.Contains(x)
                    && !currentPermissions.Contains(x)
                );



                List<string> uniqueDel = request.DeletedCreds.ConvertAll(x => x.ToUpper()).FindAll(x =>
                    !request.AddedCreds.Contains(x)
                    && currentPermissions.Contains(x)
                );

                currentPermissions.AddRange(uniqueAdd);
                foreach (string permission in uniqueDel)
                    currentPermissions.Remove(permission);
                RegisterPermissions(currentPermissions, request.Policy, request.LoginToken);
                result = true;
            }
            return result;
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.


    }
}
