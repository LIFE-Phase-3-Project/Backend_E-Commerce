using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Application.Services.ExternalAuth;

namespace Life_Ecommerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IExternalAuthService _externalauthService;

        public ExternalAuthController(IExternalAuthService externalauthService)
        {
            _externalauthService = externalauthService;
        }

        [HttpGet("google-action")]
        public async Task<IActionResult> GoogleAction()
        {
            var callbackUrl = Url.Action(nameof(GoogleLoginCallback), "Auth");
            var redirectUrl = Url.Action(nameof(GoogleLoginCallback), "Auth");
            return await _externalauthService.GoogleActionAsync(HttpContext, callbackUrl, redirectUrl);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            return await _externalauthService.GoogleLoginCallbackAsync(HttpContext);
        }
        
        // [HttpGet("getlink")]
        // public IActionResult GetLink([FromHeader(Name = "X-Custom-Referer")] string referer)
        // {
        //     if (string.IsNullOrEmpty(referer))
        //     {
        //         return BadRequest("Custom referer header is missing or empty.");
        //     }
        //
        //     return Ok($"Request referer: {referer}");
        // }
        
        // [HttpGet("getlink")]
        // public IActionResult GetLink()
        // {
        //     var referer = HttpContext.Request.Headers["Referer"].ToString();
        //     if (string.IsNullOrEmpty(referer))
        //     {
        //         return BadRequest("Referer header is missing or empty.");
        //     }
        //
        //     return Ok($"Request referer: {referer}");
        // }
        

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("User logged out successfully.");
        }

        [HttpGet("whoami")]
        public async Task<IActionResult> WhoAmI()
        {
            return await _externalauthService.WhoAmIAsync(HttpContext);
        }

        [HttpGet("get-token")]
        public async Task<IActionResult> GetToken()
        {
            return await _externalauthService.GetTokenAsync(HttpContext);
        }
    }
}