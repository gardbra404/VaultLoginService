namespace VaultLoginAPI.Models
{
    /**
     A model containing all of the information used to delete a UID/revoke access.
     */
    public class DeleteItemRequest
    {

        public string? LoginToken { get; set; }

        public string? ItemId { get; set; }
    }
}
