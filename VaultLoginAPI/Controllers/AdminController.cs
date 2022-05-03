using Microsoft.AspNetCore.Mvc;
using VaultLoginAPI.Models;
using VaultLoginAPI.Models.VaultResponses;
using VaultLoginAPI.Services;

namespace VaultLoginAPI.Controllers
{
    /**
     A controller used to perform administrative actions on the secret store including creating, modifying and deleting
     users in the credential store system, as well as modifying credentials located within the store.
     */
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        /**
         Allows for an account with admin status to register a new UID with a specified list of credentials.

         @param request a collection of the UID, the admin's auth token and the list of credentials to allow.

         @returns in the event that the user is authorized, and everything is correct, the UID's new token.
         If any item is missing, a 415 is returned, otherwise, if not authenticated, a 403 is returned.
         */
        [HttpPut]
        public string? AddNewItem(NewItemRequest request)
        {
            string? rtnVal = null;
            if (request.Validate())
            {
                AddItemResponse? response = _adminService.AddItem(request);
                if (response?.RequestID != null)
                    rtnVal = response?.Auth?.ClientToken;
                else
                    Response.StatusCode = 403;
            }
            else
            {
                Response.StatusCode = 415;
            }

            return rtnVal;
        }

        /**
         Deletes the policy for a specified UID, therefore revoking its access to credentials.

         @param request a collection of the admin auth token and the UID to revoke.

         @returns either a message stating the deletion was successful or a 403
         */
        [HttpDelete("DeleteItem")]
        public string? DeleteItemPolicy(DeleteItemRequest request)
        {
            string message;
            if (_adminService.DeleteItem(request))
            {
                Response.StatusCode = 200;
                message = string.Format("{0} has been successfully deleted", request.ItemId);
            }
            else
            {
                Response.StatusCode = 403;
                message = "Forbidden";
            }
            return message;
        }

        /**
         Updates the permissions associated with a given UID, is only accessible to admins.

         @param request a collection of the UID to update, the admin auth token, a list of credentials 
                        to add and a list of credentials to revoke.

         @returns a success message or a 403 Forbbiden error code.
         */
        [HttpPost("UpdateItem")]
        public string? UpdateItemPolicy(ModifyItemRequest request)
        {
            string message;
            if(_adminService.UpdateItem(request))
            {
                Response.StatusCode = 200;
                message = string.Format("{0} has been successfully updated", request.Policy);
            }
            else
            {
                Response.StatusCode = 403;
                message = "Forbidden";
            }
            return message;
        }

        /**
        Used to add/update a credential located within the secret store.

        @param secret, the key value pair to add/update as well as the auth token to use.
         
        @returns a success or failure message.
         */
        [HttpPost("AddCred")]
        public string? AddNewCredential(Secret secret)
        {
            return _adminService.AddSecret(secret);
        }

    }
}
