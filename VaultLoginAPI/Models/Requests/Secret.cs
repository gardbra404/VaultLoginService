namespace VaultLoginAPI.Models
{
    /**
     A model containing all of the information used to add and or update a credential in the secret store.
     */
    public class Secret
    {

        public string? LoginToken { get; set; }

        public string? SecretKey { get; set; }

        public string? SecretValue { get; set; }
    }
}
