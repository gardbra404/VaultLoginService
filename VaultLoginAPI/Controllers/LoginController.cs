using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VaultLoginAPI.Models;
using VaultLoginAPI.Services;

namespace VaultLoginAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService service)
        {
            _loginService = service;
        }

        [HttpPost]
        public string? GetKey(UserRequest request)
        {
            bool allowed = false;
            string? key = null;
            List<string> permissions = _loginService.GetPermissions(request.UID, request.Token);
            foreach (string permission in permissions)
            {
                if (permission == request.RequestedKey)
                {
                    allowed = true;
                    break;
                }
            }
            if (allowed)
            {
                key = _loginService.GetKey(request.RequestedKey, request.Token);
                if (key == null)
                    Response.StatusCode = 404;
            }
            else
                Response.StatusCode = 403;
            return key;
        }
    }
}
