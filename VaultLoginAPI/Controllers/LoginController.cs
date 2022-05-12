using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VaultLoginAPI.Models;
using VaultLoginAPI.Services;

namespace VaultLoginAPI.Controllers
{
    /**
     A controller used by non admin users to request access to a specific credential given its key.
     */
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService service)
        {
            _loginService = service;
        }


        /**
         Handles the request to retrieve a credential from the store, ensures that the UID/auth token pair
         are permitted to view this pairing.

         @param request a collection of the UID for the item as well as its auth token and the credential
                        it wishes to access.
         @returns if the item is allowed access and the key exists, the key. In the event that they key
         item is authorized, but the key cannot be found, a 404 not found is return. Otherwise, a 403 Forbidden
         is returned
         */
        [HttpPost]
        public string? GetKey(UserRequest request)
        {
            bool allowed = false;
            string? key = null;
            request.RequestedKey = request.RequestedKey?.ToUpper();
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
