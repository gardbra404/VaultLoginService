namespace VaultLoginAPI.Services
{
    public interface ILoginService
    {

        public Task<List<string>> GetPermissionsAsync(string? UID);

        public Task<string> GetKeyAsync(string key);
    }
}
