namespace VaultLoginAPI.Models
{
    public class ModifyItemRequest
    {
        public string? LoginToken { get; set; }

        public string? Policy { get; set; }

        public List<string>? AddedCreds { get; set; }

        public List<string>? DeletedCreds { get; set; }

        public bool Validate() => LoginToken != null && Policy != null && AddedCreds != null && DeletedCreds != null;
    }
}
