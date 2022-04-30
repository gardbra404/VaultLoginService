namespace VaultLoginAPI.Models
{
    public class UserRequest
    {
        public string? Token { get; set; }

        public string? UID { get; set; }

        public string? RequestedKey { get; set; }
    }
}
