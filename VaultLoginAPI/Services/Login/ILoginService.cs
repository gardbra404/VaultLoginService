namespace VaultLoginAPI.Services
{
    public interface ILoginService
    {

        public List<string> GetPermissions(string? UID, string? loginToken);

        public string? GetKey(string? key, string? loginToken);
    }
}
