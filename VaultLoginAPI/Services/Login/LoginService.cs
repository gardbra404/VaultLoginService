using Newtonsoft.Json;
using VaultLoginAPI.Models.VaultResponses;
using VaultLoginAPI.Services.Utility;

namespace VaultLoginAPI.Services
{
    /**
     An implementation of the ILoginService used to allow users to retrieve the list of credentials that
     they are allowed access to and, if authorized, return said credential
     */
    public class LoginService : ILoginService
    {
        /**
         Retrieves a specified key for a UID, the UID must first be authenticated to ensure
         that it has acess to the credential before it is retrieved.
        
         @param key the key of the key value pair which will be return from the credential store.
         @loginToken the authorization token used to access the credential store.

         @return the value associated with the key or null if it is not found.
         */
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

        /**
         Retrieves the list of permissions associated with a specified UID by retrieving them
         from the secret store for that UID located at store/data/{{UID}}.

         @param UID the UID whose credentials are to be looked up.

         @param loginToken the authorization token tied to that specified UID, as that pairing is 
                           the only non admin pairing allowed access to this location.

         @return a list of credential keys that the UID/auth token pairing are allowed access to.
         */
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
