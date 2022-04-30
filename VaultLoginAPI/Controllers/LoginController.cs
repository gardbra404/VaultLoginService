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
        public async Task<string> GetKey(UserRequest request)
        {
            var item = await _loginService.GetKeyAsync(request.Token);
            return item;
        }
    }
}
