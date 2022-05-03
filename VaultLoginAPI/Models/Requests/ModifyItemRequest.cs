namespace VaultLoginAPI.Models
{
    /**
     A model containing all of the information used to modidy a UID and add/revoke access to credentials.
     */
    public class ModifyItemRequest
    {
        public string? LoginToken { get; set; }

        public string? Policy { get; set; }

        public List<string>? AddedCreds { get; set; }

        public List<string>? DeletedCreds { get; set; }

        public bool Validate() => LoginToken != null && Policy != null && AddedCreds != null && DeletedCreds != null;
    }
}
