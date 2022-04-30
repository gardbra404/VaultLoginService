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
                if (response.RequestID != null)
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
    }
}
