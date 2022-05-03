namespace VaultLoginAPI.Services
{
    /**
     An interface to describe the functions required by a non-admin user to access their credentials
     */
    public interface ILoginService
    {

        public List<string> GetPermissions(string? UID, string? loginToken);

        public string? GetKey(string? key, string? loginToken);
    }
}
