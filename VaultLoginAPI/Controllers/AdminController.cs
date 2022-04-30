using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using VaultLoginAPI.Models;
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
                JObject obj = JObject.Parse(_adminService.AddItemAsync(request).Result);
                rtnVal = (string)JObject.Parse((string)obj["auth"])["client_token"];
            }
            else
            {
                Response.StatusCode = 403;
            }

            return rtnVal;
        }
    }
}
