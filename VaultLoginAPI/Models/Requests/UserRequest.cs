namespace VaultLoginAPI.Models
{
    /**
     A model containing all of the information used by a UID to request a credential.
     */
    public class UserRequest
    {
        public string? Token { get; set; }

        public string? UID { get; set; }

        public string? RequestedKey { get; set; }
    }
}
