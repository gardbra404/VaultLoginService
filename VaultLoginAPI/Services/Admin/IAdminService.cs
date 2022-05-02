using VaultLoginAPI.Models;
using VaultLoginAPI.Models.VaultResponses;

namespace VaultLoginAPI.Services
{
    public interface IAdminService
    { 

        public AddItemResponse? AddItem(NewItemRequest request);

        public bool UpdateItem(ModifyItemRequest request);

        public bool DeleteItem(DeleteItemRequest request);

        public string? AddSecret(Secret secret);
    }
}
