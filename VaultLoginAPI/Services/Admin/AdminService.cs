
using Newtonsoft.Json;
using VaultLoginAPI.Models;
using VaultLoginAPI.Models.VaultResponses;
using VaultLoginAPI.Services.Utility;

namespace VaultLoginAPI.Services
{
    /**
     Implenetation of the IAdminService, used to create, modify and delete user access to credentials
     as well as modify credentials as needed.
     */
    public class AdminService : IAdminService
    {
        /*
        Creates a new, basic auth token with access to a defined set of credentials it can access.
        Stored credentials a saved at the location store/data/{{UID}} to allow only it and authorized
        users to access its permissions (secured through a policy tied to the UID). Tokens created here
        are only given read access.
        
        @param request: A combination of the authorization token for login, the unique identifier that will be
                        tied to the item being registered and a list of credentials that the resulting token will
                        be allowed to access.

        @return A deserialized response from Vault, including the client's token if the proper login was provided. 
         */
        public AddItemResponse? AddItem(NewItemRequest request)
        {
            RegisterPolicy(request.UID, request.LoginToken);
            RegisterPermissions(request.Permissions, request.LoginToken, request.UID);
            string resp = new StreamReader(CreateItemToken(request)).ReadToEnd();
            return JsonConvert.DeserializeObject<AddItemResponse>(resp);
        }


        /**
         Registers a new authorization token for an item with the policy attached to its specific UID.
         
        @param request: A combination of the authorization token for login, the unique identifier that will be
                        tied to the item being registered and a list of credentials that the resulting token will
                        be allowed to access.

        @return the stringified version of the response from Vault which will be deserialized and interpreted later
         */
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


        /**
         Registers a new policy for a UID to allow it access to two secret store endpoints, 
         credentials and its own permissions endpoint (located at store/data/{{UID}}). 
         A request to this endpoint has no body and, as such, return nothing.

        @param policyName the UID for the item that will be registered, which will be tied to a
                          secret store to house the list of credentials that it has access to.
        
        @param loginToken the authorization token passed to Vault to perform this operation which
                          will either be accepted or rejected depending upon its permissions.

        @throws HttpExcpetion in the event that URL is malformed.

         */
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

        /**
         Stores a list of permissions for a UID in a secret store location marked with the same name.
         These permissions detail the list of credentials that the the UID, when paired with its auth token,
         will have access to. The request returns no body, and as such, the function is void.
         
         @param permissions a list of all the credential names that the UID will have access to, these map to items
                            that are stored in Key Value pairs located at store/cred.

        @param loginToken the authorization token passed to Vault to perform this operation which
                          will either be accepted or rejected depending upon its permissions.

        @param uid the UID for the item that will be registered, which will be tied to a
                          secret store to house the list of credentials that it has access to.

        @throws HttpExcpetion in the event that URL is malformed.
         */
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


        /**
         Allows for an authorized account to add/modify a credential located within the key value
         store. I'm still working on getting a version 2 Key Value store working so I have to rewrite
         the store every time a secret is added to the store.

         @param secret a collection of the authorized login token, and a key value pair for the secret
                       to update or add in the credential store.

         @return a stringified version of the JSON response from vault, this will later be deserialized
         to determine the results of this operation.
         */
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

        /**
         Creates an HTTP Content object to modify the credentials store. This will search the
         current collection of key value pairs for a matching key to the one provided and update its value.
         If the key is not found, the new key value pair is then added to the store.

         @param secret a collection of the authorized login token, and a key value pair for the secret
                       to update or add in the credential store.

         @return an HttpContent object that will serve as the body of the request to add the credentials.
         This contains all of the key value pairs that will constitute the store.
         */
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

        /**
         Retrieves all of the key value pairs from the credential serets store for an authorized user.
         
         @param loginToken a login token that is authorized to access the credential store, any unauthorized
         token will return nothing.

         @return a dictionary containing all of the key value pairs from the credentials store.
         */
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


        /**
         Revokes the access of a UID/auth token pairing by deleting the policy associated with
         that token. By deleting the policy, the token is no longer able to read its permissions, 
         nor is it able to access the credential store.

         @param request a collection of the authorization token used to delete the policy, as well
                        as the UID that the policy is named for.

         @return whether or not the delete was successful, dentoted by whether the response body is empty
         */
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

        /**
         Updates the permissons that are associated with a specific UID by updating the credential
         store with its name. This allows for an authorized user to both add and revoke the access to
         credentials for a specified UID.
         
         @param request a collection of the auth token used to login to Vault for the operation, the UID 
                        which will be updated as well as a list of credentials to add to the UID and a 
                        list to revoke from it.

         @return whether or not the update was successful, denoted by whether the body is empty or not.
         */
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
