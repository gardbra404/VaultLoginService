using VaultLoginAPI.Models;
using VaultLoginAPI.Models.VaultResponses;

namespace VaultLoginAPI.Services
{
    public interface IAdminService
    { 

        public AddItemResponse? AddItem(NewItemRequest request);

        public Task<string> UpdateItem(ModifyItemRequest request);

        public Task<string> DeleteItem(string id);

        public Task<string> AddSecret(Secret secret);
    }
}
