using Microsoft.AspNetCore.Mvc;
using VaultLoginAPI.Models;
using VaultLoginAPI.Models.VaultResponses;
using VaultLoginAPI.Services;

namespace VaultLoginAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

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


        [HttpPost("AddCred")]
        public string? AddNewCredential(Secret secret)
        {
            return _adminService.AddSecret(secret);
        }

    }
}
