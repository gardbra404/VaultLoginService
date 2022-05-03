namespace VaultLoginAPI.Models
{
    /**
     A model containing all of the information used to create an auth token for a UID.
     */
    public class NewItemRequest
    {

        public string? LoginToken { get; set; }

        public string? UID { get; set; }

        public List<string>? Permissions { get; set; }

        public bool Validate() => LoginToken != null && UID != null && Permissions != null && Permissions.Any();
        
    }
}
