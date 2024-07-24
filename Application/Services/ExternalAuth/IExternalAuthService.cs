using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.ExternalAuth;

public interface IExternalAuthService
{
    Task<IActionResult> GoogleActionAsync(HttpContext context, string callbackUrl, string redirectUrl);
    Task<IActionResult> GoogleLoginCallbackAsync(HttpContext context);
    Task<IActionResult> GetTokenAsync(HttpContext context);
    Task<IActionResult> WhoAmIAsync(HttpContext context);
}