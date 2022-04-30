using VaultLoginAPI.Models;

namespace VaultLoginAPI.Services
{
    public interface IAdminService
    { 

        public Task<string> AddItemAsync(NewItemRequest request);

        public Task<string> UpdateItemAsync(ModifyItemRequest request);

        public Task<string> DeleteItemAsync(string id);

        public Task<string> AddSecretAsync(Secret secret);
    }
}
