namespace VaultLoginAPI.Services
{
    public class LoginService : ILoginService
    {
        public async Task<string> GetKeyAsync(string key)
        {

            HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-Vault-Token", "hvs.ewFMNE2B983cTMo7J6ldmg6r");
            var response = await client.GetAsync("http://127.0.0.1:8200/v1/sys/policy"/*, formContent*/);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<string>> GetPermissionsAsync(string? UID)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("X-Vault-Token", "hvs.ewFMNE2B983cTMo7J6ldmg6r");
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("policy", "path \"store/data/"+UID+"\" {\n  capabilities = [\"create\", \"update\"]\n}"),
            });
            var response = await client.GetAsync("http://127.0.0.1:8200/v1/sys/policy"/*, formContent*/);
        
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return new();
        }
    }
}
