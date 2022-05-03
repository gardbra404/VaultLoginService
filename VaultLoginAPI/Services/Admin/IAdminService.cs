using VaultLoginAPI.Models;
using VaultLoginAPI.Models.VaultResponses;

namespace VaultLoginAPI.Services
{
    /**
     An interface to describe the actions allowed by an admin user to modify credentials and their access.
     */
    public interface IAdminService
    { 

        public AddItemResponse? AddItem(NewItemRequest request);

        public bool UpdateItem(ModifyItemRequest request);

        public bool DeleteItem(DeleteItemRequest request);

        public string? AddSecret(Secret secret);
    }
}
