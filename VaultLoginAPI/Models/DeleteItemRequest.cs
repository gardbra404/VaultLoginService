namespace VaultLoginAPI.Models
{
    public class DeleteItemRequest
    {

        public string? LoginToken { get; set; }

        public string? ItemId { get; set; }
    }
}
